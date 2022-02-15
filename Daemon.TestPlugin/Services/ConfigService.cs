using Autofac.Core.Activators;
using Config.Net;

namespace TestPlugin;

public class ConfigService {
    public T GetConfig<T>() where T : class {
        return new ConfigurationBuilder<T>().UseJsonFile($"{typeof(T).Assembly.GetName().Name}.config").Build();
    }
}