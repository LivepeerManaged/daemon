using NLog;

namespace Daemon.Shared.Basic;

/// <summary>
/// The Baseclass for every Class which needs a Logger
/// </summary>
public abstract class BaseClass {
	private NLog.Logger logger;

	protected Logger Logger {
		get {
			if (this.logger == null) {
				this.logger = NLog.LogManager.GetCurrentClassLogger();
			}

			return this.logger;
		}
	}
}