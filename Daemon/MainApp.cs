using Autofac;
using Daemon.Communication;
using Daemon.Plugins;
using Daemon.Shared.Communication;
using NLog;

namespace Daemon;

/// <summary>
/// This is the MainApp which is the central point of the Daemon
/// </summary>
public class MainApp {
	private PluginManager pluginManager;
	private Logger Logger = LogManager.GetLogger(typeof(MainApp).FullName);

	/// <summary>
	/// This method starts the Daemon.
	/// </summary>
	public void StartApp() {
		this.Logger.Debug("Inject Dependencies...");

		ContainerBuilder containerBuilder = new ContainerBuilder();

		containerBuilder.RegisterType<EventService>().As<IEventService>().As<EventService>().SingleInstance();

		this.Logger.Debug("Loading Plugins...");

		this.pluginManager = new PluginManager(containerBuilder);
		this.pluginManager.LoadPlugins();

		this.Logger.Debug("Plugins Successfully loaded!");
	}

	/// <summary>
	/// This method stops the Daemon.
	/// </summary>
	public void StopApp() {
		this.Logger.Debug("Received a termination signal");

		this.Logger.Info("Try to stop Daemon");

		this.Logger.Info("Unload all plugins");

		this.pluginManager.UnloadPlugins();

		this.Logger.Info("Successfully unloaded all plugins");

		this.Logger.Info("Successfully stopped Daemon");
	}
}