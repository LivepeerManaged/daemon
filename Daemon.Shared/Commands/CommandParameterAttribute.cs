namespace Daemon.Shared.Commands;

[AttributeUsage(AttributeTargets.Property)]
public class CommandParameterAttribute : Attribute {
	public string Name { get; }
	public string Description { get; }

	public CommandParameterAttribute(string name, string description = "No description") {
		Name = name;
		Description = description;
	}
}