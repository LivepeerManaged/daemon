using Daemon.Shared.Communication;
using Daemon.Shared.Communication.Attributes;

namespace TestPlugin; 

[EventName("GetDaemonVersion")]
public class GetDaemonVersionEvent : Event {
	public string Version { get; }
	public GetDaemonVersionEvent(string version) {
		this.Version = version;
	}
}