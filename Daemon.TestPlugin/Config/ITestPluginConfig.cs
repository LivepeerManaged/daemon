using Config.Net;
using Daemon.Shared.Attributes;
using Daemon.Shared.Entities;

namespace TestPlugin.Config;

[ConfigName("TestConfig")]
public interface ITestPluginConfig : IPluginConfig {
	[ConfigOption(Description = "Eth Address", Optional = false)]
	public string Address { get; set; }
	
	[ConfigOption(Description = "Orchestrator IP Address", Optional = false)]
	public string OrchAddr { get; set; }

	[ConfigOption(Description = "Orchestrator Secret", Optional = false)]
	public string OrchSecret { get; set; }
	
	[ConfigOption(Description = "Max Sessions", Optional = false, Min = 0)]
	public int MaxSessions { get; set; }
	
	[ConfigOption(Description = "Seperated ',' list of gpus to use (or 'all')", DefaultValue = "0")]
	public string Nvidia { get; set; }

	[ConfigOption(Description = "Enable Monitoring", DefaultValue = false)]
	public bool Monitoring { get; set; }
}