using Daemon.Services;
using Daemon.Shared.Commands;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;

namespace Daemon;

[Command("GetAllCommands", "Returns a list of all commands")]
public class GetAllCommandsCommand : ICommand {
	[CommandParameter("Plugin", "Used to test stuff", true)]
	public string Plugin { get; set; }

	public ICommandService CommandService { get; set; }

	public object? onCommand() {
		return CommandService.GetAllCommands().ToArray();
	}
}