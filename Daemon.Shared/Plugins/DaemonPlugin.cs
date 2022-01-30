using Autofac;
using Daemon.Shared.Basic;

namespace Daemon.Shared.Plugins;

/// <summary>
/// This is the class which is getting called by Core upon loading the Plugin
/// </summary>
public abstract class DaemonPlugin : BaseClass {
	/// <summary>
	/// This Method is getting called on loading the plugin
	/// </summary>
	public abstract void OnPluginLoad(IContainer container);

	/// <summary>
	/// This Method is getting called on unloading the plugin
	/// </summary>
	public abstract void OnPluginDisable();
}