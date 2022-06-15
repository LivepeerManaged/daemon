using Daemon.Shared.Attributes;

namespace Daemon.Shared.Commands;

[Command("Test", "Test description yay")]
public class TestCommand : ICommand {
	[CommandParameter("FirstParameter", "Used to test stuff", "Default value for the first Parameter")]
	public string FirstParameter { get; set; }

	[CommandParameter("SecondParameterLol", "This is a awesome second parameter without any default value")]
	public string SecondParameter { get; set; }

	public object? onCommand() {
		Console.WriteLine($"FirstParameter LOLOL: {FirstParameter}, SecondParameter: {SecondParameter}");
		return FirstParameter;
	}
}