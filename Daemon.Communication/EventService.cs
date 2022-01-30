using System.Reflection;
using Daemon.Shared.Communication;
using Daemon.Shared.Communication.Attributes;

namespace Daemon.Communication;

public class EventService : IEventService {
	private readonly Dictionary<Type, List<Action<Event>>> registeredEvents = new Dictionary<Type, List<Action<Event>>>();

	public void OnEvent<T>(Action<Event> onCall) where T : Event {
		if (!EventService.checkEvent<T>()) {
			throw new MissingMemberException($"The Attribute EventType is missing on the Class {typeof(T).FullName}");
		}

		if (!this.registeredEvents.ContainsKey(typeof(Event))) {
			this.registeredEvents.Add(typeof(Event), new List<Action<Event>>());
		}

		this.registeredEvents[typeof(Event)].Add(onCall);
	}

	public void TriggerEvent<T>(Event @event) {
		if (!EventService.checkEvent<T>()) {
			throw new MissingMemberException($"The Attribute EventType is missing on the Class {typeof(T).FullName}");
		}

		EventType typeAttribute = EventService.findEventType<T>();

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

	private static EventType findEventType<T>() {
		if (!EventService.checkEvent<T>()) {
			throw new MissingMemberException($"The Attribute EventType is missing on the Class {typeof(T).FullName}");
		}

		return ((EventTypeAttribute) typeof(T).GetCustomAttribute(typeof(EventTypeAttribute))!).Type;
	}

	private static bool checkEvent<T>() {
		return typeof(T).CustomAttributes.Any(x => x.AttributeType == typeof(EventTypeAttribute));
	}
}