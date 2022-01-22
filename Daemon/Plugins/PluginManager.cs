using System.Reflection;
using System.Runtime.Loader;
using Daemon.Basic;
using Daemon.Shared.Plugins;

namespace Daemon.Plugins;

/// <summary>
/// This is the plugin manager which handles the whole loading with the plugins
/// </summary>
public class PluginManager : BaseClass {
	private readonly List<DaemonPlugin> loadedPlugins;

	/// <summary>
	/// Constructor of the pluginmanager
	/// </summary>
	public PluginManager() {
		this.loadedPlugins = new List<DaemonPlugin>();
	}

	/// <summary>
	/// This methods loads the plugins out of the plugins folder.
	/// </summary>
	public void LoadPlugins() {
		List<FileInfo> assembliesToLoad = new List<FileInfo>();
		DirectoryInfo pluginDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "plugins"));

		if (!pluginDirectory.Exists) {
			this.Logger.Info("The plugins folder doesnt exist");
			return;
		}

		foreach (DirectoryInfo currentPluginDir in pluginDirectory.GetDirectories("*Plugin")) {
			this.Logger.Debug("Searching through {0}", currentPluginDir.Name);

			FileInfo[] files = currentPluginDir.GetFiles("*.dll");
			if (!files.Any()) {
				this.Logger.Debug("No Files found in {0}", currentPluginDir.Name);
				continue;
			}

			foreach (var currentFile in files) {
				this.Logger.Debug("Found the dll {0}", currentFile.Name);
				assembliesToLoad.Add(currentFile);
			}
		}

		foreach (var foundDll in assembliesToLoad) {
			this.Logger.Debug("Try to load the Plugin {0}", foundDll.Name);

			try {
				Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(foundDll.FullName);

				this.Logger.Debug("Successfully loaded the Assembly {0}", assembly.GetName());

				Type plugin = assembly.GetTypes().FirstOrDefault(type => type.BaseType == typeof(DaemonPlugin), null);

				if (plugin == null) {
					this.Logger.Debug("The Assembly {0} doesn't contain a valid Plugin", assembly.GetName());
					continue;
				}

				this.Logger.Debug("Found the PluginClass {0}", plugin.FullName ?? "Not Found");

				this.loadedPlugins.Add((DaemonPlugin) Activator.CreateInstance(plugin)!);

				this.Logger.Info("Successfully loaded the Plugin {0}", assembly.GetName(false));
			} catch (Exception e) {
				this.Logger.Fatal("During the load of the plugin {0} an error occured", e, foundDll.Name);
			}

			this.Logger.Debug("Completed the load of the Plugin {0}", foundDll.Name);
		}

		foreach (DaemonPlugin daemonPlugin in this.loadedPlugins) {
			this.Logger.Debug("Try to execute the LoadMethod");

			try {
				daemonPlugin.OnPluginLoad();

				this.Logger.Info("Successfully started the Plugin {0}", daemonPlugin.GetType().FullName ?? "Not Found");
			} catch (Exception e) {
				this.Logger.Fatal("During the start of the plugin {0}, an error occured", e, daemonPlugin.GetType().FullName ?? "Not Found");
			}
		}

		this.Logger.Info("Loaded and started {0} plugins", this.loadedPlugins.Count);
	}

	/// <summary>
	/// This methods
	/// </summary>
	public void UnloadPlugins() {
		foreach (var currentPlugin in this.loadedPlugins) {
			currentPlugin.OnPluginDisable();
		}
	}
}