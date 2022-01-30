﻿using Daemon.Shared.Basic;

namespace Daemon.Shared.Communication;

public abstract class Event : BaseClass {
	protected Event() {
		this.Logger.Debug("Recieved {0} Event", this.GetType().Name);
	}
}