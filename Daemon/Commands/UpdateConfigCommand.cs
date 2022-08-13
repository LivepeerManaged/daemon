using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using Daemon.CommandsDto;
using Daemon.Shared.Attributes;
using Daemon.Shared.Commands;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;
using Newtonsoft.Json;

namespace Daemon.Commands;

[Command("UpdateConfig", "Updates configuration")]
public class UpdateConfigCommand : ICommand {
	[CommandParameter("PluginName", "Name of the plugin to change the config")]
	private string PluginName { get; set; }
	
	[CommandParameter("Config", "config")]
	private ConfigDto Config { get; set; }

	public IPluginService PluginService { get; set; }
	public IConfigService ConfigService { get; set; }

	public object onCommand() {
		(string? configName, IPluginConfig? pluginConfig) = ConfigService.GetConfig(PluginService.GetPluginByName(PluginName).Key.GetType()).Single(config => config.Key.Equals(Config.Name));

		foreach (PropertyInfo info in pluginConfig.GetType().GetProperties()) {
			ConfigPropertyDto? configPropertyDto = Config.Properties.Find(prop => prop.Name.Equals(info.Name, StringComparison.OrdinalIgnoreCase));
			
			if (configPropertyDto == null) {
				Console.WriteLine("Skipping unknown property \"" + info.Name + "\"...");
				continue;
			}

			if (configPropertyDto.Value == null) {
				info.SetValue(pluginConfig, configPropertyDto.DefaultValue);
				continue;
			}
			
			JsonElement jsonElement = (JsonElement) configPropertyDto.Value;
			info.SetValue(pluginConfig, jsonElement.Deserialize(info.PropertyType));
		}
		
		return null;
	}
}