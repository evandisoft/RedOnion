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
		class Engine : Script.Engine
		{
			public bool Eof => Parser.Eof;
		}
		static Engine engine;
		static void Main(string[] args)
		{
			if (args.Length > 1)
			{
				Console.WriteLine("Too many arguments");
				Environment.Exit(-1);
				return;
			}
			engine = new Engine();
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

			if (args.Length == 1)
			{
				var path = args[0];
				if (!File.Exists(path))
				{
					Console.WriteLine("File does not exist: " + path);
					Environment.Exit(-1);
				}
				try
				{
					engine.Printing += s => Console.WriteLine(s);
					engine.Execute(File.ReadAllText(args[0]));
					return;
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

			Console.WriteLine("Red Onion Script Interactive Console");
			Console.WriteLine("Type 'return' or 'break' to exit");
			engine.Printing += s => Console.WriteLine("--  " + s);
			engine.Options |= EngineOption.Repl;

			var sb = new StringBuilder();
			for (; ; )
			{
				var more = sb.Length > 0;
				Console.Write(more ? "..  " : " >  ");
				var line = Console.ReadLine();
				sb.AppendLine(line);
				CompiledCode code = null;
				try
				{
					code = engine.Compile(sb.ToString());
				}
				catch (Exception ex)
				{
					if (engine.Eof && line != "")
						continue;
					Console.WriteLine(" !  Error: " + ex.Message);
					if (ex is IErrorWithLine ln)
						Console.WriteLine(ln.Line == null ?
							" !  At line {0}." : " !  At line {0}: {1}",
							ln.LineNumber, ln.Line);
				}
				var statement = code.Code?.Length > 0
					&& code.Code[0] >= (byte)OpKind.Statement
					&& code.Code[0] != (byte)OpCode.Autocall;
				if (statement && line != "" && line != "return" && line != "break")
					continue;
				sb.Length = 0;
				try
				{
					engine.Execute(code);
				}
				catch (Exception ex)
				{
					Console.WriteLine(" !  Error: " + ex.Message);
					if (ex is IErrorWithLine ln)
						Console.WriteLine(ln.Line == null ?
							" !  At line {0}." : " !  At line {0}: {1}",
							ln.LineNumber, ln.Line);
					continue;
				}
				Console.WriteLine("=>  " + engine.Result.String);
				if (engine.Exit != 0)
					return;
			}
		}
	}
}
