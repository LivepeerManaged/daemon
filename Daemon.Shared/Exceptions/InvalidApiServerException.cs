namespace Daemon.Shared.Exceptions;

public class InvalidApiServerException : Exception {
	public InvalidApiServerException(string server) : base($"Invalid ApiServer in Config! \"{server}\"") {
	}
}