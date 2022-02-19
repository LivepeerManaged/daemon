using System.Reflection;
using System.Text.Json;
using Daemon.Shared.Entities;
using Daemon.Shared.Events;
using SocketIOClient;

namespace Daemon.Shared.Services;

public class WebsocketService {
	private DaemonService DaemonService { get; set; }
	private ApiServerService ApiServerService { get; set; }
	private ReflectionsService ReflectionsService { get; set; }
	private SocketIO? _client;
	private readonly Dictionary<string, List<Action<Event>>> _registeredEvents = new();
	private readonly List<Action<string, Event>> _anyEventList = new();

	public async Task connect(EventHandler onConnected) {
		_client = new SocketIO(DaemonService.GetWebsocketServer(), new SocketIOOptions() {
			Query = new KeyValuePair<string, string>[] {
				new("token", await ApiServerService.DaemonLogin(DaemonService.getId(), DaemonService.GetSecret()))
			}
		});

		_client.OnAny((name, response) => TriggerEvent(name, response.GetValue()));

		_client.OnConnected += onConnected;

		await _client.ConnectAsync();
	}

	public void TriggerEvent(string eventName, JsonElement json) {
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

	public void TriggerEvent(Event e) {
		_client?.EmitAsync(GetEventName(e), e);
	}

	private static string GetEventName(Type e) {
		return TryGetEventAttribute(e, out EventNameAttribute? attribute) ? attribute.EventName : e.Name;
	}

	private static string GetEventName(Event e) {
		return TryGetEventAttribute(e, out EventNameAttribute? attribute) ? attribute.EventName : e.GetType().Name;
	}

	private static bool TryGetEventAttribute(Type e, out EventNameAttribute? attribute) {
		attribute = e.GetCustomAttribute<EventNameAttribute>();
		return attribute != null;
	}

	private static bool TryGetEventAttribute(Event e, out EventNameAttribute? attribute) {
		attribute = e.GetType().GetCustomAttribute<EventNameAttribute>();
		return attribute != null;
	}

	private Type[] GetAllEventTypes() {
		return ReflectionsService.GetAllAttributedTypes<EventNameAttribute>();
	}

	private Type? GetEventTypeByName(string name) {
		return GetAllEventTypes().First(type => type.GetCustomAttribute<EventNameAttribute>()?.EventName == name);
	}
}