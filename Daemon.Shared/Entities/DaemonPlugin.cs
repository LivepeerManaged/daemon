using Autofac;

namespace Daemon.Shared.Entities;

/// <summary>
///     This is the class which is getting called by Core upon loading the Plugin
/// </summary>
public abstract class DaemonPlugin {
	public abstract void RegisterServices(ContainerBuilder containerBuilder);
	public abstract Task OnPluginLoad(IContainer container);

	public abstract void OnPluginDisable();
}