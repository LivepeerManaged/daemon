using Daemon.Shared.Basic;
using NLog;

namespace Daemon;

/// <summary>
/// 
/// </summary>
public class Program : BaseClass, IDisposable {
	/// <summary>
	/// What should happen on dispose.
	/// </summary>
	public void Dispose() {
		LogManager.Flush();
		LogManager.Shutdown();
	}

	/// <summary>
	/// Entrypoint of the application
	/// </summary>
	/// <param name="args"></param>
	/// <exception cref="InvalidOperationException"></exception>
	public static void Main(string[] args) {
		new Program().startApp();
	}

	/// <summary>
	/// Loads the Daemon and execute it.
	/// </summary>
	private void startApp() {
		this.Logger.Info("Load the Application");

		this.Logger.Debug("Starting the Daemon");

		try {
			MainApp mainApp = new MainApp();

			mainApp.StartApp();

			Console.ReadKey();

			mainApp.StopApp();
		} catch (Exception e) {
			this.Logger.Fatal("During the execution of the daemon an exception occured", e);
		}
	}
}