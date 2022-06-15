namespace Daemon.Shared.Exceptions;

public class CommandNotFoundException : Exception {
	public CommandNotFoundException(string command) : base($"Command \"{command}\" not found!") {
	}
}