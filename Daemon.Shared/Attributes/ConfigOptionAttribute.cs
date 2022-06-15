namespace Daemon.Shared.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ConfigOptionAttribute : Attribute {
	public string Description { get; set; }

	public object? DefaultValue { get; set; }

	public bool Optional { get; set; }

	public float? _Min { get; set; }

	public float Min {
		get => _Min ?? 0;
		set => _Min = value;
	}

	public float? _Max { get; set; }

	public float Max {
		get => _Max ?? 0;
		set => _Max = value;
	}

	public bool MustBePositive { get; set; }

	public bool MustBeNegative { get; set; }
}