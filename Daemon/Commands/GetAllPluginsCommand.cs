using System.Dynamic;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;

namespace Daemon.Shared.Commands;

[Command("GetAllPlugins", "Returns a list of all Plugins")]
public class GetAllPluginsCommand : ICommand {
	public IPluginService PluginService { get; set; }
	public IReflectionsService ReflectionsService { get; set; }
	public ICommandService CommandService { get; set; }
	public IStatusService StatusService { get; set; }
	public IConfigService ConfigService { get; set; }

	public object onCommand() {
		List<dynamic> result = new();
		foreach ((DaemonPlugin? daemonPlugin, PluginInfo info) in PluginService.GetPlugins()) {
			Entities.PluginInfo pluginInfo = ReflectionsService.GetAssemblyInfo(daemonPlugin.GetType().Assembly);
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

			result.Add(new {
				name = pluginInfo.Name,
				title = pluginInfo.Title,
				description = pluginInfo.Description,
				version = pluginInfo.Version,
				enabled = info.Enabled,
				commands,
				status = StatusService.GetStatus(daemonPlugin.GetType()),
				config = ConfigService.GetConfig(daemonPlugin.GetType())
			});
		}

		return result;
	}
}