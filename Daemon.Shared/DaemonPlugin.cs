namespace Daemon.Shared;

/// <summary>
/// This is the class which is getting called by Core upon loading the Plugin
/// </summary>
public abstract class DaemonPlugin {
	#region Public Methods

	#region public abstract void OnPluginLoad()

	/// <summary>
	/// This Method is getting called on loading the plugin
	/// </summary>
	public abstract void OnPluginLoad();

	#endregion

	#region public abstract void OnPluginDisable()

	/// <summary>
	/// This Method is getting called on unloading the plugin
	/// </summary>
	public abstract void OnPluginDisable();

	#endregion

	#endregion
}