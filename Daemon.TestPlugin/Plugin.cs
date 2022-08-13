using Autofac;
using Autofac.Core.NonPublicProperty;
using Daemon.Shared.Entities;
using TestPlugin.Service;

namespace TestPlugin;

public class Plugin : DaemonPlugin {
	public override void RegisterServices(ContainerBuilder containerBuilder) {
		containerBuilder.RegisterType<UpdaterService>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AutoWireNonPublicProperties();
	}

	public override async Task OnPluginLoad(IContainer container) {
		UpdaterService updaterService = container.Resolve<UpdaterService>();
		await updaterService.EnsureGoLivepeer();
	}

	public override void OnPluginDisable() {
	}
}