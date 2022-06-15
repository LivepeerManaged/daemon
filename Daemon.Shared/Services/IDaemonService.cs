using Daemon.Entities;
using Daemon.Shared.Entities;

namespace Daemon.Shared.Services;

public interface IDaemonService {
	Uri GetApiServer();
	Uri GetWebsocketServer();
	string GetSecret();
	string getId();
	DaemonInfo GetDaemonInfo();
	DaemonSystemInformation GetSystemInfo();
}