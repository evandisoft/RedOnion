using RedOnion.ROS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Functions
{
	/// <summary>
	/// Run another script in its own context
	/// </summary>
	public class Run : UserObject
	{
		public RunSource Source { get; } = new RunSource();
		public RunLibrary Library { get; } = new RunLibrary();
		public Run() : base("run")
		{
			Add("source", Source);
			Add("library", Library);
			Lock();
		}
		public override void Reset()
		{
			Source.Reset();
			Library.Reset();
			base.Reset();
		}
		public override bool Call(ref Value result, object self, Arguments args, bool create)
		{
			if (args.Length != 1)
				return false;
			var core = args.Core;
			var proc = core.Processor;
			var path = args[0].ToStr();
			var src = proc.ReadScript(path);
			if (src == null) throw InvalidOperation("Could not read " + path);
			core.CallScript(proc.Compile(src, path), 0); //TODO: args
			return true;
		}

		public class RunSource : UserObject
		{
			public RunSource() : base("run.source") { }
			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Length != 1)
					return false;
				var core = args.Core;
				var proc = core.Processor;
				var src = args[0].ToStr();
				core.CallScript(proc.Compile(src), 0); //TODO: args
				return true;
			}
		}
		public class RunLibrary : UserObject
		{
			public RunLibrarySource Source { get; } = new RunLibrarySource();
			public RunLibrary() : base("run.library")
			{
				Add("source", Source);
				Lock();
			}
			public override void Reset()
			{
				Source.Reset();
				base.Reset();
			}
			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Length != 1)
					return false;
				var core = args.Core;
				var proc = core.Processor;
				var path = args[0].ToStr();
				var src = proc.ReadScript(path);
				if (src == null) throw InvalidOperation("Could not read " + path);
				core.CallScript(proc.Compile(src, path), 0, include: true); //TODO: args
				return true;
			}
		}
		public class RunLibrarySource : UserObject
		{
			public RunLibrarySource() : base("run.library.source") { }
			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Length != 1)
					return false;
				var core = args.Core;
				var proc = core.Processor;
				var src = args[0].ToStr();
				core.CallScript(proc.Compile(src), 0, include: true); //TODO: args
				return true;
			}
		}
	}
}
