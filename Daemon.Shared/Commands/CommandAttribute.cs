namespace Daemon.Shared.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute {
	public CommandAttribute(string name, string description = "No description") {
		Name = name;
		Description = description;
	}

	public string Name { get; }
	public string Description { get; }
}