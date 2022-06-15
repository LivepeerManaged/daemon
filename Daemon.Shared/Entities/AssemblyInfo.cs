using System.Reflection;

namespace Daemon.Shared.Entities;

public class AssemblyInfo {
	public bool Enabled { get; set; }
	public Assembly Assembly { get; set; }
	public byte[] Hash { get; set; }
	public string Name { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string Version { get; set; }

	public override string ToString() {
		return $"AssemblyName: \"{Name}\", Title: \"{Title}\", Description: \"{Description}\", Version: \"{Version}\"";
	}
}