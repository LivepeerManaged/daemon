namespace Daemon.Shared.Exceptions;

public class MissingApiServerException : Exception {
	public MissingApiServerException() : base("ApiServer is missing in Config!") {
	}
}