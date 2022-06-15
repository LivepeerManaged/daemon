namespace Daemon.Shared.Exceptions;

public class PluginNotFoundException : Exception {
	public PluginNotFoundException(string plugin) : base($"Plugin \"{plugin}\" not found!") {
	}
}