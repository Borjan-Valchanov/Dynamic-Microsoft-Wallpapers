using System;
using ImageCrawler;
using System.Runtime.CompilerServices;

namespace Tests {
	class Program {
		static void Main(string[] args) {
			List<(string, Action)> tests = new List<(string, Action)> {
				("ImageCrawler 30min No Parallel Max", imageCrawlerTest30NoPMax)
			};
			for (int i = 0; i < tests.Count; i++) {
				Console.WriteLine(i.ToString() + ": " + tests[i].Item1);
			}
			Console.Write("Please enter which test you would like to run: ");
			int testNr = Convert.ToInt32(Console.ReadLine());
			tests[testNr].Item2.Invoke();
		}
		static void imageCrawlerTest30NoPMax() {
			Console.Write("Please enter your preferred test directory: ");
			ImageCrawlerSvc imageCrawlerSvc = new ImageCrawlerSvc(Console.ReadLine(), -1, true);
			imageCrawlerSvc.Start();
			Thread.Sleep(30 * 60 * 1000);
			imageCrawlerSvc.Stop();
		}
	}
}