namespace Daemon.Shared.Commands; 

public interface ICommand {
	public object? onCommand();
}