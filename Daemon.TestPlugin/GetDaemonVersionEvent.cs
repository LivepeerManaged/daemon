using Daemon.Shared.Communication;
using Daemon.Shared.Communication.Attributes;

namespace TestPlugin; 

[EventType(EventType.INTERNAL)]
public class GetDaemonVersionEvent : Event {
	public Version Version { get; }
	public GetDaemonVersionEvent(Version version) {
		this.Version = version;
	}
}