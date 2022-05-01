using System.CodeDom;
using System.Reflection;
using Castle.Core.Internal;
using Config.Net;
using Config.Net.Json.Stores;
using Daemon.Shared.Attributes;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;
using TestPlugin;

namespace Daemon.Services;

public class ConfigService: IConfigService {
	private IReflectionsService ReflectionsService { get; set; }
	public T GetConfig<T>() where T: class, IPluginConfig {
		// We recall this everytime so we can have "live config editing"
		return new ConfigurationBuilder<T>().UseJsonFile($"{typeof(T).Assembly.GetName().Name}.config").Build();
	}
	
	public TT? GetConfig<T, TT>(string key) where T: class, IPluginConfig {
		object? value = typeof(T).GetProperty(key)?.GetValue(GetConfig<T>());
		if (value == null)
			throw new Exception("Config key not found!");
		return (TT) value;
	}

	private Type[] GetConfigClassesForPlugin(Type type) {
		return ReflectionsService.GetAllImplementationsInAssemblyOf<IPluginConfig>(type.Assembly);
	}

	public Dictionary<string, IPluginConfig> GetConfig(Type type) {
		Dictionary<string, IPluginConfig> pluginConfigs = new Dictionary<string, IPluginConfig>();
		Type[] types = GetConfigClassesForPlugin(type);
		foreach (Type configType in types) {
			/*
			 * Reflection for instancing Config.Net ConfigurationBuilder with type generic.
			 * Its hacky but it works well :)
			 * If you have a better solution please let me know!
			 */
			Type myParameterizedSomeClass = typeof(IConfigStore).Assembly.GetTypes().Find(t => t.Name.Equals("ConfigurationBuilder`1")).MakeGenericType(configType);
			dynamic instance = Activator.CreateInstance(myParameterizedSomeClass) ?? throw new InvalidOperationException();
			ConfigNameAttribute? configNameAttribute = ReflectionsService.GetAttributeOfType<ConfigNameAttribute>(configType);
			pluginConfigs.Add(configNameAttribute != null ? configNameAttribute.ConfigName : "Plugin", instance.UseConfigStore(new JsonFileConfigStore($"{configType.Assembly.GetName().Name}.config", true)).Build());
		}
		GetConfig<ITestPluginOtherConfig>().OtherValuesYay = "BAMBUS";
		return pluginConfigs;
	}

	public void SetConfig<T>(string key, object value) where T: class, IPluginConfig {
		try {
			T config = GetConfig<T>();
			typeof(T).GetProperty(key)?.SetValue(config, value);
			//  Config.NET JSON saves automatically so no need to save here
		} catch (Exception e) {
			Console.WriteLine(e);
			throw new Exception("Config key not found!");
		}
	}
}