using Autofac;
using Daemon.Shared.Entities;

namespace Daemon.Shared.Services;

public interface IPluginService {
	Dictionary<DaemonPlugin, PluginInfo> GetPlugins();

	/// <summary>
	///     This methods loads the plugins out of the plugins folder.
	/// </summary>
	IContainer LoadPlugins();

	/// <summary>
	///     This methods
	/// </summary>
	void UnloadPlugins();

	public DaemonPlugin GetPluginByName(string name);
}