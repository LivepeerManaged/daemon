namespace Daemon.Shared.Entities;

public interface IDaemonConfig: IPluginConfig {
	public string DaemonId { get; set; }

	public bool UseSsl { get; set; }

	public string ApiServer { get; set; }

	public int ApiServerPort { get; set; }

	public string DaemonSecret { get; set; }
	
	public string Test { get; set; }
}