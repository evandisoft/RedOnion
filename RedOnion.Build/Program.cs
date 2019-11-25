using System;
using System.IO;

namespace RedOnion.Build
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (!File.Exists("RedOnion.sln"))
			{
				Directory.SetCurrentDirectory("../../..");
				if (!File.Exists("RedOnion.sln"))
				{
					Console.WriteLine("Could not locate RedOnion.sln");
					Environment.Exit(-1);
				}
			}
			if (args.Length == 0)
			{
				GithubIOLinks.Exec();

				Release.Exec();
				Documentation.Exec();
				return;
			}
			foreach (var arg in args)
			{
				switch (arg.ToLowerInvariant())
				{
				case "release":
					Release.Exec();
					continue;
				case "documentation":
				case "docs":
					Documentation.Exec();
					continue;
				}
			}
		}
	}
}
