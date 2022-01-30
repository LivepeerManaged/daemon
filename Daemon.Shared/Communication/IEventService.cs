namespace Daemon.Shared.Communication;

public interface IEventService {
	void OnEvent<T>(Action<Event> onCall) where T : Event;

	void TriggerEvent<T>(Event @event);
}