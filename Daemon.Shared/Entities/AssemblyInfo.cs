namespace Daemon.Shared.Entities; 

public class AssemblyInfo {
	public string Name { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string Version { get; set; }

	public override string ToString() {
		return $"AssemblyName: \"{Name}\", Title: \"{Title}\", Description: \"{Description}\", Version: \"{Version}\"";
	}
}