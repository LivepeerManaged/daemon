namespace Daemon.Shared.Events;

[AttributeUsage(AttributeTargets.Class)]
public class EventAttribute : Attribute {
	public string? EventName { get; }

	public EventAttribute(string eventName) {
		EventName = eventName;
	}
}