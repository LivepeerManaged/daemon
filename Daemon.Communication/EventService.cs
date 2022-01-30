using System.Reflection;
using Daemon.Shared.Communication;
using Daemon.Shared.Communication.Attributes;

namespace Daemon.Communication;

public class EventService : IEventService {
	private readonly Dictionary<Type, List<Action<Event>>> registeredEvents = new Dictionary<Type, List<Action<Event>>>();

	public void OnEvent<T>(Action<T> onCall) where T : Event {
		this.RegisterEvent(typeof(T), x => onCall.Invoke((T) x));
	}

	public void TriggerEvent<T>(Event @event) {
		if (!EventService.checkEvent(typeof(T))) {
			throw new MissingMemberException($"The Attribute EventType is missing on the Class {typeof(T).FullName}");
		}

		EventType typeAttribute = EventService.findEventType(typeof(T));

		switch (typeAttribute) {
			case EventType.INTERNAL:
				this.CallEvent(@event);
				break;
			case EventType.API:
				throw new NotImplementedException("This Feature is not implemented yet");
				break;
			case EventType.WEBSOCKET:
				throw new NotImplementedException("This Feature is not implemented yet");
				break;
		}
	}

	public void CallEvent<T>(T @event) where T : Event {
		foreach (var registeredEvent in this.registeredEvents) {
			foreach (var registeredAction in registeredEvent.Value) {
				registeredAction.Invoke(@event);
			}
		}
	}

	public void RegisterEvent(Type type, Action<Event> action) {
		if (!EventService.checkEvent(type)) {
			throw new MissingMemberException($"The Attribute EventType is missing on the Class {type.FullName}");
		}

		if (!this.registeredEvents.ContainsKey(typeof(Event))) {
			this.registeredEvents.Add(typeof(Event), new List<Action<Event>>());
		}

		this.registeredEvents[typeof(Event)].Add(action);
	}

	private static EventType findEventType(Type type) {
		if (!EventService.checkEvent(type)) {
			throw new MissingMemberException($"The Attribute EventType is missing on the Class {type.FullName}");
		}

		return ((EventTypeAttribute) type.GetCustomAttribute(typeof(EventTypeAttribute))!).Type;
	}

	private static bool checkEvent(Type type) {
		return type.CustomAttributes.Any(x => x.AttributeType == typeof(EventTypeAttribute));
	}
}