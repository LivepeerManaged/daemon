using System.Reflection;
using System.Runtime.Loader;
using Daemon.Basic;
using Daemon.Basic.Utils;
using log4net;

namespace Daemon;

public static class Program {
	/// <summary>
	/// Entrypoint of the application
	/// </summary>
	/// <param name="args"></param>
	/// <exception cref="InvalidOperationException"></exception>
	public static void Main(string[] args) {
		// Tell the Logger who to find the config
		log4net.Config.XmlConfigurator.Configure(new Uri(Path.Combine(Environment.CurrentDirectory, "log4net.config")));

		// Inject the Logger into the custom SysLog
		SysLog.Init(LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType));

		SysLog.LogDebug("Load the Application");

		SysLog.LogDebug("Check for Updates");

		var updater = new Updater.Updater();

		if (updater.HasUpdates()) {
			if (!updater.IsUpdateCompatible()) {
				// TODO: Tell the user that a manual update is required!
				return;
			} else if (!updater.ExecuteUpdate()) {
				// TODO: Tell the user that the update couldn't proced correctly
				return;
			}
		}

		SysLog.LogDebug("Trying to load the core");

		Assembly sharedAssembly =
			AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(Environment.CurrentDirectory, "Daemon.Shared\\Daemon.Shared.dll"));
		Assembly assembly =
			AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(Environment.CurrentDirectory, "Daemon.Core\\Daemon.Core.dll"));
		Type mainClass = assembly.GetType("Daemon.Core.MainApp") ?? throw new InvalidOperationException();

		SysLog.LogDebug("Successfully loaded the core");

		SysLog.LogDebug("Trying to initiate the Daemon");

		if (Activator.CreateInstance(mainClass) is not IMainApp mainApp) {
			return;
		}

		SysLog.LogDebug("Successfully initiated the Daemon");

		SysLog.LogDebug("Starting the Daemon");

		try {
			mainApp.StartApp();

			Console.ReadKey();

			mainApp.StopApp();
		} catch (Exception e) {
			SysLog.LogException("During the execution of the daemon an exception occured", e);
		}
	}
}