using System.Runtime.Loader;
using System.Security.Cryptography;
using Autofac;
using Daemon.Shared.Plugins;
using NLog;

namespace Daemon;

/// <summary>
/// This is the plugin manager which handles the whole loading with the plugins
/// </summary>
public class PluginManager {
	private readonly List<DaemonPlugin> loadedPlugins;
	public ContainerBuilder ContainerBuilder { get; set; }
	public Logger Logger { get; }
	private readonly DirectoryInfo _pluginDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "plugins"));

	public PluginManager(ContainerBuilder containerBuilder, Logger logger) {
		this.loadedPlugins = new List<DaemonPlugin>();
		this.ContainerBuilder = containerBuilder;
		Logger = logger;
	}


	private byte[] GetHash(string file) {
		using MD5 md5 = MD5.Create();
		using FileStream stream = File.OpenRead(file);
		return md5.ComputeHash(stream);
	}

	private FileInfo[] GetPluginsInFolder() {
		Dictionary<FileInfo, byte[]> foundPlugins = new Dictionary<FileInfo, byte[]>();

		foreach (FileInfo assemblyFile in _pluginDirectory.GetFiles("*.plugin.dll", SearchOption.AllDirectories)) {
			byte[] hash = GetHash(assemblyFile.FullName);

			if (foundPlugins.ContainsValue(hash))
				continue;

			Logger.Info($"Found Plugin \"{assemblyFile.Name}\" [{BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant()}]");

			foundPlugins.Add(assemblyFile, hash);
		}

		return foundPlugins.Keys.ToArray();
	}

	/// <summary>
	/// This methods loads the plugins out of the plugins folder.
	/// </summary>
	public IContainer LoadPlugins() {
		if (!_pluginDirectory.Exists) {
			Logger.Info("Creating Plugins folder...");
			_pluginDirectory.Create();
		}

		foreach (FileInfo foundDll in GetPluginsInFolder()) {
			Logger.Debug($"Try to load Plugin \"{foundDll.Name}\"...");

			IEnumerable<Type> types = AssemblyLoadContext.Default.LoadFromAssemblyPath(foundDll.FullName).GetTypes().Where(t => t.BaseType == typeof(DaemonPlugin));

			foreach (Type pluginType in types) {
				DaemonPlugin daemonPlugin = (DaemonPlugin) Activator.CreateInstance(pluginType)!;
				try {
					daemonPlugin.RegisterServices(ContainerBuilder);
					loadedPlugins.Add(daemonPlugin);
					Logger.Debug($"Successfully loaded plugin \"{pluginType.Assembly.GetName()}\"!");
				} catch (Exception e) {
					Logger.Fatal($"During the start of plugin \"{daemonPlugin.GetType().FullName}\" an error occured");
					Logger.Fatal(e);
				}
			}
		}

		IContainer container = ContainerBuilder.Build();

		foreach (DaemonPlugin daemonPlugin in loadedPlugins) {
			try {
				daemonPlugin.OnPluginLoad(container);

				Logger.Info($"Successfully started Plugin \"{daemonPlugin.GetType().FullName}\"!");
			} catch (Exception e) {
				Logger.Fatal($"During the start of plugin \"{daemonPlugin.GetType().FullName}\" an error occured");
				Logger.Fatal(e);
			}
		}

		Logger.Info($"Loaded and started {loadedPlugins.Count} plugins");
		return container;
	}

	/// <summary>
	/// This methods
	/// </summary>
	public void UnloadPlugins() {
		foreach (DaemonPlugin currentPlugin in loadedPlugins) {
			currentPlugin.OnPluginDisable();
		}
	}
}