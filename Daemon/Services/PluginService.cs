using System.Runtime.Loader;
using System.Security.Cryptography;
using Autofac;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;
using NLog;

namespace Daemon.Services;

/// <summary>
/// This is the plugin manager which handles the whole loading with the plugins
/// </summary>
public class PluginService : IPluginService {
	private readonly List<DaemonPlugin> _loadedPlugins = new();
	private readonly ContainerBuilder _containerBuilder;
	private readonly Logger _logger = LogManager.GetLogger(typeof(PluginService).FullName);
	private readonly DirectoryInfo _pluginDirectory = new(Path.Combine(Environment.CurrentDirectory, "plugins"));

	public PluginService(ContainerBuilder containerBuilder) {
		_containerBuilder = containerBuilder;
	}

	public List<DaemonPlugin> GetLoadedPlugins() {
		return _loadedPlugins;
	}

	private byte[] GetHash(string file) {
		using MD5 md5 = MD5.Create();
		using FileStream stream = File.OpenRead(file);
		return md5.ComputeHash(stream);
	}

	private FileInfo[] GetPluginsInFolder() {
		Dictionary<FileInfo, byte[]> foundPlugins = new();

		foreach (FileInfo assemblyFile in _pluginDirectory.GetFiles("*.plugin.dll", SearchOption.AllDirectories)) {
			byte[] hash = GetHash(assemblyFile.FullName);

			if (foundPlugins.ContainsValue(hash)) {
				continue;
			}

			_logger.Info($"Found Plugin \"{assemblyFile.Name}\" [{BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant()}]");

			foundPlugins.Add(assemblyFile, hash);
		}

		return foundPlugins.Keys.ToArray();
	}

	/// <summary>
	/// This methods loads the plugins out of the plugins folder.
	/// </summary>
	public IContainer LoadPlugins() {
		if (!_pluginDirectory.Exists) {
			_logger.Info("Creating Plugins folder...");
			_pluginDirectory.Create();
		}

		foreach (FileInfo foundDll in GetPluginsInFolder()) {
			_logger.Debug($"Try to load Plugin \"{foundDll.Name}\"...");

			IEnumerable<Type> types = AssemblyLoadContext.Default.LoadFromAssemblyPath(foundDll.FullName).GetTypes().Where(t => t.BaseType == typeof(DaemonPlugin));

			foreach (Type pluginType in types) {
				DaemonPlugin daemonPlugin = (DaemonPlugin) Activator.CreateInstance(pluginType)!;

				try {
					daemonPlugin.RegisterServices(_containerBuilder);
					_loadedPlugins.Add(daemonPlugin);
					_logger.Debug($"Successfully loaded plugin \"{pluginType.Assembly.GetName()}\"!");
				} catch (Exception e) {
					_logger.Fatal($"During the start of plugin \"{daemonPlugin.GetType().FullName}\" an error occured");
					_logger.Fatal(e);
				}
			}
		}

		IContainer container = _containerBuilder.Build();

		foreach (DaemonPlugin daemonPlugin in _loadedPlugins) {
			try {
				container.InjectUnsetProperties(daemonPlugin);
				daemonPlugin.OnPluginLoad(container);
				_logger.Info($"Successfully started Plugin \"{daemonPlugin.GetType().FullName}\"!");
			} catch (Exception e) {
				_logger.Fatal($"During the start of plugin \"{daemonPlugin.GetType().FullName}\" an error occured");
				_logger.Fatal(e);
			}
		}

		_logger.Info($"Loaded and started {_loadedPlugins.Count} plugins");
		return container;
	}

	/// <summary>
	/// This methods
	/// </summary>
	public void UnloadPlugins() {
		foreach (DaemonPlugin currentPlugin in _loadedPlugins) {
			currentPlugin.OnPluginDisable();
		}
	}
}