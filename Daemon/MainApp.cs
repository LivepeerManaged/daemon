using Daemon.Plugins;
using Daemon.Shared.Basic;

namespace Daemon;

/// <summary>
/// This is the MainApp which is the central point of the Daemon
/// </summary>
public class MainApp : BaseClass {
	private readonly PluginManager pluginManager;

	/// <summary>
	/// Constructor of the MainApp
	/// </summary>
	public MainApp() {
		this.pluginManager = new PluginManager();
	}

	/// <summary>
	/// This method starts the Daemon.
	/// </summary>
	public void StartApp() {
		this.Logger.Info("The Daemon is starting");

		this.Logger.Debug("Try to load the plugins");

		this.pluginManager.LoadPlugins();

		this.Logger.Debug("Successfully loaded all plugins");

		this.Logger.Info("Successfully started the Daemon");
	}

	/// <summary>
	/// This method stops the Daemon.
	/// </summary>
	public void StopApp() {
		this.Logger.Debug("Received a termination signal");
		this.Logger.Info("Try to stop the Daemon");

		this.Logger.Info("Unload all plugins");

		this.pluginManager.UnloadPlugins();

		this.Logger.Info("Successfully unloaded all plugins");

		this.Logger.Info("Successfully stopped the Daemon");
	}
}