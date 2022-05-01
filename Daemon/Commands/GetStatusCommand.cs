using Daemon.Shared.Commands;
using Daemon.Shared.Services;

namespace Daemon.Commands;

[Command("GetStatus", "Returns the current Status of the Daemon")]
public class GetStatusCommand : ICommand {
	[CommandParameter("PluginName", "Get status for specified plugin")]
	public string PluginName { get; set; }

	public IStatusService StatusService { get; set; }

	public IPluginService PluginService { get; set; }

	public object onCommand() {
		return StatusService.GetStatus(PluginService.GetPluginByName(PluginName).GetType());
	}
}