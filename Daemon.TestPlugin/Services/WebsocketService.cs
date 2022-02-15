using System.Text.Json;
using Daemon.Shared.Communication;
using SocketIOClient;
using Testing;

namespace TestPlugin.Services;

public class WebsocketService {
	public DaemonService DaemonService { get; set; }
	public ApiServerService ApiServerService { get; set; }
	public WebsocketEventService WebsocketEventService { get; set; }
	public IEventService EventService { get; set; }

	public async void connect() {
		SocketIO client = new SocketIO(new Uri(DaemonService.GetApiServer(), "/daemon"), new SocketIOOptions() {
			Query = new KeyValuePair<string, string>[] {
				new("token", await ApiServerService.DaemonLogin(DaemonService.getId(), DaemonService.GetSecret()))
			}
		});

		WebsocketEventService.OnEvent((eventName, args) => client.EmitAsync(eventName, args));

		client.OnAny((name, response) => WebsocketEventService.TriggerEvent(name, response.GetValue()));

		await client.ConnectAsync();
	}
}