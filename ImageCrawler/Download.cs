using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageCrawler {
	internal class Download {
		// Public read-only int to read download progress
		public int Progress {
			get {
				return progress;
			}
		}
		// Private int to store download progress
		int progress;
		// Stores the URL of the downloaded file
		string url;
		// Specifies the destination the file should be downloaded to (including filename)
		string filePath;
		// WebClient used to download the file.
		// TODO:
		// As of right now, this is marked PUBLIC to allow ImageCrawler.Download to subscribe to the DownloadFileComplete event
		// This is NOT intended behaviour, end the ImageCrawler.Download class should at some point get its own event handler
		// and wCli be marked as PRIVATE.
		public WebClient wCli;
		public Download(string _url, string _filePath) {
			url = _url;
			filePath = _filePath;
			progress = 0;
			wCli = new WebClient();
			wCli.DownloadFileAsync(new Uri(url), filePath);
			wCli.DownloadProgressChanged += (sender, e) => {
				progress = e.ProgressPercentage;
			};
		}

		public void Abort() {
			// Stop the download
			wCli.CancelAsync();
			// Delete incomplete download file. This is necessary due to the behaviour of ImageCrawler.Download to
			// ignore files that already exist in their path
			File.Delete(filePath);
			progress = 0;
		}
	}
}
