using RedOnion.ROS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Functions
{
	/// <summary>
	/// Run another script in its own context (given path to the script).
	/// </summary>
	public class Run : UserObject
	{
		public RunSource Source { get; }
		public RunLibrary Library { get; }
		public RunReplace Replace { get; }
		public Run(UserObject baseClass) : base("run", baseClass)
		{
			Add("source", Source = new RunSource(baseClass));
			Add("library", Library = new RunLibrary(baseClass));
			Add("replace", Replace = new RunReplace(baseClass));
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
			core.CallScript(proc.Compile(src, path));
			return true;
		}

		/// <summary>
		/// Run another script in its own context (given the script as a string).
		/// </summary>
		public class RunSource : UserObject
		{
			public RunSource(UserObject baseClass) : base("run.source", baseClass) { }
			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Length != 1)
					return false;
				var core = args.Core;
				var proc = core.Processor;
				var src = args[0].ToStr();
				core.CallScript(proc.Compile(src));
				return true;
			}
		}
		/// <summary>
		/// Run another script in the same context (given path to the script)
		/// </summary>
		public class RunLibrary : UserObject
		{
			public RunLibrarySource Source { get; }
			public RunLibrary(UserObject baseClass) : base("run.library", baseClass)
			{
				Add("source", Source = new RunLibrarySource(baseClass));
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
				core.CallScript(proc.Compile(src, path), include: true);
				return true;
			}
		}
		/// <summary>
		/// Run another script in the same context (given the script as string)
		/// </summary>
		public class RunLibrarySource : UserObject
		{
			public RunLibrarySource(UserObject baseClass) : base("run.library.source", baseClass) { }
			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Length != 1)
					return false;
				var core = args.Core;
				var proc = core.Processor;
				var src = args[0].ToStr();
				core.CallScript(proc.Compile(src), include: true);
				return true;
			}
		}

		/// <summary>
		/// Run another script in the same context (given path to the script)
		/// </summary>
		public class RunReplace : UserObject
		{
			public RunReplaceSource Source { get; }
			public RunReplace(UserObject baseClass) : base("run.replace", baseClass)
			{
				Add("source", Source = new RunReplaceSource(baseClass));
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
				core.SetCode(proc.Compile(src), reset: true);
				return true;
			}
		}
		/// <summary>
		/// Run another script in the same context (given the script as string)
		/// </summary>
		public class RunReplaceSource : UserObject
		{
			public RunReplaceSource(UserObject baseClass) : base("run.replace.source", baseClass) { }
			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Length != 1)
					return false;
				var core = args.Core;
				var proc = core.Processor;
				var src = args[0].ToStr();
				core.SetCode(proc.Compile(src), reset: true);
				return true;
			}
		}
	}
}
