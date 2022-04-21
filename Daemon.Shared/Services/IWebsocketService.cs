using System.Text.Json;
using Daemon.Shared.Events;

namespace Daemon.Shared.Services;

public interface IWebsocketService {
	Task connect(CancellationTokenSource socketServerCancellationToken);
	void TriggerEvent(string eventName, JsonElement json = new JsonElement());
	void TriggerEvent(Event e);
	void OnEvent<T>(Action<T> onCall) where T : Event;
	void OnEvent(Action<string, object> onCall);
}