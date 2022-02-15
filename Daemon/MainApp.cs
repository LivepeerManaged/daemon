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
	public CancellationTokenSource CancellationToken { get; }
	private PluginManager pluginManager;
	private Logger Logger = LogManager.GetLogger(typeof(MainApp).FullName);

	public MainApp(CancellationTokenSource cancellationToken) {
		CancellationToken = cancellationToken;
	}

	/// <summary>
	/// This method starts the Daemon.
	/// </summary>
	public void StartApp() {
		ContainerBuilder containerBuilder = new ContainerBuilder();

		containerBuilder.RegisterType<EventService>().As<IEventService>().SingleInstance();

		Logger.Info("Loading Plugins...");

		pluginManager = new PluginManager(containerBuilder, Logger);
		pluginManager.LoadPlugins();

		Logger.Info("Plugins Successfully loaded!");
	}

	/// <summary>
	/// This method stops the Daemon.
	/// </summary>
	public void StopApp() {
		Logger.Debug("Received a termination signal");

		Logger.Info("Trying to stop Daemon...");

		Logger.Info("Unloading all plugins");

		pluginManager.UnloadPlugins();

		Logger.Info("Successfully unloaded all plugins");

		Logger.Info("Successfully stopped Daemon");
		
		CancellationToken.Cancel();
	}
}