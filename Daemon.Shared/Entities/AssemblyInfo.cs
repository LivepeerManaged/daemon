namespace Daemon.Shared.Entities; 

public class AssemblyInfo {
	public string AssemblyName { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string Version { get; set; }

	public override string ToString() {
		return $"AssemblyName: \"{AssemblyName}\", Title: \"{Title}\", Description: \"{Description}\", Version: \"{Version}\"";
	}
}