using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using Autofac;
using Daemon.Shared.Entities;
using Daemon.Shared.Exceptions;
using Daemon.Shared.Services;
using NLog;

namespace Daemon.Services;

/// <summary>
///     This is the plugin manager which handles the whole loading with the plugins
/// </summary>
public class PluginService : IPluginService {
	private readonly ContainerBuilder _containerBuilder;
	private readonly Logger _logger = LogManager.GetLogger(typeof(PluginService).FullName);
	private readonly DirectoryInfo _pluginDirectory = new(Path.Combine(Environment.CurrentDirectory, "plugins"));
	private readonly Dictionary<DaemonPlugin, AssemblyInfo> _plugins = new();
	public PluginService(ContainerBuilder containerBuilder) {
		_containerBuilder = containerBuilder;
	}

	public Dictionary<DaemonPlugin, AssemblyInfo> GetPlugins() {
		return _plugins;
	}

	/// <summary>
	///     This methods loads the plugins out of the plugins folder.
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
				Assembly assembly = pluginType.Assembly;
				
				AssemblyInfo assemblyInfo = new AssemblyInfo {
					Assembly = pluginType.Assembly,
					Hash = MD5.Create().ComputeHash(File.OpenRead(assembly.Location)),
					Name = assembly.GetName().Name,
					Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title,
					Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description,
					Version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion, // Why the fuck is it this attribute and not AssemblyVersionAttribute???
				};
				assemblyInfo.Enabled = true;
				try {
					daemonPlugin.RegisterServices(_containerBuilder);
					_plugins.Add(daemonPlugin, assemblyInfo);
					_logger.Debug($"Successfully loaded plugin \"{pluginType.Assembly.GetName()}\"!");
				} catch (Exception e) {
					_logger.Fatal($"During the start of plugin \"{daemonPlugin.GetType().FullName}\" an error occured");
					_logger.Fatal(e);
				}
			}
		}

		IContainer container = _containerBuilder.Build();
		foreach ((DaemonPlugin daemonPlugin, AssemblyInfo _) in _plugins) {
			try {
				container.InjectUnsetProperties(daemonPlugin);
				daemonPlugin.OnPluginLoad(container);
				_logger.Info($"Successfully started Plugin \"{daemonPlugin.GetType().FullName}\"!");
			} catch (Exception e) {
				_logger.Fatal($"During the start of plugin \"{daemonPlugin.GetType().FullName}\" an error occured");
				_logger.Fatal(e);
			}
		}

		_logger.Info($"Loaded and started {_plugins.Count} plugins");
		return container;
	}

	/// <summary>
	///     This methods
	/// </summary>
	public void UnloadPlugins() {
		foreach ((DaemonPlugin currentPlugin, AssemblyInfo info) in _plugins) {
			//_plugins[currentPlugin].Enabled = false;
			info.Enabled = false; // TODO check if this works instead ob the above
			currentPlugin.OnPluginDisable();
		}
	}


	private List<FileInfo> GetPluginsInFolder() {
		List<FileInfo> foundPlugins = new();

		foreach (FileInfo assemblyFile in _pluginDirectory.GetFiles("*.plugin.dll", SearchOption.AllDirectories)) {
			_logger.Info($"Found Plugin \"{assemblyFile.Name}\"");

			foundPlugins.Add(assemblyFile);
		}

		return foundPlugins;
	}

	public KeyValuePair<DaemonPlugin, AssemblyInfo> GetPluginByName(string name) {
		try {
			return GetPlugins().First(pair => pair.Value.Name == name);
		} catch (InvalidOperationException e) {
			throw new PluginNotFoundException(name);
		}
	}
}