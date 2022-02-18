namespace Daemon.Shared.Communication.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class OnEventAttribute : Attribute {
	public OnEventAttribute(Type @event) {
		EventClass = @event;
	}

	public Type EventClass { get; }
}