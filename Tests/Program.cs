using System;
using System.Runtime.CompilerServices;

namespace Tests {
	class Program {
		static void Main(string[] args) {
			List<(string, Action)> tests = new List<(string, Action)> {
				("ImageCrawler.Downloader", imageCrawlerDownloaderTest)
			};
			for (int i = 0; i < tests.Count; i++) {
				Console.WriteLine(i.ToString() + ": " + tests[i].Item1);
			}
			Console.Write("Please enter which test you would like to run: ");
			int testNr = Convert.ToInt32(Console.ReadLine());
			tests[testNr].Item2.Invoke();
		}
		static void imageCrawlerDownloaderTest() {

		}
	}
}