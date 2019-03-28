using RedOnion.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace RedOnion.Shell
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Expected path to script");
				Environment.Exit(-1);
				return;
			}
			var path = args[0];
			if (!File.Exists(path))
			{
				Console.WriteLine("File does not exist: " + path);
				Environment.Exit(-1);
			}
			var engine = new Engine();
			engine.Printing += s => Console.WriteLine(s);
			var global = engine.Root;
			global.AddType(typeof(ZipArchive));
			global.AddType(typeof(ZipArchiveMode));
			global.AddType(typeof(Stream));
			global.AddType(typeof(FileStream));
			global.AddType(typeof(Directory));
			global.AddType(typeof(DirectoryInfo));
			global.AddType(typeof(Path));
			global.AddType(typeof(File));
			global.AddType(typeof(FileMode));
			global.AddType(typeof(Encoding));
			try
			{
				engine.Execute(File.ReadAllText(args[0]));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				if (ex is IErrorWithLine ln)
					Console.WriteLine(ln.Line == null ?
						"At line {0}." : "At line {0}: {1}",
						ln.LineNumber, ln.Line);
				Environment.Exit(-1);
			}
		}
	}
}
