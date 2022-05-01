using Autofac;
using Autofac.Core.NonPublicProperty;
using Daemon.Services;
using Daemon.Shared.Services;
using NLog;

namespace Daemon;

/// <summary>
///     This is the MainApp which is the central point of the Daemon
/// </summary>
public class MainApp {
	public static IContainer Container;
	private PluginService _pluginService;
	private readonly Logger Logger = LogManager.GetLogger(typeof(MainApp).FullName);

	public MainApp(CancellationTokenSource applicationCancellationToken) {
		ApplicationCancellationToken = applicationCancellationToken;
		SocketServerCancellationToken = new CancellationTokenSource();
	}

	private CancellationTokenSource ApplicationCancellationToken { get; }
	private CancellationTokenSource SocketServerCancellationToken { get; }

	/// <summary>
	///     This method starts the Daemon.
	/// </summary>
	public async Task StartApp() {
		ContainerBuilder containerBuilder = new();
		_pluginService = new PluginService(containerBuilder);

		registerServices(containerBuilder);
		Container = _pluginService.LoadPlugins();
		IWebsocketService websocketService = Container.Resolve<IWebsocketService>();
		do {
			try {
				// TODO Reset the token here because it might can cause problems after disconnecting... more testing required
				await websocketService.connect(SocketServerCancellationToken);
			} catch (Exception e) {
				Logger.Error("Error while connecting to backend! {}", e);
			}

			Logger.Info("Disconencted from backend! trying to reconnect...");
			Thread.Sleep(3000);
			//} while (!CancellationToken.IsCancellationRequested);
		} while (true);
	}

	private void registerServices(ContainerBuilder containerBuilder) {
		containerBuilder.Register(c => _pluginService).SingleInstance().As<IPluginService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<EventService>().As<IEventService>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<WebsocketService>().As<IWebsocketService>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<ConfigService>().As<IConfigService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<DaemonService>().As<IDaemonService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<ApiServerService>().As<IApiServerService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<ReflectionsService>().As<IReflectionsService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<CommandService>().As<ICommandService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<StatusService>().As<IStatusService>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
	}

	/// <summary>
	///     This method stops the Daemon.
	/// </summary>
	public void StopApp() {
		Logger.Debug("Received a termination signal");

		Logger.Info("Trying to stop Daemon...");

		Logger.Info("Unloading all plugins");

		_pluginService.UnloadPlugins();

		Logger.Info("Successfully unloaded all plugins");

		Logger.Info("Successfully stopped Daemon");

		ApplicationCancellationToken.Cancel();
	}
}