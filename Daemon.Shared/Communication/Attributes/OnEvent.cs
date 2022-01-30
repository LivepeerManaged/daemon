namespace Daemon.Shared.Communication.Attributes;

[System.AttributeUsage(AttributeTargets.Method)]
public class OnEvent : Attribute {
	public OnEvent(Type @event) {
		this.EventClass = @event;
	}

	public Type EventClass { get; }
}