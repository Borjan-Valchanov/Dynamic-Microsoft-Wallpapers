using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		WebClient wCli;
		// Event handler for when the download finishes.
		// Now, let's take a moment to talk about event handlers. I thought of putting this in the commit message for this commit,
		// but who's ever going to read that anyway?
		// In programming, you will at some point develop a feeling of how you would implement something in code, even very abstract
		// things. But, this is just the "pseudo-code" of your application. Software Engineers would call it low level architecture.
		// The actual programmer's job is to remember the keywords and boilerplate that are required for every piece of this low level
		// architecture. But these keywords and boilerplate live in a two-class society. The one kind is the one that is just so useful
		// and needs to be used so often, you just never forget it. The other kind is the one that you sometimes really need, but
		// it's just so specialised and rarely needed that you always forget about it. That's what we got StackOverflow for.
		// The thing is: event handlers are right in the middle. They're just so useful, and quite often even, but it's still rare
		// enough that you always keep forgetting them!
		public event AsyncCompletedEventHandler DownloadFinished;
		// Event handler for when the download progress changes. Useful for only updating UI when necessary.
		public event DownloadProgressChangedEventHandler ProgressChanged;
		public Download(string _url, string _filePath) {
			url = _url;
			filePath = _filePath;
			progress = 0;
			wCli = new WebClient();
			wCli.DownloadFileAsync(new Uri(url), filePath);
			wCli.DownloadProgressChanged += (sender, e) => {
				progress = e.ProgressPercentage;
				if (ProgressChanged != null) ProgressChanged(sender, e);
			};
			wCli.DownloadFileCompleted += (sender, e) => {
				if (DownloadFinished != null) DownloadFinished(sender, e);
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
