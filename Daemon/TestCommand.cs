namespace Daemon.Shared.Commands;

[Command("Test", "Test description yay")]
public class TestCommand : ICommand {
	[CommandParameter("FirstParameter", "Used to test stuff")]
	public string FirstParameter { get; set; }

	public string SecondParameter { get; set; }

	public object? onCommand() {
		Console.WriteLine($"FirstParameter: {FirstParameter}, SecondParameter: {SecondParameter}");
		return null;
	}
}