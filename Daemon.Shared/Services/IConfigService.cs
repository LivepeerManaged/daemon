using System.Reflection;
using Daemon.Shared.Attributes;
using Daemon.Shared.Entities;

namespace Daemon.Shared.Services;

public interface IConfigService {
	string GetConfigName(Type type);
	T GetConfig<T>() where T : class, IPluginConfig;
	TT? GetConfig<T, TT>(string key) where T : class, IPluginConfig;
	Dictionary<string, IPluginConfig> GetConfig(Type type);
	Dictionary<Type, Dictionary<dynamic, Dictionary<PropertyInfo, ConfigOptionAttribute>>> GetPropertiesWithOptions(Type type);
	void SetConfig<T>(string key, object value) where T : class, IPluginConfig;
}