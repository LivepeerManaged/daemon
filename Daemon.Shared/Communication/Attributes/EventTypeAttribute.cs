namespace Daemon.Shared.Communication.Attributes;

[System.AttributeUsage(AttributeTargets.Class)]
public class EventTypeAttribute : Attribute {
	public EventTypeAttribute(Daemon.Shared.Communication.EventType type) {
		this.Type = type;
	}

	public Daemon.Shared.Communication.EventType Type { get; }
}