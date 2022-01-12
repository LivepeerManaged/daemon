using System.Reflection;
using Daemon.Basic;

namespace Daemon;

public static class Program {
	public static void Main(string[] args) {
		var updater = new Updater.Updater();

		if (updater.HasUpdates()) {
			if (!updater.IsUpdateCompatible()) {
				// TODO: Tell the user that a manual update is required!
				return;
			} else if (!updater.ExecuteUpdate()) {
				// TODO: Tell the user that the update couldn't proced correctly
				return;
			}
		}

		Console.WriteLine("No Updates");

		Assembly assembly = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, "Daemon.Core.dll"));
		Type mainClass = assembly.GetType("Daemon.Core.MainApp") ?? throw new InvalidOperationException();

		if (Activator.CreateInstance(mainClass) is not IMainApp mainApp) {
			return;
		}

		mainApp.StartApp();
	}
}