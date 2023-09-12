using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCrawler {
	// Use this class to download images to a specified directory
	public class ImageCrawlerSvc {
		public int parallelDownloadMax;
		string imgDir;
		Indexer indexer;
		Downloader downloader;

		// imgDir: Wallpaper Destination directory
		public ImageCrawlerSvc(string _imgDir, int _parallelDownloadMax) {
			imgDir = _imgDir;
			parallelDownloadMax = _parallelDownloadMax;
			indexer = new Indexer(_imgDir);
			downloader = new Downloader(imgDir, parallelDownloadMax);
			indexer.indexChanged += (sender, e) => {
				if (e.Action != System.ComponentModel.CollectionChangeAction.Add) throw new ArgumentException("Unexpected index event sent by ImageCrawler.Indexer: expected \"add\" action");
				downloader.addToIndex((string)e.Element);
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