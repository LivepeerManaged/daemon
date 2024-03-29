﻿namespace Daemon.Shared.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CommandParameterAttribute : Attribute {
	public CommandParameterAttribute(string name, string description = "No description", object? defaultValue = null, bool optional = false) {
		Name = name;
		Optional = optional;
		Description = description;
		DefaultValue = defaultValue;
	}

	public string Name { get; }
	public string Description { get; }
	public bool Optional { get; }
	public object? DefaultValue { get; }
}