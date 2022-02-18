using Autofac;
using Daemon.Shared.Services;
using NLog;
using TestPlugin;

namespace Daemon;

/// <summary>
/// This is the MainApp which is the central point of the Daemon
/// </summary>
public class MainApp {
	private CancellationTokenSource CancellationToken { get; }
	private PluginManager pluginManager;
	private Logger Logger = LogManager.GetLogger(typeof(MainApp).FullName);

	public MainApp(CancellationTokenSource cancellationToken) {
		CancellationToken = cancellationToken;
	}

	/// <summary>
	/// This method starts the Daemon.
	/// </summary>
	public void StartApp() {
		ContainerBuilder containerBuilder = new();

		containerBuilder.RegisterType<EventService>().SingleInstance();
		containerBuilder.RegisterType<ConfigService>();
		containerBuilder.RegisterType<DaemonService>();
		containerBuilder.RegisterType<ApiServerService>();
		containerBuilder.RegisterType<WebsocketService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

		pluginManager = new PluginManager(containerBuilder);
		pluginManager.LoadPlugins();
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