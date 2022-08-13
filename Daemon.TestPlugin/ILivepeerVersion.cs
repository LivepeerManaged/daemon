using Config.Net;

namespace TestPlugin;

public interface ILivepeerVersion {
	[Option(DefaultValue = -1)]
	public int Id { get; set; }

	public string Name { get; set; }
	
	public Uri DownloadUri { get; set; }
	
	public DateTime LastCheckedUtc { get; set; }
}