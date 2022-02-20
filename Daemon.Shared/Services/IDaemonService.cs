using Daemon.Shared.Entities;

namespace Daemon.Shared.Services;

public interface IDaemonService {
	Uri GetApiServer();
	Uri GetWebsocketServer();
	IDaemonConfig GetConfig();
	string GetSecret();
	string getId();
}