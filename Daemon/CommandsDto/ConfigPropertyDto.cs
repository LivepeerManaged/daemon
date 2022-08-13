namespace Daemon.CommandsDto; 

public class ConfigPropertyDto {
	public string Name { get; set; }
	public string Type { get; set; }
	public object? Value { get; set; }
	public string Description { get; set; }
	public object DefaultValue { get; set; }
	public bool Optional { get; set; }
	public float? Min { get; set; }
	public float? Max { get; set; }
	public float? Step { get; set; }
	public bool MustBePositive { get; set; }
	public bool MustBeNegative { get; set; }
}