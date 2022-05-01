using System.Reflection;
using Daemon.Shared.Entities;
using Daemon.Shared.Services;

namespace Daemon.Services; 

public class StatusService: IStatusService {
	private Dictionary<Assembly, Dictionary<string, string>> Status { get; set; } = new Dictionary<Assembly, Dictionary<string, string>>();

	public Dictionary<string, string> GetStatus(Type type) {
		if (!Status.ContainsKey(type.Assembly))
			Status[type.Assembly] = new Dictionary<string, string>();
		return Status[type.Assembly];
	}

	public Dictionary<string, string> GetStatus<T>() where T: DaemonPlugin {
		return GetStatus(typeof(T));
	}

	public string GetStatus<T>(string key) where T: DaemonPlugin {
		return GetStatus<T>().First(pair => pair.Key == key).Value;
	}
	public string GetStatus(Type type, string key) {
		return GetStatus(type).First(pair => pair.Key == key).Value;
	}
	
	public void SetStatus<T>(string key, string value) where T: DaemonPlugin {
		GetStatus<T>()[key] = value;
	}
}