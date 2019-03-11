using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RedOnion.Script.Parsing;

namespace RedOnion.Script.BasicObjects
{
	/// <summary>
	/// Function for creating functions
	/// </summary>
	public class FunctionFun : BasicObject
	{
		/// <summary>
		/// Prototype of all function objects
		/// </summary>
		public FunctionObj Prototype { get; }

		public override ObjectFeatures Features
			=> ObjectFeatures.Function | ObjectFeatures.Constructor;

		public FunctionFun(Engine engine, IObject baseClass, FunctionObj prototype)
			: base(engine, baseClass, new Properties("prototype", prototype))
			=> Prototype = prototype;

		public override Value Call(IObject self, int argc)
			=> new Value(Create(argc));

		public override IObject Create(int argc)
		{
			if (argc == 0)
				return null;
			string arglist = null;
			for (var i = 0; i < (argc - 1); i++)
			{
				if (arglist == null)
					arglist = Arg(argc, i).String;
				else
					arglist += ", " + Arg(argc, i).String;
			}
			List<Engine.ArgInfo> args = null;
			if (arglist != null)
			{
				var scanner = new Scanner();
				scanner.Line = arglist;
				for (;;)
				{
					if (scanner.Word == null)
						return null;
					if (args == null)
						args = new List<Engine.ArgInfo>();
					args.Add(new Engine.ArgInfo()
					{
						Name = scanner.Word,
						Type = -1,
						Value = -1
					});
					if (scanner.Next().Eol)
						scanner.NextLine();
					if (scanner.Eof)
						break;
					if (scanner.Curr != ',')
						return null;
					if (scanner.Next().Eol)
						scanner.NextLine();
					if (scanner.Eof)
						break;
				}
			}
			var body = Arg(argc, argc - 1).String;
			var code = Engine.Compile(body, out var strings);
			return new FunctionObj(Engine, Prototype,
				strings, code, 0, code.Length, -1, args?.ToArray(),
				(Engine.Options & Engine.Option.FuncText) == 0 ? null :
				"function anonymous(" +
				(args == null ? "" : string.Join(", ", args.Select(x => x.Name).ToArray())) +
				") {\n" + body + "\n}");
		}
	}

	/// <summary>
	/// Function object (callable, can construct)
	/// </summary>
	[DebuggerDisplay("{GetType().Name}: {String}")]
	public class FunctionObj : BasicObject
	{
		/// <summary>
		/// Shared string table
		/// </summary>
		public string[] Strings { get; protected set; }
		/// <summary>
		/// Shared code
		/// </summary>
		public byte[] Code { get; protected set; }
		/// <summary>
		/// Function code position
		/// </summary>
		public int CodeAt { get; protected set; }
		/// <summary>
		/// Function code size
		/// </summary>
		public int CodeSize { get; protected set; }
		/// <summary>
		/// Function type code position
		/// </summary>
		public int TypeAt { get; protected set; }
		/// <summary>
		/// Comma-separated list of argument names
		/// </summary>
		public string ArgsString { get; protected set; }
		/// <summary>
		/// Array of argument names and values (will be null if empty)
		/// </summary>
		protected Engine.ArgInfo[] Args { get; set; }
		/// <summary>
		/// Number of declared arguments
		/// </summary>
		public int ArgCount => Args?.Length ?? 0;
		/// <summary>
		/// Get name of argument by index
		/// </summary>
		public string ArgName(int i)
			=> i >= ArgCount ? null : Args[i].Name;
		/// <summary>
		/// Full function code as string
		/// </summary>
		public string String { get; protected set; }
		/// <summary>
		/// Get function code as string (if enabled in engine options - opt.funcText)
		/// </summary>
		public override Value Value => new Value(String);
		/// <summary>
		/// Private variables/fields
		/// </summary>
		public IObject Scope { get; protected set; }

		public override ObjectFeatures Features
			=> ObjectFeatures.Function | ObjectFeatures.Constructor;

		/// <summary>
		/// Create Function.prototype
		/// </summary>
		public FunctionObj(Engine engine, IObject baseClass)
			: base(engine, baseClass)
		{
		}

		/// <summary>
		/// Create new function object
		/// </summary>
		public FunctionObj(Engine engine, FunctionObj baseClass,
			string[] strings, byte[] code, int codeAt, int codeSize, int typeAt,
			Engine.ArgInfo[] args, string body = null, IObject scope = null)
			: base(engine, baseClass, StdProps)
		{
			Strings = strings;
			Code = code;
			CodeAt = codeAt;
			CodeSize = codeSize;
			TypeAt = typeAt;
			Args = args;
			ArgsString = args == null ? "" : string.Join(", ", args.Select(x => x.Name).ToArray());
			String = body ?? "function";
			Scope = scope;
		}

		public override Value Call(IObject self, int argc)
		{
			CreateContext(self, Scope);
			var args = Ctx.Vars.BaseClass;
			if (Args != null)
			{
				for (var i = 0; i < Args.Length; i++)
				{
					//TODO: cast/convert to argument type
					args.Set(Args[i].Name, i < argc ? Arg(argc, i) :
						Args[i].Value < 0 ? new Value() :
						Engine.Expression(Strings, Code, Args[i].Value).Result);
				}
			}
			Engine.Execute(Strings, Code, CodeAt, CodeSize);
			return DestroyContext();
		}

		public override IObject Create(int argc)
		{
			var it = new BasicObject(Engine, Engine.Box(Get("prototype")));
			CreateContext(it, Scope);
			var args = Ctx.Vars.BaseClass;
			if (this.Args != null)
			{
				for (var i = 0; i < this.Args.Length; i++)
				{
					//TODO: cast/convert to argument type
					args.Set(Args[i].Name, i < argc ? Arg(argc, i) :
						Args[i].Value < 0 ? new Value() :
						Engine.Expression(Strings, Code, Args[i].Value).Result);
				}
			}
			Engine.Execute(Strings, Code, CodeAt, CodeSize);
			DestroyContext();
			return it;
		}

		public static Properties StdProps { get; } = new Properties();
	}
}
