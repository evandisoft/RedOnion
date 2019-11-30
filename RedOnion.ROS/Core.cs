using System;
using System.Diagnostics;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Parsing;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS
{
	[DebuggerDisplay("{DebugString}")]
	public partial class Core : IDisposable
	{
		public Core(Processor processor)
		{
			this.processor = processor ?? this as Processor;
			vals = new ArgumentList(this);
		}

		/// <summary>Instruction pointer (index to `code`).</summary>
		protected int at;
		/// <summary>Currently executing code.</summary>
		protected byte[] code;
		/// <summary>String table of current code.</summary>
		protected string[] str;

		/// <summary>Processor we belong to (can as well be `this`).</summary>
		protected Processor processor;
		/// <summary>Global variables.</summary>
		protected Globals globals;
		/// <summary>Value stack.</summary>
		protected ArgumentList vals;
		/// <summary>Active context (current function or script).</summary>
		protected Context ctx;
		/// <summary>Value of `this` for current code.</summary>
		protected Value self;
		/// <summary>Result when exiting.</summary>
		protected Value result;
		/// <summary>Pending exception (or Value.Void).</summary>
		protected Value error;
		/// <summary>Number of try..catch..finally blocks.</summary>
		protected int catchBlocks;

		protected struct SavedContext
		{
			public Context context;
			public Value prevSelf;
			public Value prevError;
			public CompiledCode code;
			public int at, vtop;
			public bool create;
			// run.library (context unchanged)
			public BlockCode blockCode;
			public int blockEnd;
		}
		protected ListCore<SavedContext> stack;

		public Globals Globals
		{
			get => globals;
			set => SetGlobals(value);
		}
		protected virtual void SetGlobals(Globals value)
			=> globals = value;
		public Processor Processor => processor;
		public Context Context => ctx;
		public ExitCode Exit { get; protected set; }
		public bool Paused => Exit == ExitCode.Yield || Exit == ExitCode.Countdown;
		public Value Result => result;
		public int Countdown { get; set; }
		public ArgumentList Arguments => vals;

		protected CompiledCode compiled;
		public CompiledCode Code
		{
			get => compiled;
			protected set => SetCode(value);
		}
		protected internal void SetCode(CompiledCode value, bool reset = false)
		{
			if (stack.size > 0)
			{
				if (!reset)
					ctx = stack[0].context;
				stack.Clear();
			}
			at = 0;
			code = value?.Code;
			str = value?.Strings;
			vals.Clear();
			self = Value.Null;
			Exit = ExitCode.None;
			result = Value.Void;
			error = Value.Void;
			compiled = value;
			if (ctx == null || reset)
				ctx = new Context();
			ctx.RootStart = 0;
			ctx.RootEnd = code.Length;
			ctx.PopAll();
		}

		public virtual void Log(string msg)
			=> Debug.Print(msg);
		public void Log(string msg, params object[] args)
			=> Log(string.Format(Value.Culture, msg, args));
		[Conditional("DEBUG")]
		public void DebugLog(string msg)
			=> Debug.Print(msg);
		[Conditional("DEBUG")]
		public void DebugLog(string msg, params object[] args)
			=> Log(string.Format(Value.Culture, msg, args));

		~Core() => Dispose(false);
		public void Dispose() => Dispose(true);
		protected virtual void Dispose(bool disposing) { }

		public void ResetContext()
		{
			if (stack.size > 0)
			{
				ctx = stack[0].context;
				stack.Clear();
			}
			ctx?.Reset();
		}

		/// <summary>
		/// Execute script from source. Returns true if finished
		/// (<see cref="Exit" /> is set to <see cref="ExitCode.Countdown"/>
		/// or <see cref="ExitCode.Yield"/> when returning false).
		/// </summary>
		/// <param name="script">Source code of the script</param>
		/// <param name="path">Path to the script (optional, for error tracking)</param>
		/// <param name="countdown">Countdown until auto-yield</param>
		public bool Execute(string script, string path = null, int countdown = 1000)
		{
			if (script == null) script = processor.ReadScript(path);
			Code = processor.Compile(script, path);
			if (Globals == null) Globals = new Globals();
			return Execute(countdown);
		}
		/// <summary>
		/// Execute compiled script. Returns true if finished
		/// (<see cref="Exit" /> is set to <see cref="ExitCode.Countdown"/>
		/// when returning false).
		/// </summary>
		/// <param name="code">Compiled script</param>
		/// <param name="countdown">Countdown until auto-yield</param>
		public bool Execute(CompiledCode code, int countdown = 1000)
		{
			Code = code;
			if (Globals == null) Globals = new Globals();
			return Execute(countdown);
		}
		/// <summary>
		/// Execute function without arguments. Returns true if finished
		/// (<see cref="Exit" /> is set to <see cref="ExitCode.Countdown"/>
		/// when returning false).
		/// </summary>
		/// <param name="fn">The function to execute</param>
		/// <param name="countdown">Countdown until auto-yield</param>
		public bool Execute(Function fn, int countdown = 1000)
		{
			ctx = new Context(fn, fn.Context, null);
			Code = fn.Code;
			ctx.RootStart = at = fn.CodeAt;
			ctx.RootEnd = at + fn.CodeSize;
			ctx.PopAll();
			ctx.Push(at, at + fn.CodeSize, BlockCode.Function);
			if (fn.BoundArguments == null)
				ctx.Add("arguments", new Value[0]);
			else
			{
				var args = new Value[fn.BoundArguments.Length];
				fn.BoundArguments.CopyTo(args, 0);
				ctx.Add("arguments", args);
				for (int i = 0; i < fn.ArgumentCount; i++)
				{
					if (i < args.Length)
						ctx.Add(fn.ArgumentName(i), ref args[i]);
					else ctx.Add(fn.ArgumentName(i), Value.Null);
				}
			}
			result = Value.Void;
			error = Value.Void;
			return Execute(countdown);
		}

		protected void Identifier(int at)
		{
			var code = this.code;
			int idx = Int(code, at);

			/* The following optimisation is too dangerous
			 * because sometimes slots are created at first access
			 * which then causes problems when invoking the same function
			 * or lambda/closure for second time, where the slot does not exist yet.
			 * TODO: Create OpCode.Local and let parser/compiler do the optimisation
			 * for local variables where we are 100% sure it must have been created already.

			int found;
			string name;
			if (idx >= 0)
			{
				// encountered this for the first time
				// => try to find it in local variables
				name = str[idx];
				found = ctx.Find(name);
				if (found >= 0)
				{
					!! WARNING !!
					!! exactly that needs some indicator (e.g. int Find(string, out bool local))
					!! becase found is not automatically local (see closures)

					// we found it to be local, mark it as such
					// and embed known index to speed things up
					// TODO: do that in parser/compiler
					idx = ~found;
					code[at++] = (byte)idx;
					code[at++] = (byte)(idx >> 8);
					code[at++] = (byte)(idx >> 16);
					code[at++] = (byte)(idx >> 24);
					ref var local = ref vals.Push();
					local.obj = local.desc = ctx;
					local.num = new Value.NumericData(found, idx);
					return;
				}
				// not local - the index may be changing,
				// but at least skip the search to this+globals
				// note: the method could be transfered to other objects
				// making this-index not reliable (and globals as well - libs)
				idx = ~(idx | 0x40000000);
				code[at++] = (byte)idx;
				code[at++] = (byte)(idx >> 8);
				code[at++] = (byte)(idx >> 16);
				code[at++] = (byte)(idx >> 24);
				// note: neither idx nor found is used later, only the name
				// TODO: use similar bound-index approach for strong types
			}
			else
			{
				// not the first time, let us decode it
				found = ~idx;
				if ((found & 0x40000000) == 0)
				{
					// local, marked previously, this speeds up loops
					ref var local = ref vals.Push();
					local.obj = local.desc = ctx;
					local.num = new Value.NumericData(found, idx);
					return;
				}
				// get back the original index and load the name
				idx = found & 0x3FFFFFFF;
				name = str[idx];
			}
			*/

			string name = str[idx];
			int found = ctx.Find(name);
			// if local (or tracked reference in closure)
			if (found >= 0)
			{
				ref var it = ref vals.Push();
				it.obj = it.desc = ctx;
				it.num = new Value.NumericData(found, ~found);
				return;
			}
			// try `this` first
			if (self.obj != null
			&& (found = self.desc.Find(self.obj, name, false)) >= 0)
			{
				ref var it = ref vals.Push();
				it.desc = self.desc;
				it.obj = self.obj;
				it.num = new Value.NumericData(found, ~found);
				return;
			}
			// try globals last
			if ((found = Globals?.Find(name) ?? -1) >= 0)
			{
				ref var it = ref vals.Push();
				it.obj = it.desc = Globals;
				it.num = new Value.NumericData(found, ~found);
				return;
			}
			throw InvalidOperation("Variable '{0}' not found", name);
		}

		protected void Dereference(int argc)
		{
			for (int i = 0; i < argc; i++)
			{
				ref var arg = ref vals.Top(i - argc);
				if (arg.IsReference && !arg.desc.Get(ref arg, arg.num.Int))
					throw CouldNotGet(ref arg);
			}
		}

		protected void Call(int argc, bool create, OpCode op = OpCode.Void)
		{
			if (argc > 0)
				Dereference(argc);
			object self = null;
			Descriptor selfDesc = null;
			int idx = -1;
			ref var it = ref vals.Top(-argc-1);
			if (it.IsReference)
			{
				selfDesc = it.desc;
				self = it.obj;
				idx = it.num.Int;
				if (!it.desc.Get(ref it, idx))
					throw CouldNotGet(ref it);
			}
			if (it.IsFunction)
			{
				var fn = (Function)it.desc;
				ref var ret = ref stack.Add();
				ret.context = ctx;
				ret.prevSelf = this.self;
				ret.prevError = error;
				ret.code = compiled;
				ret.at = at;
				ret.vtop = vals.Size - argc;
				ret.create = create;
				compiled = fn.Code;
				code = compiled.Code;
				str = compiled.Strings;
				at = fn.CodeAt;
				ctx = new Context(fn, fn.Context, null);
				if (create)
					this.self = new Value(new UserObject(fn.Prototype));
				else this.self = new Value(selfDesc, self);
				var args = new Value[argc];
				for (int i = 0; i < argc; i++)
					args[i] = vals.Top(i - argc);
				if (fn.BoundArguments != null)
				{
					var combined = new Value[fn.BoundArguments.Length + args.Length];
					fn.BoundArguments.CopyTo(combined, 0);
					args.CopyTo(combined, fn.BoundArguments.Length);
					args = combined;
					argc += fn.BoundArguments.Length;
				}
				ctx.Add("arguments", args);
				for (int i = 0; i < fn.ArgumentCount; i++)
				{
					if (i < argc)
						ctx.Add(fn.ArgumentName(i), ref args[i]);
					else ctx.Add(fn.ArgumentName(i), Value.Null);
				}
				result = Value.Void;
				error = Value.Void;
				return;
			}
#if DEBUG
			bool wasReplace = it.desc is Functions.Run.RunReplace || it.desc is Functions.Run.RunReplaceSource;
#endif
			if (!it.desc.Call(ref it, self, new Arguments(Arguments, argc), create)
				&& op != OpCode.Autocall)
			{
				throw InvalidOperation(create ? op == OpCode.Identifier
					? "Could not create new {0}" : argc == 0
					? "{0} cannot create object given zero arguments" : argc == 1
					? "{0} cannot create object given that argument"
					: "{0} cannot create object given these arguments" : argc == 0
					? "{0} cannot be called with zero arguments" : argc == 1
					? "{0} cannot be called with that argument"
					: "{0} cannot be called with these two arguments",
					self != null ? selfDesc.NameOf(self, idx) : it.Name);
			}
#if DEBUG
			Debug.Assert(argc <= vals.Count || wasReplace);
#endif
			vals.Pop(Math.Min(argc, vals.Count));
		}
		// see Functions.Run
		internal void CallScript(CompiledCode script, bool include = false)
		{
			ref var ret = ref stack.Add();
			ret.context = ctx;
			ret.prevSelf = self;
			ret.prevError = error;
			ret.code = compiled;
			ret.at = at;
			ret.vtop = vals.Size - 1;
			ret.create = false;
			compiled = script;
			code = compiled.Code;
			str = compiled.Strings;
			at = 0;
			if (include)
			{
				ret.blockCode = ctx.BlockCode;
				ret.blockEnd = ctx.BlockEnd;
				ctx.BlockEnd = code.Length;
				// see Execute(int): `while (at == blockEnd)` at the top
				ctx.BlockCode = BlockCode.Library;
			}
			else ctx = new Context() { RootEnd = code.Length };
			result = Value.Void;
			error = Value.Void;
			vals.GetRef(2, 0) = Value.Void;
		}

		public static unsafe float Float(byte[] code, int at)
		{
			var v = Int(code, at);
			return *(float*)&v;
		}
		public static unsafe double Double(byte[] code, int at)
		{
			var v = Long(code, at);
			return *(double*)&v;
		}
		public static long Long(byte[] code, int at)
		{
			uint lo = (uint)Int(code, at);
			int hi = Int(code, at + 4);
			return lo | ((long)hi << 32);
		}
		public static int Int(byte[] code, int at)
		{
			int v = code[at++];
			v |= code[at++] << 8;
			v |= code[at++] << 16;
			return v | (code[at++] << 24);
		}
		public static short Short(byte[] code, int at)
		{
			int v = code[at++];
			return (short)(v | (code[at++] << 8));
		}

		static internal InvalidOperationException InvalidOperation(string msg)
			=> new InvalidOperationException(msg);
		static internal InvalidOperationException InvalidOperation(string msg, params object[] args)
			=> new InvalidOperationException(string.Format(Value.Culture, msg, args));
		static internal InvalidOperationException CouldNotGet(ref Value it)
			=> new InvalidOperationException(string.Format(Value.Culture,
			"Could not get '{0}' of '{1}'", it.desc.NameOf(it.obj, it.num.Int), it.desc.Name));

		private string DebugString
		{
			get
			{
				var code = this.compiled;
				if (code == null)
					return "null";
				var lineMap = code.LineMap;
				if (lineMap == null || lineMap.Length == 0)
					return "no line map";
				var lines = code.Lines;
				if (lines == null)
					return "no lines";
				int i = Array.BinarySearch(lineMap, at - 1);
				if (i < 0)
				{
					i = ~i;
					if (i > 0)
						i--;
				}
				return i >= 0 && lines != null && i < lines.Count ? lines[i].ToString() : "#" + i;
			}
		}
	}
}
