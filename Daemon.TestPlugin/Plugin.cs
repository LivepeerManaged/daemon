using Autofac;
using Daemon.Shared.Communication;
using Daemon.Shared.Plugins;
using Testing;
using TestPlugin.Services;

namespace TestPlugin;

public class Plugin : DaemonPlugin {
	public override void RegisterServices(ContainerBuilder containerBuilder) {
		containerBuilder.RegisterType<ConfigService>();
		containerBuilder.RegisterType<DaemonService>();
		containerBuilder.RegisterType<ApiServerService>();
		containerBuilder.RegisterType<WebsocketService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
		containerBuilder.RegisterType<WebsocketEventService>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
	}

	public override void OnPluginLoad(IContainer container) {
		try {
			WebsocketService websocketService = container.Resolve<WebsocketService>();

			websocketService.connect();
		} catch (Exception e) {
			Console.WriteLine(e);
			throw;
		}
		//this.Logger.Info(.GetConfig().ApiServer);

		/*
		 * 	var exitEvent = new ManualResetEvent(false);
			var url = new Uri("wss://xxx");
	
			using (var client = new WebsocketClient(url)) {
				client.ReconnectTimeout = TimeSpan.FromSeconds(30);
				client.ReconnectionHappened.Subscribe(info =>
					Log.Information($"Reconnection happened, type: {info.Type}"));
	
				client.MessageReceived.Subscribe(msg => Log.Information($"Message received: {msg}"));
				client.Start();
	
				Task.Run(() => client.Send("{ message }"));
	
				exitEvent.WaitOne();
			}		
		 */
	}

	public override void OnPluginDisable() {
	}
}