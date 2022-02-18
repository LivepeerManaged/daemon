using System.Net.Http.Json;
using Daemon.Shared.Entities;

namespace Daemon.Shared.Services;

public class ApiServerService {
	private DaemonService daemonService { get; set; }
	private HttpClient httpClient = new HttpClient();

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