namespace Daemon.Shared.Services;

public interface IApiServerService {
	Task<string> DaemonLogin(string id, string secret);
}