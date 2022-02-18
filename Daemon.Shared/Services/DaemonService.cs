using System.Security.Cryptography;
using System.Text;
using Castle.Core.Internal;
using Testing;
using Testing.Exceptions;

namespace TestPlugin;

public class DaemonService {
	private ConfigService configService { get; set; }

	public Uri GetApiServer() {
		if (GetConfig().ApiServer.IsNullOrEmpty())
			throw new MissingApiServerException();

		if (!Uri.TryCreate($"{(GetConfig().UseSsl ? "https://" : "http://")}{GetConfig().ApiServer}:{GetConfig().ApiServerPort}", UriKind.Absolute, out Uri? uri))
			throw new InvalidApiServerException(GetConfig().ApiServer);

		return uri;
	}

	public IDaemonConfig GetConfig() {
		return configService.GetConfig<IDaemonConfig>("Daemon");
	}

	public string GetSecret() {
		return GetConfig().DaemonSecret;
	}

	public string getId() {
		return GetConfig().DaemonId;
	}

	/*
	 *   public RSA GetRSAKey() {
	      IDaemonConfig config = GetConfig();
	      
	      if (config.PublicKey.IsNullOrEmpty() || config.PrivateKey.IsNullOrEmpty()) {
	          throw new MissingKeyPairException();
	      }
	      
	      return ParseKeyPair(config.PublicKey, config.PrivateKey);
	  }
	 */

	public string SignMessage(RSA rsa, string message) {
		return Convert.ToBase64String(rsa.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1));
	}

	private RSA ParseKeyPair(string publicKey, string privateKey) {
		RSA rsa = RSA.Create(2048);
		rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
		rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
		return rsa;
	}
}