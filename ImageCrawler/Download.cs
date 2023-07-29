using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCrawler {
	internal class Download {
		string pUrl;
		public string URL {
			get {
				return pUrl;
			}
		}
		// TODO
		public Download(string URL) {
			pUrl = URL;
		}
		public void Abort() {
			throw new NotImplementedException();
		}
	}
}