using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text.Json;
using Daemon.Shared.Events;
using Daemon.Shared.Services;
using Newtonsoft.Json;
using NLog;
using SocketIOClient;
using TestPlugin;

namespace Daemon.Services;

public class WebsocketService : IWebsocketService {
	private IDaemonService DaemonService { get; set; }
	private IApiServerService ApiServerService { get; set; }
	private IReflectionsService ReflectionsService { get; set; }
	private ICommandService CommandService { get; set; }
	private Logger Logger = LogManager.GetLogger(typeof(WebsocketService).FullName);

	private SocketIO? _client;
	private readonly Dictionary<string, List<Action<Event>>> _registeredEvents = new();
	private readonly List<Action<string, Event>> _anyEventList = new();
	public async Task connect(CancellationTokenSource socketServerCancellationToken) {
		Logger.Info($"connecting to \"{DaemonService.GetWebsocketServer()}\"...");
		Task<string> daemonLogin = ApiServerService.DaemonLogin(DaemonService.getId(), DaemonService.GetSecret());
		
		if(daemonLogin.IsFaulted)
			throw new Exception("Cannot establish connection to backend!");
			
		_client = new SocketIO(DaemonService.GetWebsocketServer(), new SocketIOOptions() {
			Query = new KeyValuePair<string, string>[] {
				new("token", daemonLogin.Result)
			},
			Reconnection = true,
			ReconnectionDelay = 1000
		});

		_client.OnAny((name, response) => {
			try {
				TriggerEvent(name, response.GetValue());
			} catch (Exception e) {
				Logger.Error("Error while TriggeringEvent {} {}", response.GetValue().GetString(), e);
			}
		});

		_client.On("TriggerCommand", response => {
			// TODO error handling for missing commandname
			try {
				string commandName = response.GetValue().ToString();
				JsonElement parameter = response.GetValue(1);
				Type? commandTypeByName = CommandService.GetCommandTypeByName(commandName);

				if (commandTypeByName == null)
					return;

				object? commandReturnValue = CommandService.TriggerCommand(commandName, parameter.Deserialize<Dictionary<string, JsonElement>>());
				// TODO add logging with command name and args
				if (commandReturnValue != null)
					response.CallbackAsync(JsonConvert.SerializeObject(commandReturnValue));
			} catch (Exception e) {
				Logger.Error("Error while Executing Command {} {}", response.GetValue().GetString(), e);
			}
		});

		_client.OnConnected += (sender, args) => {
			Logger.Info($"Successfully connected to backend!");
			try {
				TriggerEvent(new DaemonReadyEvent());
			} catch (Exception e) {
				Logger.Error("Error while sending DaemonReadyEvent {}", e);
			}
		};
		
		_client.OnDisconnected += (sender, args) => {
			Logger.Error($"Connection to backend lost! trying to reconnect...");
		};
		
		_client.OnReconnected += (sender, args) => {
			Logger.Info($"Sucesfully Reonnected to backend!");
		};

		_client.OnReconnectAttempt += (sender, args) => {
			Logger.Info($"Trying to reconnect...!");
		};
		
		_client.OnReconnectError += (sender, args) => {
			Logger.Info($"Reconecting failed!");
		};
		
		await _client.ConnectAsync();
		socketServerCancellationToken.Token.WaitHandle.WaitOne();
	}

	public void TriggerEvent(string eventName, JsonElement json = new JsonElement()) {
		Type? eventTypeByName = GetEventTypeByName(eventName);

		if (eventTypeByName == null) {
			throw new Exception($"Event class not found for \"{eventName}\"");
		}

		Event? eventFromType = (Event) json.Deserialize(eventTypeByName);

		if (eventFromType == null) {
			throw new Exception("whut?");
		}

		foreach (Action<Event> registeredAction in _registeredEvents.Where(registeredAction => registeredAction.Key == eventName).SelectMany(registeredEvent => registeredEvent.Value)) {
			registeredAction.Invoke(eventFromType);
		}

		foreach (Action<string, Event> action in _anyEventList) {
			action.Invoke(eventName, eventFromType);
		}
	}

	// TODO add OffEvent
	public void OnEvent<T>(Action<T> onCall) where T : Event {
		string eventName = GetEventName(typeof(T));

		if (!_registeredEvents.ContainsKey(eventName)) {
			_registeredEvents.Add(eventName, new List<Action<Event>>());
		}

		_registeredEvents[eventName].Add(e => onCall.Invoke((T) e));
	}

	public void OnEvent(Action<string, object> onCall) {
		_anyEventList.Add(onCall);
	}

	// public async Task TriggerVanillaEvent(string eventName, JsonObject obj) => await _client?.EmitAsync(eventName, obj)!;

	// public void OnVanillaEvent(string eventName, Action<object> onCall) => _client?.On(eventName, onCall);

	public void TriggerEvent(Event e) {
		_client?.EmitAsync(GetEventName(e), e);
	}

	private string GetEventName(Type e) {
		return TryGetEventAttribute(e, out EventAttribute? attribute) ? attribute.EventName : e.Name;
	}

	private string GetEventName(Event e) {
		return TryGetEventAttribute(e, out EventAttribute? attribute) ? attribute.EventName : e.GetType().Name;
	}

	private bool TryGetEventAttribute(Type e, out EventAttribute? attribute) {
		attribute = ReflectionsService.GetAttributeOfType<EventAttribute>(e);
		return attribute != null;
	}

	private bool TryGetEventAttribute(Event e, out EventAttribute? attribute) {
		attribute = ReflectionsService.GetAttributeOfType<EventAttribute>(e.GetType());
		return attribute != null;
	}

	private Type[] GetAllEventTypes() {
		return ReflectionsService.GetAllAttributedTypes<EventAttribute>();
	}

	private Type? GetEventTypeByName(string name) {
		return GetAllEventTypes().First(type => type.GetCustomAttribute<EventAttribute>()?.EventName == name);
	}
}