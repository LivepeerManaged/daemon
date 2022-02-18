﻿using System.Runtime.CompilerServices;

namespace Daemon.Shared.Communication.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EventNameAttribute : Attribute {
	public string? EventName { get; }

	public EventNameAttribute(string eventName) {
		EventName = eventName;
	}
}