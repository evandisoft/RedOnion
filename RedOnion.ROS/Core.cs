using System;
using System.Diagnostics;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Parsing;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS
{
	[DebuggerDisplay("{DebugString}")]
	public partial class Core : ICore
	{
		public Core() { }
		public Core(UserObject globals) => Globals = globals;

		protected int at;
		protected byte[] code;
		protected string[] str;
		protected ArgumentList vals = new ArgumentList();
		protected Context ctx = new Context();
		protected Value self; // `this` for current code
		protected Value result;
		protected struct SavedContext
		{
			public Context context;
			public Value prevSelf;
			public CompiledCode code;
			public int at, vtop;
			public bool create;
		}
		protected ListCore<SavedContext> stack;

		public UserObject Globals { get; set; }
		public ExitCode Exit { get; protected set; }
		public Value Result => result;
		public int Countdown { get; set; }
		public ArgumentList Arguments => vals;

		protected CompiledCode compiled;
		public CompiledCode Code
		{
			get => compiled;
			protected set
			{
				at = 0;
				code = value?.Code;
				str = value?.Strings;
				vals.Clear();
				self = Value.Null;
				Exit = ExitCode.None;
				result = Value.Void;
				compiled = value;
				ctx.RootStart = 0;
				ctx.RootEnd = code.Length;
			}
		}

		protected Parser Parser { get; } = new Parser();
		public CompiledCode Compile(string source, string path = null)
			=> Parser.Compile(source, path);

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

		public void ResetContext() => ctx.Reset();
		public bool Execute(string script, string path = null, int countdown = 1000)
		{
			Code = Compile(script, path);
			if (Globals == null) Globals = new Globals();
			return Execute(countdown);
		}
		public bool Execute(CompiledCode code, int countdown = 1000)
		{
			Code = code;
			if (Globals == null) Globals = new Globals();
			return Execute(countdown);
		}
		public bool Execute(Function fn, int countdown = 1000)
		{
			Code = fn.Code;
			ctx = fn.Context;
			ctx.Push(at, at + fn.CodeSize, OpCode.Function);
			ctx.Add("arguments", new Value[0]);
			result = Value.Void;
			ctx.RootStart = at = fn.CodeAt;
			ctx.RootEnd = at + fn.CodeSize;
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

		protected int CallFunction(Function fn, Descriptor selfDesc, object self, int argc, bool create)
		{
			ref var ret = ref stack.Add();
			ret.context = ctx;
			ret.prevSelf = this.self;
			ret.code = compiled;
			ret.at = at;
			ret.vtop = vals.Size - argc;
			ret.create = create;
			compiled = fn.Code;
			code = compiled.Code;
			str = compiled.Strings;
			at = fn.CodeAt;
			ctx = fn.Context;
			ctx.Push(at, at + fn.CodeSize, OpCode.Function);
			if (create)
				this.self = new Value(new UserObject(fn.Prototype));
			else this.self = new Value(selfDesc, self);
			var args = new Value[argc];
			for (int i = 0; i < argc; i++)
				args[i] = vals.Top(i - argc);
			ctx.Add("arguments", args);
			for (int i = 0; i < fn.ArgumentCount; i++)
			{
				if (i < argc)
					ctx.Add(fn.ArgumentName(i), ref args[i]);
				else ctx.Add(fn.ArgumentName(i), fn.ArgumentDefault(i));
			}
			result = Value.Void;
			return at + fn.CodeSize;
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
