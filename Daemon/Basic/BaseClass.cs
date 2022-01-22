using NLog;

namespace Daemon.Basic;

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