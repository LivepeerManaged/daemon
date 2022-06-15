using System.Reflection;
using Castle.Core.Internal;
using Daemon.Entities;
using Daemon.Shared.Entities;
using Daemon.Shared.Exceptions;
using Daemon.Shared.Services;
using Hardware.Info;

namespace Daemon.Services;

public class DaemonService : IDaemonService {
	private IConfigService ConfigService { get; set; }
	private IReflectionsService ReflectionsService { get; set; }
	
	private static readonly IHardwareInfo HardwareInfo = new HardwareInfo();

	public DaemonService() {
		new Task(() => Parallel.Invoke(() => HardwareInfo.RefreshCPUList(), () => HardwareInfo.RefreshMemoryStatus())).Start();
	}

	public Uri GetApiServer() {
		if (GetConfig().ApiServer.IsNullOrEmpty()) {
			throw new MissingApiServerException();
		}

		if (!Uri.TryCreate($"{(GetConfig().UseSsl ? "https://" : "http://")}{GetConfig().ApiServer}:{GetConfig().ApiServerPort}", UriKind.Absolute, out Uri? uri)) {
			throw new InvalidApiServerException(GetConfig().ApiServer);
		}

		return uri;
	}

	public Uri GetWebsocketServer() {
		return new Uri(GetApiServer(), "daemon");
	}

	private IDaemonConfig GetConfig() {
		return ConfigService.GetConfig<IDaemonConfig>();
	}

	public string GetSecret() {
		return GetConfig().DaemonSecret;
	}

	public string getId() {
		return GetConfig().DaemonId;
	}

	public DaemonInfo GetDaemonInfo() {
		AssemblyInfo assemblyInfo = ReflectionsService.GetAssemblyInfo(Assembly.GetEntryAssembly()!);
		
		return new DaemonInfo {
			CoreVersion = assemblyInfo.Version,
			UpdaterVersion = "Not yet existent",
		};
	}
	
	public DaemonSystemInformation GetSystemInfo() {
		return new DaemonSystemInformation {
			OS = GetOsInfo(),
			MachineName = Environment.MachineName,
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