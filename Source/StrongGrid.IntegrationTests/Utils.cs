using System;

namespace StrongGrid.IntegrationTests
{
	public static class Utils
	{
		public static char Prompt(string prompt)
		{
			while (Console.KeyAvailable)
			{
				Console.ReadKey(false);
			}
			Console.Out.WriteLine(prompt);
			var result = Console.ReadKey();
			return result.KeyChar;
		}
	}
}
