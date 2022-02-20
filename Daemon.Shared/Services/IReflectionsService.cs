using System.Reflection;
using Daemon.Shared.Entities;

namespace Daemon.Shared.Services;

public interface IReflectionsService {
	Type[] GetAllImplementationsOf<T>();
	Type[] GetTypesOfAssembly(Assembly assembly);
	Dictionary<string, object> DynamicToDictionary(dynamic dynamicObject);
	Type[] GetAllLoadedTypes();
	Assembly[] GetAllLoadedAssemblies();
	Type[] GetAllAttributedTypes<T>() where T : Attribute;
	TAttribute? GetAttributeOfType<TAttribute>(Type type) where TAttribute : Attribute;
	TAttribute? GetAttributeOfProperty<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute;
	Dictionary<PropertyInfo, TAttribute> GetPropertiesWithAttributes<TAttribute>(Type type) where TAttribute : Attribute;
	bool HasAttribute<TAttribute>(Type type) where TAttribute : Attribute;
	bool HasAttribute<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute;
	public AssemblyInfo GetAssemblyInfo(Assembly assembly);
}