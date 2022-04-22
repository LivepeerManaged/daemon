using Daemon.Shared.Commands;
using Daemon.Shared.Services;
using Hardware.Info;

namespace Daemon.Commands;

[Command("DaemonSystemInformation", "Returns a list of all commands")]
public class DaemonSystemInformation : ICommand {
	private static readonly IHardwareInfo HardwareInfo = new HardwareInfo();
	public ICommandService CommandService { get; set; }

	public object? onCommand() {
		Parallel.Invoke(() => HardwareInfo.RefreshCPUList(), () => HardwareInfo.RefreshMemoryStatus());
		return new {
			OS = GetOsInfo(),
			Environment.MachineName,
			Processor = HardwareInfo.CpuList.Select(cpu => cpu.Name).Aggregate((s1, s2) => $"{s1}, {s2}"),
			Memory = Math.Round(HardwareInfo.MemoryStatus.TotalPhysical / 1024d / 1024d / 1024d)
		};
	}

	private static string GetOsInfo() {
		OperatingSystem os = Environment.OSVersion;
		Version version = os.Version;
		switch (os.Platform) {
			case PlatformID.Win32NT:
				return version.Major switch {
					5 => version.Minor == 0 ? "Windows 2000" : "Windows XP",
					6 => version.Minor switch {
						0 => "Windows Vista",
						1 => "Windows 7",
						2 => "Windows 8",
						_ => "Windows 8.1"
					},
					10 => "Windows 10",
					_ => "?"
				};
			case PlatformID.Unix:
				return "Linux";
			case PlatformID.Other:
				return "Other"; // TODO add more checks
			default:
				return "?";
		}
	}
}