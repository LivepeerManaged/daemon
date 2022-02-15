namespace Daemon.Shared.Communication;

public abstract class Event {
	protected Event() {
		//this.Logger.Debug("Recieved {0} Event", this.GetType().Name);
	}
}