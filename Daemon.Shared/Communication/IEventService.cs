namespace Daemon.Shared.Communication;

public interface IEventService {
	void OnEvent<T>(Action<T> onCall) where T : Event;

	void TriggerEvent<T>(Event @event);
}