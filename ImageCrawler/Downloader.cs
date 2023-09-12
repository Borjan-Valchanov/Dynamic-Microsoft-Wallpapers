using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageCrawler {
	// Use this class to download a specified number of images simultaneously to a specified directory from a specified index.
	internal class Downloader {
		// Specifies internally whether downloading is enabled.
		bool enabled;
		// Specifies download destination directory
		string fileDestDir;
		// Holds to be downloaded elements
		Queue<string> index;
		// Holds ongoing downloads
		List<Download> downloading;
		// Maximum amount of downloads in parallel
		public int parallelDownloadMax {
			get {
				return parallelDownloadMax;
			}
			set {
				// if the maximum download limit is raised, we can start new downloads
				bool initNewDownloads = value > parallelDownloadMax;
				parallelDownloadMax = value;
				if (initNewDownloads && enabled) startNewDownloads();
			}
		}
		public Downloader(string _fileDestDir, int _parallelDownloadMax = 1) {
			enabled = false;
			fileDestDir = _fileDestDir;
			index = new Queue<string>();
			parallelDownloadMax = _parallelDownloadMax;
			downloading = new List<Download>();
		}
		// Expose a method to add a new item to the index when the Indexer raises a indexChanged event
		public void addToIndex(string value) {
			index.Enqueue(value);
			if (enabled) startNewDownloads();
		}
		// Start new downloads.
		// Will be called when a download finishes, the download file index updates, or the parallel download limit is raised
		private void startNewDownloads() {
			if (!enabled) return;
			// If there are no files to be downloaded in the index or the parallel download maximum is reached or exceeded,
			// do not start any new downloads
			if (index.Count == 0) return;
			if (downloading.Count >= parallelDownloadMax) return;
			// Iterate so many times as that the parallel download maximum is met or the end of the index reached
			for (int c = downloading.Count; c < parallelDownloadMax && index.Count != 0; c++) {
				// Retrieve oldest index item
				string downloadURL = index.Dequeue();
				// Generate file path based on destination directory and file name of the downloaded file
				string filename = fileDestDir + downloadURL.Split("/").Last();
				// If the file exists already, do not download it again
				if (File.Exists(filename)) continue;
				// Initialise a download object to handle the download
				Download download = new Download(downloadURL, filename);
				// When the download finishes, remove the download from the download list
				download.DownloadFinished += (sender, e) => {
					if (!downloading.Remove(download)) throw new Exception("Finished download could not be removed.");
					startNewDownloads();
				};
				// Add download to download list
				downloading.Add(download);
			}
		}
		// Enables downloading
		public void Enable() {
			enabled = true;
			startNewDownloads();
		}
		// Disables downloading and aborts all current downloads
		public void Disable() {
			enabled = false;
			for (int i = 0; i < downloading.Count; i++) {
				downloading[i].Abort();
			}
			downloading = new List<Download>();
			index = new Queue<string>();
		}
	}
}