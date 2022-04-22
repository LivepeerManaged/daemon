namespace Daemon.Shared.Events;

[AttributeUsage(AttributeTargets.Class)]
public class EventAttribute : Attribute {
	public EventAttribute(string eventName) {
		EventName = eventName;
	}

	public string? EventName { get; }
}