using System.Reflection;
using System.Runtime.Loader;
using Daemon.Basic;
using Daemon.Basic.Utils;
using Daemon.Shared;

namespace Daemon.Core;

public class MainApp : IMainApp {
	private List<DaemonPlugin> plugins = new List<DaemonPlugin>();

	public void StartApp() {
		SysLog.LogInfo("The Daemon is starting");

		SysLog.LogDebug("Try to load the plugins");

		this.loadPlugins();

		SysLog.LogDebug("Successfully loaded all plugins");

		SysLog.LogInfo("Successfully started the Daemon");
	}

	public void StopApp() {
		SysLog.LogDebug("Received a termination signal");
		SysLog.LogInfo("Try to stop the Daemon");

		SysLog.LogInfo("Unload all plugins");

		this.unloadPlugins();

		SysLog.LogInfo("Successfully unloaded all plugins");

		SysLog.LogInfo("Successfully stopped the Daemon");
	}


	private void loadPlugins() {
		List<FileInfo> assembliesToLoad = new List<FileInfo>();
		DirectoryInfo pluginDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "plugins"));

		if (!pluginDirectory.Exists) {
			SysLog.LogInfo("The plugins folder doesnt exist");
			return;
		}

		foreach (DirectoryInfo currentPluginDir in pluginDirectory.GetDirectories("*Plugin")) {
			SysLog.LogDebug("Searching through {0}", currentPluginDir.Name);

			FileInfo[] files = currentPluginDir.GetFiles("*.dll");
			if (!files.Any()) {
				SysLog.LogDebug("No Files found in {0}", currentPluginDir.Name);
				continue;
			}

			foreach (var currentFile in files) {
				SysLog.LogDebug("Found the dll {0}", currentFile.Name);
				assembliesToLoad.Add(currentFile);
			}
		}

		foreach (var foundDll in assembliesToLoad) {
			SysLog.LogDebug("Try to load the Plugin {0}", foundDll.Name);

			try {
				Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(foundDll.FullName);

				SysLog.LogDebug("Successfully loaded the Assembly {0}", assembly.GetName());

				Type plugin = assembly.GetTypes().FirstOrDefault(type => type.BaseType == typeof(DaemonPlugin), null);

				if (plugin == null) {
					SysLog.LogDebug("The Assembly {0} doesn't contain a valid Plugin", assembly.GetName());
					continue;
				}

				SysLog.LogDebug("Found the PluginClass {0}", plugin.FullName ?? "Not Found");

				this.plugins.Add((DaemonPlugin) Activator.CreateInstance(plugin)!);

				SysLog.LogInfo("Successfully loaded the Plugin {0}", assembly.GetName(false));
			} catch (Exception e) {
				SysLog.LogException("During the load of the plugin {0} an error occured", e, foundDll.Name);
			}

			SysLog.LogDebug("Completed the load of the Plugin {0}", foundDll.Name);
		}

		foreach (DaemonPlugin daemonPlugin in plugins) {
			SysLog.LogDebug("Try to execute the LoadMethod");

			try {
				daemonPlugin.OnPluginLoad();

				SysLog.LogInfo("Successfully started the Plugin {0}", daemonPlugin.GetType().FullName ?? "Not Found");
			} catch (Exception e) {
				SysLog.LogException("During the start of the plugin {0}, an error occured", e, daemonPlugin.GetType().FullName ?? "Not Found");
			}
		}

		SysLog.LogInfo("Loaded and started {0} plugins", this.plugins.Count);
	}

	private void unloadPlugins() {
		foreach (var currentPlugin in this.plugins) {
			currentPlugin.OnPluginDisable();
		}
	}
}