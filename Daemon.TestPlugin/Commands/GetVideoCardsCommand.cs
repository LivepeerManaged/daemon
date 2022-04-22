using Daemon.Shared.Commands;
using Hardware.Info;

namespace Daemon.Commands;

[Command("GetVideoCards", "Returns a list of all commands")]
public class GetVideoCardsCommand : ICommand {
	private static readonly IHardwareInfo HardwareInfo = new HardwareInfo();

	public object? onCommand() {
		HardwareInfo.RefreshVideoControllerList();
		return HardwareInfo.VideoControllerList;
	}
}