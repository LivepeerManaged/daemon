using System.IO.Compression;
using System.Net;
using Config.Net;
using NLog;
using Octokit;

namespace TestPlugin.Service;

public class UpdaterService {
	GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("livepeerManagedUpdater"));
	private readonly Logger _logger = LogManager.GetLogger(typeof(UpdaterService).FullName);

	public async Task<bool> CheckForUpdates() {
		ILivepeerVersion currentConfig = GetConfig();

		if (!((DateTime.Now - currentConfig.LastCheckedUtc).TotalHours > 1)) {
			_logger.Info("No Update available (Updates are only checked every hour because of the GitHub API limits)");
			return false;
		}

		_logger.Info("Updating livepeer_version with newest version");

		ILivepeerVersion updatedConfig = await UpdateVersionFile();
		if (currentConfig.Id == updatedConfig.Id) {
			_logger.Info("No Update available...");
			return false;
		}

		_logger.Info("New version found!");
		_logger.Info($"Old Version: \"{currentConfig.Name}\" New Version: {updatedConfig.Name}");
		_logger.Info("Downloading...");

		await Download(updatedConfig.DownloadUri);

		_logger.Info("Download finished!");

		return true;
	}

	public async Task EnsureGoLivepeer() {
		ILivepeerVersion livepeerVersion = GetConfig();
		string downloadFileName = Path.GetFileName(livepeerVersion.DownloadUri.ToString());
		if (!Directory.Exists("go-livepeer") && !File.Exists(downloadFileName)) {
			_logger.Warn("go-livepeer missing! Starting installing...");
			await Download(livepeerVersion.DownloadUri);
		}
		
		if (!Directory.Exists("go-livepeer") && File.Exists(downloadFileName)) {
			_logger.Warn("go-livepeer not extracted! Extrating...");
			ExtractZip(downloadFileName);
			_logger.Info("successfully extracted!");
		}
	}
	

	private Task Download(Uri uri) {
		WebClient webClient = new WebClient();
		int last = -1;
		webClient.DownloadProgressChanged += (sender, args) => {
			int modolo = args.ProgressPercentage % 10;
			if (modolo != 0) {
				return;
			}

			if (last == args.ProgressPercentage)
				return;

			last = args.ProgressPercentage;

			_logger.Info($"Downloading latest Livepeer version... {args.ProgressPercentage}%");
		};
		return webClient.DownloadFileTaskAsync(uri, Path.GetFileName(uri.ToString()));
	}

	private async Task<ILivepeerVersion> UpdateVersionFile() {
		IReadOnlyList<Release>? releases = await gitHubClient.Repository.Release.GetAll("livepeer", "go-livepeer");
		Release? latest = releases[0];

		ILivepeerVersion livepeerVersion = GetConfig();
		livepeerVersion.Id = latest.Id;
		livepeerVersion.Name = latest.TagName;
		Uri? uri = null;

		if (OperatingSystem.IsWindows())
			uri = new Uri(latest.Assets.First(asset => asset.Name == "livepeer-windows-amd64.zip").BrowserDownloadUrl);
		else if (OperatingSystem.IsLinux())
			uri = new Uri(latest.Assets.First(asset => asset.Name == "livepeer-linux-amd64.tar.gz").BrowserDownloadUrl);
		else if (OperatingSystem.IsMacOS())
			uri = new Uri(latest.Assets.First(asset => asset.Name == "livepeer-darwin-amd64.tar.gz").BrowserDownloadUrl);

		livepeerVersion.DownloadUri = uri;
		livepeerVersion.LastCheckedUtc = DateTime.Now;
		return livepeerVersion;
	}

	private void ExtractZip(string pathFrom) {
		string folderName = Path.GetFileNameWithoutExtension(pathFrom);
		ZipFile.ExtractToDirectory(pathFrom, folderName, true);
		DirectoryInfo directoryInfo = new DirectoryInfo(folderName).GetDirectories().First();
		directoryInfo.MoveTo("go-livepeer");
	}

	private ILivepeerVersion GetConfig() {
		return new ConfigurationBuilder<ILivepeerVersion>().UseJsonFile($"livepeer_version").Build();
	}
}