using Autofac;
using Daemon.Communication;
using Daemon.Plugins;
using Daemon.Shared.Basic;
using Daemon.Shared.Communication;

namespace Daemon;

/// <summary>
/// This is the MainApp which is the central point of the Daemon
/// </summary>
public class MainApp : BaseClass {
	private PluginManager pluginManager;

	/// <summary>
	/// This method starts the Daemon.
	/// </summary>
	public void StartApp() {
		this.Logger.Debug("Try to initiate dependency injection");

		ContainerBuilder containerBuilder = new Autofac.ContainerBuilder();

		containerBuilder.RegisterType<EventService>().As<IEventService>().As<EventService>().SingleInstance();

		this.Logger.Debug("Successfully initiated dependency injection");

		this.Logger.Debug("Try to load the plugins");

		this.pluginManager = new PluginManager(containerBuilder);
		this.pluginManager.LoadPlugins();

		this.Logger.Debug("Successfully loaded all plugins");

		this.Logger.Info("Successfully started Daemon");
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