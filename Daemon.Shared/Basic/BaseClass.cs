using NLog;

namespace Daemon.Shared.Basic;

/// <summary>
/// The Baseclass for every Class which needs a Logger
/// </summary>
public abstract class BaseClass {
	private NLog.Logger logger;

	/// <summary>
	/// The Logger for the current Class
	/// </summary>
	protected Logger Logger {
		get {
			if (this.logger == null) {
				this.logger = NLog.LogManager.GetLogger(GetType().FullName);
			}

			return this.logger;
		}
	}
}