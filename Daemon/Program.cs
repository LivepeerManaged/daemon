using NLog;

namespace Daemon;

/// <summary>
/// 
/// </summary>
public class Program : IDisposable {
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
		// this.Logger.Debug("Starting Daemon");

		try {
			MainApp mainApp = new MainApp();

			mainApp.StartApp();

			Console.ReadKey();

			mainApp.StopApp();
		} catch (Exception e) {
			// this.Logger.Fatal("During the execution of daemon an exception occured {0}", e);
		}
	}
}