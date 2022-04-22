using System.Dynamic;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;

namespace Daemon.Shared.Commands;

[Command("GetAllPlugins", "Returns a list of all Plugins")]
public class GetAllPluginsCommand : ICommand {
	public IPluginService PluginService { get; set; }
	public IReflectionsService ReflectionsService { get; set; }
	public ICommandService CommandService { get; set; }

	public object onCommand() {
		List<dynamic> result = new();
		foreach ((DaemonPlugin? daemonPlugin, bool enabled) in PluginService.GetPlugins()) {
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
						commandParameterAttribute.Name,
						commandParameterAttribute.Description,
						commandParameterAttribute.Optional,
						commandParameterAttribute.DefaultValue
					});
				}

				commands.Add(commandInfo);
			}

			result.Add(new {
				assemblyInfo.Name,
				assemblyInfo.Title,
				assemblyInfo.Description,
				assemblyInfo.Version,
				Enabled = enabled,
				Commands = commands
			});
		}

		return result;
	}

	private class PluginInfo {
		public AssemblyInfo AssemblyInfo { get; set; }
	}
}