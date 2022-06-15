using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;

namespace Daemon.Services;

public class ReflectionsService : IReflectionsService {
	public Type[] GetAllImplementationsOf<T>() {
		return GetAllLoadedTypes().Where(type => typeof(T).IsAssignableFrom(type) && type != typeof(T)).ToArray();
	}

	public Type[] GetAllImplementationsInAssemblyOf<T>(Assembly assembly) {
		return GetAllLoadedTypes().Where(type => typeof(T).IsAssignableFrom(type) && type != typeof(T) && type.Assembly == assembly).ToArray();
	}

	public Type[] GetTypesOfAssembly(Assembly assembly) {
		return assembly.GetTypes();
	}

	public Dictionary<string, object> DynamicToDictionary(dynamic dynamicObject) {
		Dictionary<string, object> dictionary = new();

		foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dynamicObject)) {
			dictionary.Add(propertyDescriptor.Name, propertyDescriptor.GetValue(dynamicObject));
		}

		return dictionary;
	}

	public Type[] GetAllLoadedTypes() {
		return GetAllLoadedAssemblies().SelectMany(assembly => assembly.GetTypes()).ToArray();
	}

	public Assembly[] GetAllLoadedAssemblies() {
		return AppDomain.CurrentDomain.GetAssemblies();
	}

	public Type[] GetAllAttributedTypes<T>() where T : Attribute {
		return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes().Where(t => t.GetCustomAttribute<T>() != null)).ToArray();
	}

	public TAttribute? GetAttributeOfType<TAttribute>(Type type) where TAttribute : Attribute {
		return type.GetCustomAttribute<TAttribute>();
	}

	public TAttribute? GetAttributeOfProperty<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute {
		return propertyInfo.GetCustomAttribute<TAttribute>();
	}

	public Dictionary<PropertyInfo, TAttribute> GetPropertiesWithAttributes<TAttribute>(Type type) where TAttribute : Attribute {
		Dictionary<PropertyInfo, TAttribute> dictionary = new();

		foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic)) {
			TAttribute? customAttribute = propertyInfo.GetCustomAttribute<TAttribute>();
			if (customAttribute != null) {
				dictionary.Add(propertyInfo, customAttribute);
			}
		}

		return dictionary;
	}

/*
  	public bool TryGetAttributeOfType<TAttribute>(Object obj, out TAttribute? attribute) where TAttribute : Attribute {
		attribute = GetAttributeOfType<TAttribute>(obj);
		return attribute != null;
	}
 */

	public bool HasAttribute<TAttribute>(Type type) where TAttribute : Attribute {
		return GetAttributeOfType<TAttribute>(type) != null;
	}


	public bool HasAttribute<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute {
		return GetAttributeOfProperty<TAttribute>(propertyInfo) != null;
	}

	public AssemblyInfo GetAssemblyInfo(Assembly assembly) {
		return new AssemblyInfo {
			Assembly = assembly,
			Hash = GetHash(assembly.Location),
			Name = assembly.GetName().Name,
			Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title,
			Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description,
			Version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion, // Why the fuck is it this attribute and not AssemblyVersionAttribute???
		};
	}
	
	private byte[] GetHash(string file) {
		using MD5 md5 = MD5.Create();
		using FileStream stream = File.OpenRead(file);
		return md5.ComputeHash(stream);
	}
}