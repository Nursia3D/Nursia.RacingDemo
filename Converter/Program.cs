using System;
using System.IO;

namespace Converter
{
	internal static class Program
	{
		static void Process()
		{
			var inputFolder = @"D:\Projects\Nursia.RacingGame\RacingGame\Assets\Models";
			var outputFolder = @"D:\Projects\Nursia.RacingGame\RacingGame\Assets\Scenes";

			var models = Directory.EnumerateFiles(inputFolder, "*.material");

			foreach (var model in models)
			{
			}
		}

		static void Main(string[] args)
		{
			try
			{
				using (var game = new Game1())
				{
					game.Run();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}