using System.Reflection;
using System.Text.Json;
using Castle.Core.Internal;
using Daemon.Shared.Commands;
using Newtonsoft.Json;

namespace Daemon.Shared.Services;

public class CommandService {
	private ReflectionsService ReflectionsService { get; set; }

	public void TriggerCommand(string name, dynamic parameters) {
		Dictionary<string, object> parameterDictionary = ReflectionsService.DynamicToDictionary(parameters);
		TriggerCommand(name, parameterDictionary);
	}
	
	public void TriggerCommand(string name, Dictionary<string, JsonElement> parameters) {
		Type? findCommandTypeByName = GetCommandTypeByName(name);

		if (findCommandTypeByName == null)
			throw new Exception("Command not found [replace this with real exception!]");

		Console.WriteLine("PREE");
		ICommand command = BindCommandParameter((Activator.CreateInstance(findCommandTypeByName) as ICommand)!, parameters);
		Console.WriteLine("FOUND THINGY");
		command.onCommand();
	}

	// TODO Maybe replace this with Official Reflections Binder? 
	private ICommand BindCommandParameter(ICommand command, Dictionary<string, JsonElement> parameters) {
		Dictionary<PropertyInfo, CommandParameterAttribute> propertiesWithAttributes = ReflectionsService.GetPropertiesWithAttributes<CommandParameterAttribute>(command.GetType());

		foreach ((string key, JsonElement value) in parameters.Where(parameterPair => propertiesWithAttributes.Any(valuePair => valuePair.Value.Name == parameterPair.Key))) {
			PropertyInfo propertyInfo = propertiesWithAttributes.First(pair => pair.Value.Name == key).Key;
			propertyInfo.SetValue(command, value.Deserialize(propertyInfo.PropertyType));
		}

		return command;
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