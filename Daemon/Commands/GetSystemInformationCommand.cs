using Daemon.Services;
using Daemon.Shared.Commands;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;

namespace Daemon.Commands;

[Command("GetSystemInformation", "Returns a list of all commands")]
public class GetSystemInformationCommand : ICommand {
	[CommandParameter("Plugin", "Used to test stuff", true)]
	public string Plugin { get; set; }

	public ICommandService CommandService { get; set; }

	public object? onCommand() {
		Dictionary<AssemblyInfo, Dictionary<CommandAttribute, CommandParameterAttribute[]>> allCommands = CommandService.GetAllCommands();
		foreach ((AssemblyInfo? key, Dictionary<CommandAttribute, CommandParameterAttribute[]>? value) in allCommands) {
			Console.WriteLine($"{key}");
			foreach ((CommandAttribute? commandAttribute, CommandParameterAttribute[]? commandParameterAttributes) in value) {
				Console.WriteLine($"\t[{commandAttribute.Name}]");
				foreach (CommandParameterAttribute commandParameterAttribute in commandParameterAttributes) {
					Console.WriteLine($"\t\t[{commandParameterAttribute.Name}] {commandParameterAttribute.Description}");
				}
			}
		}

		return allCommands.ToArray();
	}
}