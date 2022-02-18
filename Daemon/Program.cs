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
	public static async Task Main(string[] args) {
		CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());

		new MainApp(cancellationTokenSource).StartApp();

		cancellationTokenSource.Token.WaitHandle.WaitOne();
	}
}