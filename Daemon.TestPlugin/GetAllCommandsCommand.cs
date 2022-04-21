using Daemon.Shared.Commands;
using Daemon.Shared.Services;

namespace Daemon.Commands;

[Command("GetAllCommands", "Returns a list of all commands")]
public class GetAllCommandsCommand : ICommand {
	public ICommandService CommandService { get; set; }

	public object? onCommand() {
		return CommandService.GetAllCommands().ToArray();
	}
}