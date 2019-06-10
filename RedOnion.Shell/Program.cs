using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using RedOnion.ROS;
using RedOnion.ROS.Objects;

namespace RedOnion.Shell
{
	class Program
	{
		static ShellCore core;
		static ShellGlobals globals;
		class ShellGlobals: Globals
		{
			public ShellGlobals()
			{
				Add(typeof(ZipArchive));
				Add(typeof(ZipArchiveMode));
				Add(typeof(Stream));
				Add(typeof(FileStream));
				Add(typeof(Directory));
				Add(typeof(DirectoryInfo));
				Add(typeof(Path));
				Add(typeof(File));
				Add(typeof(FileMode));
				Add(typeof(Encoding));
			}
		}
		class ShellCore: Core
		{
			public ShellCore() : base(globals = new ShellGlobals()) { }
			public bool Eof => Parser.Eof;
		}
		static void Main(string[] args)
		{
			if (args.Length > 1)
			{
				Console.WriteLine("Too many arguments");
				Environment.Exit(-1);
				return;
			}

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
					core = new ShellCore();
					Print.Listen += s => Console.WriteLine(s);
					core.Execute(File.ReadAllText(args[0]));
					return;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error: " + ex.Message);
					if (ex is Error err)
						Console.WriteLine(err.Line == null ?
							"At line {0}." : "At line {0}: {1}",
							err.LineNumber, err.Line);
					Environment.Exit(-1);
				}
			}

			Console.WriteLine("Red Onion Script Interactive Console");
			Console.WriteLine("Type 'return' or 'break' to exit");
			core = new ShellCore();
			Print.Listen += s => Console.WriteLine("--  " + s);

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
					code = core.Compile(sb.ToString());
				}
				catch (Exception ex)
				{
					if (core.Eof && line != "")
						continue;
					Console.WriteLine(" !  Error: " + ex.Message);
					if (ex is Error err)
						Console.WriteLine(err.Line == null ?
							" !  At line {0}." : " !  At line {0}: {1}",
							err.LineNumber, err.Line);
				}
				var statement = code.Code?.Length > 0
					&& code.Code[0] >= (byte)OpKind.Statement
					&& code.Code[0] != (byte)OpCode.Autocall;
				if (statement && line != "" && line != "return" && line != "break")
					continue;
				sb.Length = 0;
				try
				{
					core.Execute(code);
				}
				catch (Exception ex)
				{
					Console.WriteLine(" !  Error: " + ex.Message);
					if (ex is Error err)
						Console.WriteLine(err.Line == null ?
							" !  At line {0}." : " !  At line {0}: {1}",
							err.LineNumber, err.Line);
					continue;
				}
				if (core.Exit != 0)
					return;
				Console.WriteLine("=>  " + core.Result.ToStr());
			}
		}
	}
}
