﻿using Autofac;
using Daemon.Shared.Entities;

namespace TestPlugin;

public class Plugin : DaemonPlugin {
	public override void RegisterServices(ContainerBuilder containerBuilder) {
	}

	public override void OnPluginLoad(IContainer container) {
	}

	public override void OnPluginDisable() {
	}
}