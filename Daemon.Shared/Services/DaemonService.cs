using System.Security.Cryptography;
using System.Text;
using Castle.Core.Internal;
using Daemon.Shared.Entities;
using Daemon.Shared.Exceptions;

namespace Daemon.Shared.Services;

public class DaemonService {
	private ConfigService ConfigService { get; set; }

	public Uri GetApiServer() {
		if (GetConfig().ApiServer.IsNullOrEmpty()) {
			throw new MissingApiServerException();
		}

		if (!Uri.TryCreate($"{(GetConfig().UseSsl ? "https://" : "http://")}{GetConfig().ApiServer}:{GetConfig().ApiServerPort}", UriKind.Absolute, out Uri? uri)) {
			throw new InvalidApiServerException(GetConfig().ApiServer);
		}

		return uri;
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
}