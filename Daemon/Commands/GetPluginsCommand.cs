using System.Dynamic;
using System.Reflection;
using Daemon.CommandsDto;
using Daemon.Shared.Attributes;
using Daemon.Shared.Commands;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;

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

		//(Type? _, Dictionary<dynamic, Dictionary<PropertyInfo, ConfigOptionAttribute>>? propertyOptions) = ConfigService.GetPropertiesWithOptions(daemonPlugin.GetType()).First();
		List<ConfigDto> configList = new List<ConfigDto>();

		foreach ((Type configType, Dictionary<dynamic, Dictionary<PropertyInfo, ConfigOptionAttribute>> propertyOptions) in ConfigService.GetPropertiesWithOptions(daemonPlugin.GetType())) {
			List<ConfigPropertyDto> configProperties = new List<ConfigPropertyDto>();
			foreach ((dynamic configInstance, Dictionary<PropertyInfo, ConfigOptionAttribute> attributes) in propertyOptions) {
				foreach ((PropertyInfo? propertyInfo, ConfigOptionAttribute? attribute) in attributes) {
					ConfigPropertyDto configPropertyDto = new ConfigPropertyDto {
						Name = propertyInfo.Name,
						Value = propertyInfo.GetValue(configInstance) ?? attribute.DefaultValue,
						Description = attribute?.Description ?? "",
						DefaultValue = attribute?.DefaultValue ?? null,
						Optional = attribute?.Optional == false
					};

					if (propertyInfo.PropertyType == typeof(string)) {
						configPropertyDto.Type = "string";
					} else if (propertyInfo.PropertyType == typeof(bool)) {
						configPropertyDto.Type = "boolean";
						Console.WriteLine(configPropertyDto.Value != null);
						Console.WriteLine(configPropertyDto.Value);
					} else if (propertyInfo.PropertyType == typeof(int)) {
						configPropertyDto.Type = "integer";
					} else if (propertyInfo.PropertyType == typeof(double)) {
						configPropertyDto.Type = "double";
					} else if (propertyInfo.PropertyType == typeof(float)) {
						configPropertyDto.Type = "float";
					} else if (propertyInfo.PropertyType == typeof(long)) {
						configPropertyDto.Type = "long";
					} else {
						Console.WriteLine("Warning! " + propertyInfo.PropertyType + " IS NOT ADDED!");
					}

					if (attribute != null) {
						configPropertyDto.Min = attribute._Min.HasValue ? attribute._Min.Value : null;
						configPropertyDto.Max = attribute._Max.HasValue ? attribute._Max.Value : null;
						configPropertyDto.Step = attribute._Step.HasValue ? attribute._Step.Value : null;
					}


					configProperties.Add(configPropertyDto);
				}
				configList.Add(new ConfigDto {
					Name = ConfigService.GetConfigName(configType),
					Properties = configProperties
				});
			}
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