using Daemon.Basic.Utils;

namespace Daemon.Updater;

/// <summary>
/// This class has t
/// </summary>
public class Updater {
	private const string UPDATER_URL = "";

	public Updater() {
	}

	public bool IsUpdateCompatible() {
		SysLog.LogDebug("Check if the Update is compatible");

		SysLog.LogDebug("Checked if the update ist compatible: ");
		return true;
	}

	public bool HasUpdates() {
		bool hasUpdate = false;

		SysLog.LogDebug("Start search for Updates");

		SysLog.LogDebug($"Search completed: {hasUpdate}");

		return hasUpdate;
	}

	public bool ExecuteUpdate() {
		SysLog.LogDebug("Start Updating");

		SysLog.LogDebug("Successfully ended the Update");
		return true;
	}
}