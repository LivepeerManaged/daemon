using Autofac;
using Autofac.Core.NonPublicProperty;
using Daemon.Services;
using Daemon.Shared.Commands;
using Daemon.Shared.Services;
using NLog;

namespace Daemon;

/// <summary>
/// This is the MainApp which is the central point of the Daemon
/// </summary>
public class MainApp {
	private CancellationTokenSource CancellationToken { get; }
	private PluginManager pluginManager;
	private Logger Logger = LogManager.GetLogger(typeof(MainApp).FullName);
	public static IContainer Container;
	public MainApp(CancellationTokenSource cancellationToken) {
		CancellationToken = cancellationToken;
	}

	/// <summary>
	/// This method starts the Daemon.
	/// </summary>
	public void StartApp() {
		ContainerBuilder containerBuilder = new();
		registerServices(containerBuilder);
		pluginManager = new PluginManager(containerBuilder);
		Container = pluginManager.LoadPlugins();

		/*
		 * CommandService commandService = container.Resolve<CommandService>();

		commandService.TriggerCommand("Test", new {
			FirstParameter = "TESTLOL",
			SecondParameter = "SECOND WOOO"
		});

		foreach (CommandParameterAttribute commandParameterAttribute in commandService.GetCommandParameters<TestCommand>()) {
			Console.WriteLine($"{commandParameterAttribute.Name}: {commandParameterAttribute.Description}");
		}
		 */

		IWebsocketService websocketService = Container.Resolve<IWebsocketService>();
		websocketService.connect((sender, args) => {
			Console.WriteLine("Connected!");
		});
	}

	private void registerServices(ContainerBuilder containerBuilder) {
		containerBuilder.RegisterType<EventService>().As<IEventService>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<WebsocketService>().As<IWebsocketService>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<ConfigService>().As<IConfigService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<DaemonService>().As<IDaemonService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<ApiServerService>().As<IApiServerService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<ReflectionsService>().As<IReflectionsService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<CommandService>().As<ICommandService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
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