using Daemon.Shared.Commands;
using Daemon.Shared.Services;

namespace Daemon.Commands;

[Command("GetLoadedPlugins", "Returns a list of loaded Plugins")]
public class GetLoadedPluginsCommand : ICommand {
	public IPluginService PluginService { get; set; }
	public IReflectionsService ReflectionsService { get; set; }

	public object? onCommand() {
		return PluginService.GetPlugins().Select(plugin => ReflectionsService.GetAssemblyInfo(plugin.GetType().Assembly)).ToArray();
	}
}