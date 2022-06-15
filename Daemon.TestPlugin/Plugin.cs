using Autofac;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;

namespace TestPlugin;

public class Plugin : DaemonPlugin {
	public override void RegisterServices(ContainerBuilder containerBuilder) {
	}

	public override void OnPluginLoad(IContainer container) {
		IStatusService statusService = container.Resolve<IStatusService>();
		statusService.SetStatus<Plugin>("Test", "LOL");
	}

	public override void OnPluginDisable() {
	}
}