namespace Daemon.Shared.Services;

public interface IConfigService {
	T GetConfig<T>(string? configName = null) where T : class;
}