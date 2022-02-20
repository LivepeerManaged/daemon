using Daemon.Shared.Events;

namespace Daemon.Shared.Services;

public interface IEventService {
	void OnEvent<T>(Action<T> onCall) where T : Event;
	void TriggerEvent(Event e);
}