using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageCrawler {
	// Use this class to download images to a specified directory
	public class ImageCrawlerSvc {
		public int parallelDownloadMax;
		string directory;
		Indexer indexer;
		Downloader downloader;
		bool debug = false;

		// imgDir: Wallpaper Destination directory
		public ImageCrawlerSvc(string _directory, int _parallelDownloadMax, bool _debug = false) {
			debug = _debug;
			// If there's a new line in the directory provided, it's invalid. I mean seriously?
			if (_directory.Contains(Environment.NewLine)) throw new ArgumentException("The provided directory is invalid (New line)");
			// Regex that matches all backslshes immediately in front of the end of the string
			Regex directoryRegex = new Regex("\\*$");
			// Replace all forward slashes with backward ones and remove all backward slashes at the end, then assign that to this.directory
			directory = directoryRegex.Replace(_directory.Replace("/", "\\"), "");
			// If the directory doesn't exist, create it.
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
			parallelDownloadMax = _parallelDownloadMax;
			indexer = new Indexer(_directory, debug);
			downloader = new Downloader(directory, parallelDownloadMax, debug);
			indexer.indexChanged += (sender, e) => {
				if (e.Action != System.ComponentModel.CollectionChangeAction.Add) throw new ArgumentException("Unexpected index event sent by ImageCrawler.Indexer: expected \"add\" action");
				downloader.addToIndex(e.Element.ToString());
			};
		}
		// Start downloading
		public void Start() {
			// Start indexing
			indexer.StartIndexing();
			// Enable downloader
			downloader.Enable();
		}
		// Stop downloading
		public void Stop() {
			// Stop indexing
			indexer.StopIndexing();
			// Disable downloader
			downloader.Disable();
		}
	}
}