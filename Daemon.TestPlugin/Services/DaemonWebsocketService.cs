using Daemon.Shared.Communication;
using SocketIOClient;
using Testing;

namespace TestPlugin.Services;

public class DaemonWebsocketService {
	public DaemonService DaemonService { get; }
	public ApiServerService ApiServerService { get; }
	public IEventService EventService { get; }
	private SocketIO client;

	public DaemonWebsocketService(DaemonService daemonService, ApiServerService apiServerService, IEventService eventService) {
		DaemonService = daemonService;
		ApiServerService = apiServerService;
		EventService = eventService;
	}

	public async void connect() {
		Console.WriteLine(new Uri(DaemonService.GetApiServer(), "/daemon"));
		client = new SocketIO(new Uri(DaemonService.GetApiServer(), "/daemon"));
		
		client.Options.Query = new KeyValuePair<string, string>[] {
			new("token", await ApiServerService.DaemonLogin(DaemonService.getId(), DaemonService.GetSecret()))
		};
		
		EventService.OnEvent<GetDaemonVersionEvent>(e => {
			Console.WriteLine("EVENT!");
			Console.WriteLine(e);
		});
		EventService.TriggerEvent<GetDaemonVersionEvent>(new GetDaemonVersionEvent(new Version(1,0)));

		client.OnConnected += (sender, args) => {
			Console.WriteLine("Connected!");
			client.EmitAsync("test");
			client.EmitAsync("test2");
		};
		client.OnError += (sender, s) => {
			Console.WriteLine("sender");
			Console.WriteLine(sender);
			Console.WriteLine(s);
		};
		client.On("exception", response => {
			Console.WriteLine("error:");
			Console.WriteLine(response);
		});
		Console.WriteLine("Connecting...");

		await client.ConnectAsync();
	}
}