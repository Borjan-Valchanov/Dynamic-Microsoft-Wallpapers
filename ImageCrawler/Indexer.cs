using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ImageCrawler {
	// Use this class to crawl wallpaper source webpages and add not yet downloaded wallpapers to a download index.
	internal class Indexer {
		// Website to crawl for Bing wallpapers
		const string bing = "www.bwallpaperhd.com";
		// Website to crawl for Windows Spotlight wallpapers
		const string spotlight = "windows10spotlight.com";
		// Field to store target directory, in order to check whether the image needs to be added to the index at all
		string directory;
		// Event that communicates index additions
		public event CollectionChangeEventHandler indexChanged;
		private event EventHandler indexThreadAbort;
		bool debug = false;
		// The constructor. What else did you expect?
		public Indexer(string _directory, bool _debug = false) {
			debug = _debug;
			directory = _directory;
			// Define the two threads for the two pages
			bingThread = new Thread(() => {
				indexWallpapersFromWPSite(this, bing, debug);
			});
			spotlightThread = new Thread(() => {
				indexWallpapersFromWPSite(this, spotlight, debug);
			});
		}
		private void addToIndex(string value) {
			// Raise the event of an added index value.
			if (indexChanged != null) indexChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
		// Start the two indexing threads
		public void StartIndexing() {
			bingThread.Start();
			spotlightThread.Start();
		}
		// Stop the two indexing threads.
		public void StopIndexing() {
			// Call an event handler that terminates the threads.
			if (indexThreadAbort != null) indexThreadAbort(this, EventArgs.Empty);
		}
		// Define the two indexing threads.
		Thread bingThread;
		Thread spotlightThread;
		// This will be run asynchronously and does the actual crawling.
		static void indexWallpapersFromWPSite(Indexer indexer, string domain, bool debug = false) {
			bool indexingActive = true;
			indexer.indexThreadAbort += (sender, e) => {
				indexingActive = false;
			};
			// Page counter
			int page = 1;
			// Regex for matching the resolution suffix of images
			Regex resolutionRegex = new Regex("-[0-9]*x[0-9]*");
			// string object to store page source code
			string source;
			// WebClient to retrieve the page's source code
			WebClient wCli = new WebClient();
			// HtmlDocument object to be able to work with the page's source code
			HtmlDocument doc = new HtmlDocument();
			// Iterative crawler code will be executed until we get a 404
			try {
				// Iterate over pages
				while (indexingActive) {
					// Download page source html
					source = wCli.DownloadString("https://" + domain + "/page/" + page.ToString());
					// Load for analysis
					doc.LoadHtml(source);
					// Load the source attribute of every image into an IEnumerable<string>, which is just a weird kind of
					// C# string array.
					IEnumerable<string> imgs = doc.DocumentNode.Descendants("img").Select(element => element.Attributes["src"].Value);
					// Iterate over all image src's and check if they qualify
					// If so, add them to the index (and possibly get a cross thread execution exception, but that's fun! Isn't it?)
					for (int i = 0; i < imgs.Count(); i++) {
						// If the image isn't from /wp-content/uploads, continue to the next one
						if (!imgs.ElementAt(i).ToLower().Contains(domain.ToLower() + "/wp-content/uploads/")) continue;
						// Using RegEx to remove resolution scaling from link and invoking index
						indexer.addToIndex(resolutionRegex.Replace(imgs.ElementAt(i), ""));
					}
					// Go to the next page and repeat
					page++;
				}
			} catch (WebException e) {
				// If we get a 404 (HttpStatusCode.NotFound), we're at the end of available pages, so this is expected
				// If not, throw the exception again
				if (typeof(HttpWebResponse) != e.GetType()) throw e;
				if (((HttpWebResponse)e.Response).StatusCode != HttpStatusCode.NotFound) throw e;
				return;
			} catch (ThreadAbortException) {
				return;
			}
		}
	}
}