using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace BalticAmadeus_3
{
	class Program
	{
		public static void Main(string[] args)
		{
			File.WriteAllText(@"C:\rez.txt", String.Empty);
			string str = null;
			string label = null;
			string kalba = null;
			var darzelis = new List<Darzelis>();
			var filteredList = new List<Darzelis>();
			var filtered = new List<Tuple<string,int,int>>();
			var percent = new List<Tuple<string, double>>();
			Read(darzelis);
			var min = Min(darzelis);
			var max = Max(darzelis);
			FormatWords(darzelis, str, label, kalba, min, max);
			MostFreeSpace(darzelis, filtered, percent);
			GroupAndSort(darzelis, filteredList);
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		static void Read(List<Darzelis> darzelis)
		{
			using (var reader = new StreamReader(@"C:\test.csv")) {
				string headerLine = reader.ReadLine();
				while (!reader.EndOfStream) {
					var laikinas = new Darzelis();
					var line = reader.ReadLine();
					var values = line.Split(';');
					laikinas.DarzelioId = Convert.ToInt32(values[0]);
					laikinas.Pavadinimas = values[1];
					laikinas.GrupesId = Convert.ToInt32(values[2]);
					laikinas.Grupe = values[3];
					laikinas.KalbosId = Convert.ToInt32(values[4]);
					laikinas.Kalba = values[5];
					laikinas.VaikuSkaicius = Convert.ToInt32(values[6]);
					laikinas.LaisvosVietos = Convert.ToInt32(values[7]);
					darzelis.Add(laikinas);
				}
			}
		}
		static int Min(List<Darzelis> darzelis)
		{
			var min = darzelis.Min(x => x.VaikuSkaicius);
			using (StreamWriter outputFile = new StreamWriter(@"C:\rez.txt", true)) {
				outputFile.WriteLine(min + " - mažiausias");
			}
			return min;
		}
		static int Max(List<Darzelis> darzelis)
		{
			var max = darzelis.Max(x => x.VaikuSkaicius);
			using (StreamWriter outputFile = new StreamWriter(@"C:\rez.txt", true)) {
				outputFile.WriteLine(max + " - didžiausias");
			}
			return max;
		}
		static void FormatWords(List<Darzelis> darzelis, string str, string label, string kalba, int min, int max)
		{
			foreach (var i in darzelis) {
				if (min == i.VaikuSkaicius) {	
					if (i.Pavadinimas[0] == '"') {
						i.Pavadinimas = i.Pavadinimas.Substring(1);
					}
					str = i.Pavadinimas.Substring(0, 3);
					label = i.Grupe;
					label = label.Replace("\"Nuo ", "");
					label = label.Replace(" metų\"", "");
					label = label.Replace(" iki ", "-");
					kalba = i.Kalba.Substring(0, 4);
					using (StreamWriter outputFile = new StreamWriter(@"C:\rez.txt", true)) {
						outputFile.WriteLine(str + "_" + label + "_" + kalba);
					}
				}
				if (max == i.VaikuSkaicius) {	
					if (i.Pavadinimas[0] == '"') {
						i.Pavadinimas = i.Pavadinimas.Substring(1);
					}
					str = i.Pavadinimas.Substring(0, 3);
					label = i.Grupe;
					label = label.Replace("\"Nuo ", "");
					label = label.Replace(" metų\"", "");
					label = label.Replace(" iki ", "-");
					kalba = i.Kalba.Substring(0, 4);
					using (StreamWriter outputFile = new StreamWriter(@"C:\rez.txt", true)) {
						outputFile.WriteLine(str + "_" + label + "_" + kalba);
					}
				}
			}
		}
		static void MostFreeSpace(List<Darzelis> darzelis, List<Tuple<string,int,int>>filtered, List<Tuple<string,double>> percent)
		{
			foreach (var i in darzelis) {
				filtered.Add(Tuple.Create<string, int,int>(i.Kalba, i.LaisvosVietos, i.VaikuSkaicius));
			}
			var sumvalue = filtered.Sum(c => c.Item2);
			while (filtered.Count > 0) {
				var first = filtered[0].Item1;
				double betweensum = filtered.SkipWhile(x => x.Item1 != first)        
    		.TakeWhile(x => x.Item1 == first)
   		 	.Sum(x => x.Item2);
				var betweencount = filtered.SkipWhile(x => x.Item1 != first)        
    		.TakeWhile(x => x.Item1 == first)
   		 	.Sum(x => x.Item3);
				double kiek = (betweensum / betweencount) * 100;
				kiek = Math.Round(kiek, 2);
				percent.Add(Tuple.Create<string, double>(first, kiek));
				filtered.RemoveAll(item => item.Item1 == first);
			}
			var most = percent.Max(x => x.Item2);
			var lang = percent.Max(x => x.Item1);
			using (StreamWriter outputFile = new StreamWriter(@"C:\rez.txt", true)) {
				outputFile.WriteLine(string.Format("{0:0.00}", most + "% - " + lang));
			}
				
		}
		static void GroupAndSort(List<Darzelis> darzelis, List<Darzelis> filteredList)
		{
			filteredList = darzelis.Where(x => x.LaisvosVietos >= 2 && x.LaisvosVietos <= 4).ToList();
			var query =
				from darzel in filteredList
				group darzel by darzel.Pavadinimas into newGroup
				orderby newGroup.Key
				select newGroup;
			query = query.OrderByDescending(x => x.Key);
			foreach (var nameGroup in query) {
				using (StreamWriter outputFile = new StreamWriter(@"C:\rez.txt", true)) {
					outputFile.WriteLine(nameGroup.Key + ":");
				}
				foreach (var darzel in nameGroup) {
					using (StreamWriter outputFile = new StreamWriter(@"C:\rez.txt", true)) {
						outputFile.WriteLine(darzel.Grupe + "; Kalba:" + darzel.Kalba + "; " + darzel.VaikuSkaicius + " vaikai; " + darzel.LaisvosVietos + " laisvos vietos");
					}
				}
			}
		}
	}

	public class Darzelis
	{
		public int DarzelioId { get; set; }
		public string Pavadinimas { get; set; }
		public int GrupesId { get; set; }
		public string Grupe { get; set; }
		public int KalbosId { get; set; }
		public string Kalba { get; set; }
		public int VaikuSkaicius { get; set; }
		public int LaisvosVietos { get; set; }

	}
}