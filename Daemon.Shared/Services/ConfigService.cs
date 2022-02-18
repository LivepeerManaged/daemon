﻿using Autofac.Core.Activators;
using Config.Net;

namespace TestPlugin;

public class ConfigService {
	public T GetConfig<T>(string? configName = null) where T : class {
		return new ConfigurationBuilder<T>().UseJsonFile($"{configName ?? typeof(T).Assembly.GetName().Name}.config").Build();
	}
}