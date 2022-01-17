using log4net;

namespace Daemon.Shared;

/// <summary>
/// This is the class which is getting called by Core upon loading the Plugin
/// </summary>
public abstract class DaemonPlugin {
	#region Instanzvariablen

	private ILog? sysLog;

	#endregion

	#region Protected Properties

	#region protected ILog Logger

	/// <summary>
	/// Gets the Logger of the specified Plugin
	/// </summary>
	protected ILog Logger {
		get {
			return this.sysLog ??= LogManager.GetLogger(this.GetType().FullName);
		}
	}

	#endregion

	#endregion

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