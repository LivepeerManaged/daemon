namespace Daemon.CommandsDto; 

public class ConfigDto {
	public string Name { get; set; }
	public List<ConfigPropertyDto> Properties { get; set; }
}