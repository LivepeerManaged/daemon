using Daemon.Shared.Entities;

namespace Daemon.Shared.Services;

public interface IConfigService {
	public T GetConfig<T>() where T : class, IPluginConfig;
	public TT? GetConfig<T, TT>(string key) where T : class, IPluginConfig;
	public Dictionary<string, IPluginConfig> GetConfig(Type type);
	public void SetConfig<T>(string key, object value) where T: class, IPluginConfig;
}