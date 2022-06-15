using Daemon.Shared.Commands;
using Hardware.Info;

namespace TestPlugin.Commands;

[Command("GetVideoCards", "Returns a list of all GPUS")]
public class GetVideoCardsCommand : ICommand {
	private static readonly IHardwareInfo HardwareInfo = new HardwareInfo();

	public object? onCommand() {
		HardwareInfo.RefreshVideoControllerList();
		return HardwareInfo.VideoControllerList;
	}
}