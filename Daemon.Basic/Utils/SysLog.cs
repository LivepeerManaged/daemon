using log4net;

namespace Daemon.Basic.Utils;

public static class SysLog {
	private static ILog logger;

	public static void Init(ILog logger) {
		SysLog.logger = logger;
	}

	public static void LogDebug(string message, params object[] parameters) {
		SysLog.logger.DebugFormat(message, parameters);
	}

	public static void LogInfo(string message, params object[] parameters) {
		SysLog.logger.InfoFormat(message, parameters);
	}

	public static void LogWarn(string message, params object[] parameters) {
		SysLog.logger.WarnFormat(message, parameters);
	}

	public static void LogError(string message, params object[] parameters) {
		SysLog.logger.ErrorFormat(message, parameters);
	}

	public static void LogException(string message, Exception ex, params object[] parameters) {
		SysLog.logger.FatalFormat(message, ex, parameters);
	}
}