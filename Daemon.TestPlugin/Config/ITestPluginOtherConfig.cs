using Daemon.Shared.Attributes;
using Daemon.Shared.Entities;

namespace TestPlugin.Config; 
[ConfigName("OtherConfig")]
public interface ITestPluginOtherConfig: IPluginConfig {
	public string OtherValuesYay { get; set; }
}