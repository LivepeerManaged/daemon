using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using TestPlugin;

namespace Testing;

public class ApiServerService {
	private readonly DaemonService daemonService;
	private HttpClient httpClient;

	public ApiServerService(DaemonService daemonService) {
		this.daemonService = daemonService;
		this.httpClient = new HttpClient();
	}

/*
 * 
    public async Task<string> ActivateDaemon() {
        RSA rsa = daemonService.GetRSAKey();
        return await PostFormRequest<string>(new Uri(daemonService.GetApiServer(), "/daemon/activateDaemon"), new Dictionary<string, string> {
            { "publicKey", Convert.ToBase64String(rsa.ExportRSAPublicKey()) },
            { "daemonSecret", daemonService.GetSecret() },
            { "signature", daemonService.SignMessage(rsa, daemonService.GetSecret()) },
        });
    }

 */
	/**
     * This one is for later. We will allow the logged in user to create a damon via the api
     * public async Task<IDaemon> CreateDaemon() {
        RSA rsa = daemonService.GetRSAKey();
        return await PostFormRequest<IDaemon>(new Uri(daemonService.GetApiServer(), "/daemon/createDaemon"), new Dictionary<string, string> {
            { "publicKey", Convert.ToBase64String(rsa.ExportRSAPublicKey()) },
        });
    }
     */
	public async Task<string> DaemonLogin(string id, string secret) {
		return await PostFormRequestAsString(new Uri(daemonService.GetApiServer(), "/daemon/login"), new Dictionary<string, string> {
			{ "id", id },
			{ "secret", secret },
		});
	}


	private async Task<T> GetFormRequestAsJson<T>(Uri uri, Dictionary<string, string> parameter) {
		return await HandleJsonResponse<T>(await httpClient.SendAsync(new HttpRequestMessage {
			Method = HttpMethod.Get,
			RequestUri = uri,
			Content = new FormUrlEncodedContent(parameter),
		}));
	}

	private async Task<T> PostFormRequestAsJson<T>(Uri uri, Dictionary<string, string> parameter) {
		return await HandleJsonResponse<T>(await httpClient.SendAsync(new HttpRequestMessage {
			Method = HttpMethod.Post,
			RequestUri = uri,
			Content = new FormUrlEncodedContent(parameter),
		}));
	}

	private async Task<string> GetFormRequestAsString(Uri uri, Dictionary<string, string> parameter) {
		return (await httpClient.SendAsync(new HttpRequestMessage {
			Method = HttpMethod.Get,
			RequestUri = uri,
			Content = new FormUrlEncodedContent(parameter),
		}).Result.Content.ReadAsStringAsync());
	}

	private async Task<string> PostFormRequestAsString(Uri uri, Dictionary<string, string> parameter) {
		return (await httpClient.SendAsync(new HttpRequestMessage {
			Method = HttpMethod.Post,
			RequestUri = uri,
			Content = new FormUrlEncodedContent(parameter),
		}).Result.Content.ReadAsStringAsync());
	}

	private static async Task<T> HandleJsonResponse<T>(HttpResponseMessage response) {
		// TODO Change exception type to the one bellow (and hardcode a known "shared format" for exceptions no matter what exception in the backend) 
		if (response.IsSuccessStatusCode) {
			return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException();
		}

		ApiServerError? error = await response.Content.ReadFromJsonAsync<ApiServerError>();

		if (error == null)
			throw new Exception("Unknown HTTP Exception: " + await response.Content.ReadAsStringAsync());

		throw error;
	}
}