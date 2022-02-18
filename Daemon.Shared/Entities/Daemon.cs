using System.ComponentModel;

namespace TestPlugin;

public class Daemon {
	public string Id { get; set; }
	public string PublicKey { get; set; }
	public string DaemonSecret { get; set; }
	public string Label { get; set; }
	public DateTime CreatedAt { get; set; }
	public bool Activated { get; set; }
}