using Daemon.Shared.Commands;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;

namespace Daemon.Commands;

[Command("GetDaemonInfo", "Returns info about the Daemon")]
public class GetDaemonInfoCommand : ICommand {
	public IDaemonService DaemonService { get; set; }

	public object onCommand() {
		return new {
			Daemon = DaemonService.GetDaemonInfo(),
			System = DaemonService.GetSystemInfo(),
		};
	}
}