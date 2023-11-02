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
		Dictionary<string, Download> downloading;
		private int PparallelDownloadMax;
		bool debug = false;
		// Maximum amount of downloads in parallel
		public int parallelDownloadMax {
			get {
				return PparallelDownloadMax;
			}
			set {
				// if the maximum download limit is raised, we can start new downloads
				bool initNewDownloads = value > parallelDownloadMax;
				PparallelDownloadMax = value;
				if (initNewDownloads && enabled) startNewDownloads();
			}
		}
		public Downloader(string _fileDestDir, int _parallelDownloadMax = 1, bool _debug = false) {
			debug = _debug;
			enabled = false;
			fileDestDir = _fileDestDir;
			index = new Queue<string>();
			parallelDownloadMax = _parallelDownloadMax;
			downloading = new Dictionary<string, Download>();
		}
		// Expose a method to add a new item to the index when the Indexer raises a indexChanged event
		public void addToIndex(string value) {
			// TODO: Add debug log output
			if (string.IsNullOrEmpty(value)) return;
			index.Enqueue(value);
			if (enabled) startNewDownloads();
		}
		// Start new downloads.
		// Will be called when a download finishes, the download file index updates, or the parallel download limit is raised
		private void startNewDownloads() {
			if (!enabled) return;
			// If there are no files to be downloaded in the index or the parallel download maximum is reached or exceeded
			// (if it is not infinite aka -1), do not start any new downloads
			if (downloading.Count >= parallelDownloadMax && parallelDownloadMax != -1) return;
			// Iterate so many times as that the parallel download maximum is met (if it's not infinite aka -1) or the end of the index reached
			for (int c = downloading.Count; (c < parallelDownloadMax || parallelDownloadMax == -1) && index.Count != 0; c++) {
				// Retrieve oldest index item
				string downloadURL;
				try {
					downloadURL = index.Dequeue();
				} catch (InvalidOperationException ex) {
					// TODO: Add debug log output
					return;
				}
				// TODO: Add debug log output
				if (string.IsNullOrEmpty(downloadURL)) return;
				// Generate file path based on destination directory and file name of the downloaded file
				string filename = fileDestDir + "\\" + downloadURL.Split("/").Last();
				// If the file exists already, do not download it again
				if (File.Exists(filename)) continue;
				// Initialise a download object to handle the download
				Download download = new Download(downloadURL, filename);
				// When the download finishes, remove the download from the download list
				download.DownloadFinished += (sender, e) => {
					// I honestly hate this solution. During debugging, I'd be getting the
					// "Finished download could not be removed." exception when using the Disable() method,
					// since the download dictionary would already be re-initialised after the Abort()
					// method of the Download objects were called. The completion of that method
					// (apart from the fact that they're called asynchronously) does however not align
					// with the completion of the download: when using Abort(), we need to wait
					// until the download object releases its file handle on the download file
					// and then deletes the incomplete download file, which is why
					// download abortion as well as completion are handled by one event handler, as well here as
					// in the download object itself. This leads to this event handler trying to remove dictionary
					// entries that don't exist anymore. Since I didn't just want to ditch the reinitialising of the
					// dictionary (Who knows what could go wrong that there stays something behind?)
					// I opted for this abominable solution. The reason it checks if the dictionary is empty before
					// checking if the download object is still contained is since Dictionary.Count is O(1)
					// as the count is seperately kept track of instead of counting all of it's entries, as opposed
					// to Dictionary.ContainsKey() which, at worst, is O(n).
					if (downloading.Count != 0) {
						if (downloading.ContainsKey(downloadURL)) {
							if (!downloading.Remove(downloadURL)) throw new Exception("Finished download could not be removed.");
						}
					}
					startNewDownloads();
				};
				// Add download to download list
				try
				{
					downloading.Add(downloadURL, download);
				}
				catch (Exception ex)
				{
					Console.WriteLine(downloading.ToString());
					// TODO: Add debug log output
					return;
				}
			}
		}
		// Enables downloading
		public void Enable() {
			enabled = true;
			startNewDownloads();
		}
		// Disables downloading and aborts all current downloads
		public void Disable() {
			// This stops any calls to startNewDownloads from starting new downloads
			enabled = false;
			// Tell every download SIMULTANEOUSLY to abort (honestly I love this)
			downloading.AsParallel().ForAll(downloadPair => {
				downloadPair.Value.Abort();
			});
			// Reinitialise the download dictionary
			downloading = new Dictionary<string, Download>();
			// Reinitialise the index
			index = new Queue<string>();
		}
	}
}