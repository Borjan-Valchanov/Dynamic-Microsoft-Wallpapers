using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCrawler {
	// Use this class to download images to a specified directory
	public class ImageCrawlerSvc {
		List<string> index {
			get {
				return index;
			}
			set {
				index = value;
				indexUpdated();
			}
		}
		string imgDir;
		Indexer indexer;
		Downloader downloader;

		// imgDir: Wallpaper Destination directory
		public ImageCrawlerSvc(string _imgDir) {
			index = new List<string>();
			imgDir = _imgDir;
			indexer = new Indexer();
			downloader = new Downloader();
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
		// At some point this should be replaced with an event handler
		private void indexUpdated() {
			indexer.UpdateIndex(index);
		}
	}
}