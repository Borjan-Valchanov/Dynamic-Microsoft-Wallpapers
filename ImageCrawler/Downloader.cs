using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCrawler {
	// Use this class to download a specified number of images simultaneously to a specified directory from a specified index.
	internal class Downloader {
		// Holds to be downloaded elements
		Queue<string> index;
		// Holds ongoing downloads
		List<Download> downloading;
		// Maximum amount of downloads in parallel
		public int parallelDownloadMax;
		// TODO
		public Downloader(Queue<string> _index, int _parallelDownloadMax = 1) {
			index = _index;
			parallelDownloadMax = _parallelDownloadMax;
		}
		public void UpdateIndex(Queue<string> _index) {
			index = _index;
		}
	}
}