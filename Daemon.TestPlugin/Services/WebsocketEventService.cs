using System.Collections;
using System.Reflection;
using System.Text.Json;
using Daemon.Shared.Communication;
using Daemon.Shared.Communication.Attributes;

namespace TestPlugin.Services;

public class WebsocketEventService {
	private readonly Dictionary<string, List<Action<Event>>> _registeredEvents = new();
	private readonly List<Action<string, Event>> _anyEventList = new();

	public void OnEvent<T>(Action<T> onCall) where T : Event {
		string eventName = GetEventName(typeof(T));

		if (!_registeredEvents.ContainsKey(eventName))
			_registeredEvents.Add(eventName, new List<Action<Event>>());

		_registeredEvents[eventName].Add(e => onCall.Invoke((T) e));
	}

	public void OnEvent(Action<string, object> onCall) {
		_anyEventList.Add(onCall);
	}

	// TODO add OffEvent

	public void TriggerEvent(string eventName, JsonElement json) {
		Type? eventTypeByName = GetEventTypeByName(eventName);

		if (eventTypeByName == null)
			throw new Exception($"Event class not found for \"{eventName}\"");

		Event? eventFromType = (Event) json.Deserialize(eventTypeByName);
		
		if (eventFromType == null)
			throw new Exception("whut?");

		foreach (Action<Event> registeredAction in _registeredEvents.Where(registeredAction => registeredAction.Key == eventName).SelectMany(registeredEvent => registeredEvent.Value))
			registeredAction.Invoke(eventFromType);
	
		foreach (Action<string, Event> action in _anyEventList)
			action.Invoke(eventName, eventFromType);
	}

	private static string GetEventName(Type type) {
		return TryGetEventAttribute(type, out EventNameAttribute? attribute) ? attribute.EventName : type.Name;
	}

	private static bool TryGetEventAttribute(Type type, out EventNameAttribute? attribute) {
		attribute = type.GetCustomAttribute<EventNameAttribute>();
		return attribute != null;
	}

	public Type[] GetAllEventTypes() {
		return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes().Where(t => t.GetCustomAttribute<EventNameAttribute>() != null)).ToArray();
	}

	public Type? GetEventTypeByName(string name) {
		return GetAllEventTypes().First(type => type.GetCustomAttribute<EventNameAttribute>()?.EventName == name);
	}
}