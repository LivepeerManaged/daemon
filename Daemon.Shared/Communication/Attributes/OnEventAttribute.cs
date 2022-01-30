namespace Daemon.Shared.Communication.Attributes;

[System.AttributeUsage(AttributeTargets.Method)]
public class OnEventAttribute : Attribute {
	public OnEventAttribute(Type @event) {
		this.EventClass = @event;
	}

	public Type EventClass { get; }
}