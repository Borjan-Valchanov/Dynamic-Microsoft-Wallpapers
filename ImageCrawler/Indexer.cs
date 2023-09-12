using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCrawler {
	// Use this class to crawl wallpaper source webpages and add not yet downloaded wallpapers to a download index.
	internal class Indexer {
		public event CollectionChangeEventHandler indexChanged;
		public Indexer() {

			throw new NotImplementedException();
		}
		private void addToIndex(string value) {
			// Raise the event of an added index value.
			if (indexChanged != null) indexChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
	}
}