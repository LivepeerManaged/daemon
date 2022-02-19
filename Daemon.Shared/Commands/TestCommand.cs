namespace Daemon.Shared.Commands; 

[Command("Test", "Test description yay")]
public class TestCommand: ICommand {
	[CommandParameter("FirstParameter", "Used to test stuff")]
	public string FirstParameter { get; set; }
	
	public string SecondParameter { get; set; }
	
	public void onCommand() {
		Console.WriteLine("Awesome implementation!");
		Console.WriteLine($"FirstParameter: {FirstParameter}, SecondParameter: {SecondParameter}");
	}
}