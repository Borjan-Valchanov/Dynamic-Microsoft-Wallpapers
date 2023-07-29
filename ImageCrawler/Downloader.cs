using System;
using System.Collections.Generic;
using System.Linq;
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
		// Start new downloads.
		// Will be called when a download finishes, the download file index updates, or the parallel download limit is raised
		private void startNewDownloads() {
			if (index.Count == 0) return;
			// TODO
		}
	}
}