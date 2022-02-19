using Daemon.Shared.Entities;
using Daemon.Shared.Events;

namespace Daemon.Shared.Services;

public class EventService {
	private readonly Dictionary<Type, List<Action<Event>>> _registeredEvents = new();

	public void OnEvent<T>(Action<T> onCall) where T : Event {
		if (!_registeredEvents.ContainsKey(typeof(T))) {
			_registeredEvents.Add(typeof(T), new List<Action<Event>>());
		}

		_registeredEvents[typeof(T)].Add(x => onCall.Invoke((T) x));
	}

	public void TriggerEvent(Event e) {
		foreach (Action<Event> registeredAction in _registeredEvents.Where(registeredAction => registeredAction.Key == e.GetType()).SelectMany(registeredEvent => registeredEvent.Value)) {
			registeredAction.Invoke(e);
		}
	}
}