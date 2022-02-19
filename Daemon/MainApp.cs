using System.Dynamic;
using System.Reflection;
using Autofac;
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

		CommandService commandService = container.Resolve<CommandService>();

		commandService.TriggerCommand("Test", new {
			FirstParameter = "TESTLOL",
			SecondParameter = "SECOND WOOO"
		});
		foreach (CommandParameterAttribute commandParameterAttribute in commandService.GetCommandParameters<TestCommand>()) {
			Console.WriteLine($"{commandParameterAttribute.Name}: {commandParameterAttribute.Description}");
		}

	}

	private void registerServices(ContainerBuilder containerBuilder) {
		containerBuilder.RegisterType<EventService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
		containerBuilder.RegisterType<WebsocketService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
		containerBuilder.RegisterType<ConfigService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
		containerBuilder.RegisterType<DaemonService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
		containerBuilder.RegisterType<ApiServerService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
		containerBuilder.RegisterType<ReflectionsService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
		containerBuilder.RegisterType<CommandService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
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