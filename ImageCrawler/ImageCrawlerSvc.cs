using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCrawler {
	// Use this class to download images to a specified directory
	public class ImageCrawlerSvc {
		Queue<string> index {
			get {
				return index;
			}
			set {
				index = value;
				indexUpdated();
			}
		}
		public int parallelDownloadMax;
		string imgDir;
		Indexer indexer;
		Downloader downloader;

		// imgDir: Wallpaper Destination directory
		public ImageCrawlerSvc(string _imgDir, int _parallelDownloadMax) {
			index = new Queue<string>();
			imgDir = _imgDir;
			parallelDownloadMax = _parallelDownloadMax;
			indexer = new Indexer();
			downloader = new Downloader(imgDir, index, parallelDownloadMax);
		}
		// Start downloading
		public void Start() {
			// Start indexing
			// Enable downloader
		}
		// Stop downloading
		public void Stop() {
			// Stop indexing
			// Clear index
			// Disable downloader
		}
		private void indexUpdated() {
			downloader.UpdateIndex(index);
		}
	}
}