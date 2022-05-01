using Daemon.Shared.Entities;

namespace Daemon.Shared.Services;

public interface IStatusService {
	public Dictionary<string, string> GetStatus(Type type);
	public Dictionary<string, string> GetStatus<T>() where T : DaemonPlugin;
	public string GetStatus<T>(string key) where T : DaemonPlugin;
	public string GetStatus(Type type, string key);

	public void SetStatus<T>(string key, string value) where T : DaemonPlugin;
}