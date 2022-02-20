using Config.Net;
using Daemon.Shared.Services;

namespace Daemon.Services;

public class ConfigService : IConfigService {
	public T GetConfig<T>(string? configName = null) where T : class {
		return new ConfigurationBuilder<T>().UseJsonFile($"{configName ?? typeof(T).Assembly.GetName().Name}.config").Build();
	}
}