using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageCrawler {
	// Use this class to download a specified number of images simultaneously to a specified directory from a specified index.
	internal class Downloader {
		// Specifies download destination directory
		string fileDestDir;
		// Holds to be downloaded elements
		Queue<string> index;
		// Holds ongoing downloads
		List<Task> downloading;
		// Maximum amount of downloads in parallel
		public int parallelDownloadMax {
			get {
				return parallelDownloadMax;
			}
			set {
				// if the maximum download limit is raised, we can start new downloads
				bool initNewDownloads = value > parallelDownloadMax;
				parallelDownloadMax = value;
				if (initNewDownloads) startNewDownloads();
			}
		}
		public Downloader(string _fileDestDir, Queue<string> _index, int _parallelDownloadMax = 1) {
			fileDestDir = _fileDestDir;
			index = _index;
			parallelDownloadMax = _parallelDownloadMax;
			downloading = new List<Task>();
			startNewDownloads();
		}
		// Intercept when the ImageCrawlerSvc object makes a change to the Downloader's download index,
		// so new downloads can be initiated right away.
		// This is used instead of an accessor so index changes from within the downloader do not cause a
		// startNewDownloads() call
		public void UpdateIndex(Queue<string> _index) {
			index = _index;
			startNewDownloads();
		}
		// TODO: Reintroduce ImageCrawler.Download class to better handle download abortion/failing.
		//
		// Start new downloads.
		// Will be called when a download finishes, the download file index updates, or the parallel download limit is raised
		private void startNewDownloads() {
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
				// Create a web client to handle the download
				WebClient wCli = new WebClient();
				// Create a new download task
				Task downloadTask = wCli.DownloadFileTaskAsync(downloadURL, filename);
				// When the download finishes, remove the download task from download list
				wCli.DownloadFileCompleted += (sender, e) => {
					if (!downloading.Remove(downloadTask)) throw new Exception("Finished download task could not be removed.");
					startNewDownloads();
				};
				// Start downloading
				downloadTask.Start();
			}
		}
	}
}