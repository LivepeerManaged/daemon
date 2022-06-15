using System.Dynamic;
using System.Reflection;
using Daemon.Shared.Attributes;
using Daemon.Shared.Commands;
using Daemon.Shared.Entities;
using Daemon.Shared.Exceptions;
using Daemon.Shared.Services;
using Newtonsoft.Json.Converters;
using TestPlugin.Config;

namespace Daemon.Commands;

[Command("GetPlugins", "Returns a list of all Plugins")]
public class GetPluginsCommand : ICommand {
	[CommandParameter("PluginName", "Name of a specific plugin", "", true)]
	private string PluginName { get; set; } = "";

	public IPluginService PluginService { get; set; }
	public IReflectionsService ReflectionsService { get; set; }
	public ICommandService CommandService { get; set; }
	public IStatusService StatusService { get; set; }
	public IConfigService ConfigService { get; set; }

	public object onCommand() {
		if (PluginName.Length > 0) {
			(DaemonPlugin? daemonPlugin, AssemblyInfo? info) = PluginService.GetPluginByName(PluginName);
			return GetPluginInfo(daemonPlugin, info);
		}

		List<dynamic> result = new();
		foreach ((DaemonPlugin? daemonPlugin, AssemblyInfo info) in PluginService.GetPlugins())
			result.Add(GetPluginInfo(daemonPlugin, info));

		return result;
	}

	private dynamic GetPluginInfo(DaemonPlugin? daemonPlugin, AssemblyInfo info) {
		AssemblyInfo assemblyInfo = ReflectionsService.GetAssemblyInfo(daemonPlugin.GetType().Assembly);
		Dictionary<CommandAttribute, CommandParameterAttribute[]> commandsFromPlugin = CommandService.GetCommandsFromPluginAssembly(daemonPlugin.GetType().Assembly);
		/*
		 * If anyone knows a cleaner solution please tell me.
		 * I did not want to create a class for all transfer objects.
		 * But maybe i will do because this is kinda ugly :(
		 */
		List<dynamic> commands = new();
		foreach ((CommandAttribute? command, CommandParameterAttribute[]? parameters) in commandsFromPlugin) {
			dynamic commandInfo = new ExpandoObject();
			commandInfo.Name = command.Name;
			commandInfo.Description = command.Description;
			commandInfo.Parameter = new List<dynamic>();
			foreach (CommandParameterAttribute commandParameterAttribute in parameters) {
				commandInfo.Parameter.Add(new {
					name = commandParameterAttribute.Name,
					description = commandParameterAttribute.Description,
					optional = commandParameterAttribute.Optional,
					defaultValue = commandParameterAttribute.DefaultValue,
				});
			}

			commands.Add(commandInfo);
		}

		Dictionary<string, Dictionary<string, dynamic>> configList = new Dictionary<string, Dictionary<string, dynamic>>();
		foreach ((Type configType, Dictionary<dynamic, Dictionary<PropertyInfo, ConfigOptionAttribute>> value) in ConfigService.GetPropertiesWithOptions(daemonPlugin.GetType())) {
			Dictionary<string, dynamic> configDictionary = new Dictionary<string, dynamic>();
			foreach ((dynamic _, Dictionary<PropertyInfo, ConfigOptionAttribute> attributes) in value) {
				foreach ((PropertyInfo? propertyInfo, ConfigOptionAttribute? attribute) in attributes) {
					Dictionary<string,object> options = new Dictionary<string, object>();
					
					// No Rider... this CAN absolutely be null. If you look you can actually the the little `?` behind the name so its NULLABLE
					if (attribute != null) {
						options.Add("Description", attribute.Description);
						options.Add("DefaultValue", attribute.DefaultValue);
						options.Add("Optional", attribute.Optional);
						options.Add("MustBePositive", attribute.MustBePositive);
						options.Add("MustBeNegative", attribute.MustBeNegative);
						
						if (attribute._Min.HasValue)
							options.Add("Min", attribute._Min.Value);
						
						if (attribute._Max.HasValue)
							options.Add("Max", attribute._Max.Value);
						
						configDictionary.Add(propertyInfo.Name, new {
							Type = propertyInfo.PropertyType.Name,
							Options = options
						});
					} else {
						configDictionary.Add(propertyInfo.Name, new {
							Type = propertyInfo.PropertyType.Name,
						});
					}
				}
				
			}
			configList.Add(ConfigService.GetConfigName(configType), configDictionary);
		}
		

		return new {
			name = assemblyInfo.Name,
			title = assemblyInfo.Title,
			description = assemblyInfo.Description,
			version = assemblyInfo.Version,
			enabled = info.Enabled,
			commands,
			status = StatusService.GetStatus(daemonPlugin.GetType()),
			config = configList
		};
	}
}