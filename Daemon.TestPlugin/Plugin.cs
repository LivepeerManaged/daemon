using Autofac;
using Daemon.Shared.Plugins;
using Testing;
using TestPlugin.Services;

namespace TestPlugin;

public class Plugin : DaemonPlugin {
	public override void RegisterServices(ContainerBuilder containerBuilder) {
		containerBuilder.RegisterType<ConfigService>();
		containerBuilder.RegisterType<DaemonService>();
		containerBuilder.RegisterType<ApiServerService>();
		containerBuilder.RegisterType<DaemonWebsocketService>().SingleInstance();

	}

	public override void OnPluginLoad(IContainer container) {
		DaemonWebsocketService websocketService = container.Resolve<DaemonWebsocketService>();
		websocketService.connect();
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