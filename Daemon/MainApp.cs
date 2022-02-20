using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using Autofac;
using Autofac.Core.NonPublicProperty;
using Daemon.Shared.Commands;
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
		registerServices(containerBuilder);
		pluginManager = new PluginManager(containerBuilder);
		IContainer container = pluginManager.LoadPlugins();

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

		WebsocketService websocketService = container.Resolve<WebsocketService>();
		websocketService.OnEvent<TestEvent>(o => {
			Console.WriteLine("LASDC;ASCASICO");
		});
		
		websocketService.connect((sender, args) => {
			websocketService.TriggerEvent(new TestEvent());
			Console.WriteLine("Connected!");
		});
	}

	private void registerServices(ContainerBuilder containerBuilder) {
		containerBuilder.RegisterType<EventService>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<WebsocketService>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<ConfigService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<DaemonService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<ApiServerService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<ReflectionsService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
		containerBuilder.RegisterType<CommandService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
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