namespace Daemon.Shared.Attributes;

[AttributeUsage(AttributeTargets.Interface)]
public class ConfigNameAttribute : Attribute {
	public ConfigNameAttribute(string configName) {
		ConfigName = configName;
	}

	public string ConfigName { get; }
}