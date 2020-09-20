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
		public RunOnce Once { get; }
		public RunInclude Include { get; }
		public RunReplace Replace { get; }
		public Run(UserObject baseClass) : base("run", baseClass)
		{
			Add("source", Source = new RunSource(baseClass));
			Add("once", Once = new RunOnce(baseClass));
			Add("include", Include = new RunInclude(baseClass));
			Add("replace", Replace = new RunReplace(baseClass));

			Add("library", Include); // older alias
			Lock();
		}
		public override void Reset()
		{
			Source.Reset();
			Once.Reset();
			Include.Reset();
			base.Reset();
		}
		public override bool Call(ref Value result, object self, in Arguments args)
		{
			if (args.Length != 1)
				return false;
			var core = args.Core;
			var proc = core.Processor;
			var path = args[0].ToStr();
			var src = proc.ReadScript(path);
			if (src == null) throw new InvalidOperation("Could not read " + path);
			core.CallScript(proc.Compile(src, path));
			return true;
		}

		/// <summary>
		/// Run another script in its own context (given the script as a string).
		/// </summary>
		public class RunSource : UserObject
		{
			public RunSource(UserObject baseClass) : base("run.source", baseClass) { }
			public override bool Call(ref Value result, object self, in Arguments args)
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
		/// Run only once (manages set of paths and does not run scripts twice).
		/// </summary>
		public class RunOnce : UserObject
		{
			public readonly HashSet<string> packages = new HashSet<string>();
			public RunOnce(UserObject baseClass) : base("run.once", baseClass) { }
			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length != 1)
					return false;
				var path = args[0].ToStr();
				if (packages.Contains(path))
				{
					result = Value.False;
					return true;
				}
				var core = args.Core;
				var proc = core.Processor;
				var src = proc.ReadScript(path);
				if (src == null) throw new InvalidOperation("Could not read " + path);
				core.CallScript(proc.Compile(src, path));
				packages.Add(path);
				return true;
			}
		}

		/// <summary>
		/// Run another script in the same context (given path to the script)
		/// </summary>
		public class RunInclude : UserObject
		{
			public RunIncludeSource Source { get; }
			public RunInclude(UserObject baseClass) : base("run.include", baseClass)
			{
				Add("source", Source = new RunIncludeSource(baseClass));
				Lock();
			}
			public override void Reset()
			{
				Source.Reset();
				base.Reset();
			}
			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length != 1)
					return false;
				var core = args.Core;
				var proc = core.Processor;
				var path = args[0].ToStr();
				var src = proc.ReadScript(path);
				if (src == null) throw new InvalidOperation("Could not read " + path);
				core.CallScript(proc.Compile(src, path), include: true);
				return true;
			}
		}
		/// <summary>
		/// Run another script in the same context (given the script as string)
		/// </summary>
		public class RunIncludeSource : UserObject
		{
			public RunIncludeSource(UserObject baseClass) : base("run.include.source", baseClass) { }
			public override bool Call(ref Value result, object self, in Arguments args)
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
			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length != 1)
					return false;
				var core = args.Core;
				var proc = core.Processor;
				var path = args[0].ToStr();
				var src = proc.ReadScript(path);
				if (src == null) throw new InvalidOperation("Could not read " + path);
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
			public override bool Call(ref Value result, object self, in Arguments args)
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
