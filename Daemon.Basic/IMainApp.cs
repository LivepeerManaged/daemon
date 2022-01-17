namespace Daemon.Basic;

public interface IMainApp {
	/// <summary>
	/// Starts the whole daemon
	/// </summary>
	void StartApp();

	/// <summary>
	/// Stops the whole daemon
	/// </summary>
	void StopApp();
}