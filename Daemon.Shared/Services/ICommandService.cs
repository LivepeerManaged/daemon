using System.Text.Json;
using Daemon.Shared.Commands;

namespace Daemon.Shared.Services;

public interface ICommandService {
	object? TriggerCommand(string name, Dictionary<string, JsonElement> parameters);
	object? TriggerCommand(ICommand command);
	CommandParameterAttribute[] GetCommandParameters<T>() where T : ICommand;
	Type? GetCommandTypeByName(string name);
}