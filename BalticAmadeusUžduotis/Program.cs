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
			File.WriteAllText(@"C:\MinMax.txt", String.Empty); //išvalomi .txt duomenys
			File.WriteAllText(@"C:\Format.txt", String.Empty);
			File.WriteAllText(@"C:\Group.txt", String.Empty);
			File.WriteAllText(@"C:\FreeSpace.txt", String.Empty);
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
		static void Read(List<Darzelis> darzelis) //skaitomi duomenys iš .csv failo
		{
			using (var reader = new StreamReader(@"C:\test.csv")) {
				string headerLine = reader.ReadLine();
				while (!reader.EndOfStream) 
				{
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
		static int Min(List<Darzelis> darzelis) //grąžinama minimali reikšmė
		{
			var min = darzelis.Min(x => x.VaikuSkaicius);
			using (StreamWriter outputFile = new StreamWriter(@"C:\MinMax.txt", true)) {
				outputFile.WriteLine(min + " - mažiausias");
			}
			Console.WriteLine(min + " - mažiausias");
			return min;
		}
		static int Max(List<Darzelis> darzelis) //grąžinama maksimali reikšmė
		{
			var max = darzelis.Max(x => x.VaikuSkaicius);
			using (StreamWriter outputFile = new StreamWriter(@"C:\MinMax.txt", true)) {
				outputFile.WriteLine(max + " - didžiausias");
			}
			Console.WriteLine(max + " - didžiausias");
			return max;
		}
		static void FormatWords(List<Darzelis> darzelis, string str, string label, string kalba, int min, int max)
		{
			foreach (var i in darzelis) 
			{
				if (min == i.VaikuSkaicius) //ieško visų darželių, turinčių minimalią reikšmę
				{	
					if (i.Pavadinimas[0] == '"') //panaikina kabutes
					{
						i.Pavadinimas = i.Pavadinimas.Substring(1);
					}
					str = i.Pavadinimas.Substring(0, 3);
					label = i.Grupe;
					label = label.Replace("\"Nuo ", "");
					label = label.Replace(" metų\"", "");
					label = label.Replace(" iki ", "-");
					kalba = i.Kalba.Substring(0, 4);
					using (StreamWriter outputFile = new StreamWriter(@"C:\Format.txt", true)) {
						outputFile.WriteLine(str + "_" + label + "_" + kalba); //išrašomasa apkarpytas sakinys į .txt failą
					}
				}
				if (max == i.VaikuSkaicius) //ieško pagal didžiausią reikšmę
				{	
					if (i.Pavadinimas[0] == '"') //panaikina kabutes
					{
						i.Pavadinimas = i.Pavadinimas.Substring(1);
					}
					str = i.Pavadinimas.Substring(0, 3);
					label = i.Grupe;
					label = label.Replace("\"Nuo ", "");
					label = label.Replace(" metų\"", "");
					label = label.Replace(" iki ", "-");
					kalba = i.Kalba.Substring(0, 4);
					using (StreamWriter outputFile = new StreamWriter(@"C:\Format.txt", true)) {
						outputFile.WriteLine(str + "_" + label + "_" + kalba); //išrašo apkarpytą sakinį
					}
				}
			}
		}
		static void MostFreeSpace(List<Darzelis> darzelis, List<Tuple<string,int,int>>filtered, List<Tuple<string,double>> percent)
		{
			foreach (var i in darzelis) 
			{
				filtered.Add(Tuple.Create<string, int,int>(i.Kalba, i.LaisvosVietos, i.VaikuSkaicius));
			}
			while (filtered.Count > 0) //vykdo ciklą, kol list'as nėra tuščias
			{
				var first = filtered[0].Item1;
				double betweensum = filtered.SkipWhile(x => x.Item1 != first)        
    		.TakeWhile(x => x.Item1 == first)
   		 	.Sum(x => x.Item2);
				var betweencount = filtered.SkipWhile(x => x.Item1 != first)        
    		.TakeWhile(x => x.Item1 == first)
   		 	.Sum(x => x.Item3);
				double kiek = (betweensum / betweencount) * 100;
				percent.Add(Tuple.Create<string, double>(first, kiek));
				filtered.RemoveAll(item => item.Item1 == first);
			}
			var most = percent.Max(x => x.Item2); //randamas procentaliai laisviausias darželis
			var lang = percent.Max(x => x.Item1);
			using (StreamWriter outputFile = new StreamWriter(@"C:\FreeSpace.txt", true)) {
				outputFile.WriteLine(string.Format("{0:0.00}", most) + "% - " + lang);
			}
				
		}
		static void GroupAndSort(List<Darzelis> darzelis, List<Darzelis> filteredList)
		{
			filteredList = darzelis.Where(x => x.LaisvosVietos >= 2 && x.LaisvosVietos <= 4).ToList();
			var query =
				from darzel in filteredList //sugrupuojami pagal pavadinimą
				group darzel by darzel.Pavadinimas into newGroup
				orderby newGroup.Key
				select newGroup;
			query = query.OrderByDescending(x => x.Key); //rikiuojama nuo z iki a
			foreach (var nameGroup in query) {
				using (StreamWriter outputFile = new StreamWriter(@"C:\Group.txt", true)) {
					outputFile.WriteLine(nameGroup.Key + ":"); //spausdinami duomenys į .txt failą
				}
				foreach (var darzel in nameGroup) {
					using (StreamWriter outputFile = new StreamWriter(@"C:\Group.txt", true)) {
						outputFile.WriteLine(darzel.Grupe + "; Kalba:" + darzel.Kalba + "; " + darzel.VaikuSkaicius + " vaikai; " + darzel.LaisvosVietos + " laisvos vietos");
					}
				}
			}
		}
	}

	public class Darzelis //darželių klasė
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
