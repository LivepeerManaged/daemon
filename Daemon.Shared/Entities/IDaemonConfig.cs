namespace Daemon.Shared.Entities;

public interface IDaemonConfig {
	string DaemonId { get; set; }

	bool UseSsl { get; set; }

	string ApiServer { get; set; }

	int ApiServerPort { get; set; }

	string DaemonSecret { get; set; }
}