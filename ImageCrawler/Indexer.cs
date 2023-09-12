using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ImageCrawler {
	// Use this class to crawl wallpaper source webpages and add not yet downloaded wallpapers to a download index.
	internal class Indexer {
		Task index;
		string directory;
		public event CollectionChangeEventHandler indexChanged;
		public Indexer(string _directory) {
			directory = _directory;
		}
		private void addToIndex(string value) {
			// Raise the event of an added index value.
			if (indexChanged != null) indexChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
		public void StartIndexing() {
			
		}
		public void StopIndexing() {

		}
	}
}