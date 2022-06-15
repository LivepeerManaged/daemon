using System.Reflection;
using System.Text.Json;
using Daemon.Shared.Attributes;
using Daemon.Shared.Commands;
using Daemon.Shared.Entities;

namespace Daemon.Shared.Services;

public interface ICommandService {
	object? TriggerCommand(string name, Dictionary<string, JsonElement> parameters);
	object? TriggerCommand(ICommand command);
	public CommandAttribute? GetCommandAttribute(Type type);
	public Dictionary<CommandAttribute, CommandParameterAttribute[]> GetCommandsFromPluginAssembly(Assembly assembly);
	CommandParameterAttribute[] GetCommandParameters(Type type);
	KeyValuePair<CommandAttribute, CommandParameterAttribute[]> GetCommandInfoForType(Type type);
	Type? GetCommandTypeByName(string name);
	Dictionary<AssemblyInfo, Dictionary<CommandAttribute, CommandParameterAttribute[]>> GetAllCommands();
}