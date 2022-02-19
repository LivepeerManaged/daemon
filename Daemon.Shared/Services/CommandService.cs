using System.Reflection;
using Castle.Core.Internal;
using Daemon.Shared.Commands;

namespace Daemon.Shared.Services;

public class CommandService {
	public ReflectionsService ReflectionsService { get; set; }

	public void TriggerCommand(string name, dynamic parameters) {
		Type? findCommandTypeByName = GetCommandTypeByName(name);

		if (findCommandTypeByName == null)
			throw new Exception("Command not found [replace this with real exception!]");

		ICommand command = (Activator.CreateInstance(findCommandTypeByName) as ICommand)!;
		Dictionary<string, object> parameterDictionary = ReflectionsService.DynamicToDictionary(parameters);
		Dictionary<PropertyInfo, CommandParameterAttribute> propertiesWithAttributes = ReflectionsService.GetPropertiesWithAttributes<CommandParameterAttribute>(command.GetType());

		foreach ((string key, object value) in parameterDictionary.Where(parameterPair => propertiesWithAttributes.Any(valuePair => valuePair.Value.Name == parameterPair.Key))) {
			PropertyInfo propertyInfo = propertiesWithAttributes.First(pair => pair.Value.Name == key).Key;
			propertyInfo.SetValue(command, value);
		}

		command.onCommand();
	}

	public void TriggerCommand(ICommand command) {
		command.onCommand();
	}

	public CommandParameterAttribute[] GetCommandParameters<T>() where T : ICommand {
		return typeof(T).GetProperties().Select(p => ReflectionsService.GetAttributeOfProperty<CommandParameterAttribute>(p)).Where(a => a != null).ToArray()!;
	}

	public Type? GetCommandTypeByName(string name) {
		return ReflectionsService.GetAllImplementationsOf<ICommand>().Find(command => ReflectionsService.GetAttributeOfType<CommandAttribute>(command)?.Name == name);
	}
}