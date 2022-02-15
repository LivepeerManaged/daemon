using Daemon.Shared.Communication;
using Daemon.Shared.Communication.Attributes;

namespace TestPlugin; 

[EventName("test")]
public class TestEvent : Event {
	public TestEvent() {
	}
}