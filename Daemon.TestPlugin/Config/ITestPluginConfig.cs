using Config.Net;
using Daemon.Shared.Attributes;
using Daemon.Shared.Entities;

namespace TestPlugin.Config;

[ConfigName("TestConfig")]
public interface ITestPluginConfig : IPluginConfig {
	[ConfigOption(Description = "Testing this boolean")]
	public bool TestBoolean { get; set; }

	[ConfigOption(Description = "Testing", Optional = true, DefaultValue = "DEFAULT IS THIS")]
	public string TestString { get; set; }

	[ConfigOption(Description = "Testing Int")]
	public int TestInt { get; set; }

	[ConfigOption(Description = "Testing Double", Min = 100, Max = 100)]
	public double TestDouble { get; set; }

	public float TestFloat { get; set; }
}