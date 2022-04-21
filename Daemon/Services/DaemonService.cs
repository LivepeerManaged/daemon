using System.Reflection;
using Castle.Core.Internal;
using Daemon.Shared.Entities;
using Daemon.Shared.Exceptions;
using Daemon.Shared.Services;

namespace Daemon.Services;

public class DaemonService : IDaemonService {
	private IConfigService ConfigService { get; set; }
	private IReflectionsService ReflectionsService { get; set; }

	public Uri GetApiServer() {
		if (GetConfig().ApiServer.IsNullOrEmpty()) {
			throw new MissingApiServerException();
		}

		if (!Uri.TryCreate($"{(GetConfig().UseSsl ? "https://" : "http://")}{GetConfig().ApiServer}:{GetConfig().ApiServerPort}", UriKind.Absolute, out Uri? uri)) {
			throw new InvalidApiServerException(GetConfig().ApiServer);
		}

		return uri;
	}

	public Uri GetWebsocketServer() {
		return new Uri(GetApiServer(), "daemon");
	}

	public IDaemonConfig GetConfig() {
		return ConfigService.GetConfig<IDaemonConfig>("Daemon");
	}

	public string GetSecret() {
		return GetConfig().DaemonSecret;
	}

	public string getId() {
		return GetConfig().DaemonId;
	}

	public AssemblyInfo GetDaemonInfo() {
		return ReflectionsService.GetAssemblyInfo(Assembly.GetEntryAssembly());
	}
}