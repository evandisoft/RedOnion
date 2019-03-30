using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using RedOnion.Script.Parsing;
using RedOnion.Script.ReflectedObjects;

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

		public FunctionFun(IEngine engine, IObject baseClass, FunctionObj prototype)
			: base(engine, baseClass, new Properties("prototype", prototype))
			=> Prototype = prototype;

		public override Value Call(IObject self, Arguments args)
			=> new Value(Create(args));

		public override IObject Create(Arguments args)
		{
			if (args.Length == 0)
				return null;
			string arglist = null;
			for (var i = 0; i < args.Length - 1; i++)
			{
				if (arglist == null)
					arglist = args[i].String;
				else
					arglist += ", " + args[i].String;
			}
			List<ArgumentInfo> info = null;
			if (arglist != null)
			{
				var scanner = new Scanner();
				scanner.Line = arglist;
				for (;;)
				{
					if (scanner.Word == null)
						return null;
					if (info == null)
						info = new List<ArgumentInfo>();
					info.Add(new ArgumentInfo()
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
			var body = args[args.Length - 1].String;
			var code = Engine.Compile(body);
			return new FunctionObj(Engine, Prototype,
				code, 0, code.Code.Length, -1, info?.ToArray());
		}
	}

	/// <summary>
	/// Function object (callable, can construct)
	/// </summary>
	[DebuggerDisplay("function {Name} {ArgsString}")]
	[Creator(typeof(FunctionFun))]
	public class FunctionObj : BasicObject
	{
		protected string name;
		public override string Name => name ?? "anonymous";
		/// <summary>
		/// Shared code
		/// </summary>
		public CompiledCode Code { get; protected set; }
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
		public ArgumentInfo[] Arguments { get; set; }
		/// <summary>
		/// Number of declared arguments
		/// </summary>
		public int ArgumentCount => Arguments?.Length ?? 0;
		/// <summary>
		/// Get name of argument by index
		/// </summary>
		public string ArgumentName(int i)
			=> i >= ArgumentCount ? null : Arguments[i].Name;
		/// <summary>
		/// Full function code as string or at least name and arguments
		/// </summary>
		public string String { get; protected set; }
		/// <summary>
		/// Get function code as string (if enabled in engine options - opt.funcText)
		/// </summary>
		public override Value Value => new Value(String);
		/// <summary>
		/// Private variables/fields
		/// </summary>
		public IScope Scope { get; protected set; }
		/// <summary>
		/// Prototype of this function object (base class for created objects)
		/// </summary>
		public IObject Prototype
		{
			get
			{
				if (prototype == null)
					prototype = new BasicObject(Engine);
				return prototype;
			}
			protected set => prototype = value;
		}
		IObject prototype;

		public override ObjectFeatures Features
			=> ObjectFeatures.Function | ObjectFeatures.Constructor;

		/// <summary>
		/// Create Function.prototype
		/// </summary>
		public FunctionObj(IEngine engine, IObject baseClass)
			: base(engine, baseClass)
		{
		}

		/// <summary>
		/// Create new function object
		/// </summary>
		public FunctionObj(IEngine engine, FunctionObj baseClass,
			CompiledCode code, int codeAt, int codeSize, int typeAt,
			ArgumentInfo[] args, string body = null, IScope scope = null)
			:
			this(engine, null, baseClass,
			code, codeAt, codeSize, typeAt,
			args, body, scope)
		{ }
		/// <summary>
		/// Create new function object
		/// </summary>
		public FunctionObj(IEngine engine, string name, FunctionObj baseClass,
			CompiledCode code, int codeAt, int codeSize, int typeAt,
			ArgumentInfo[] args, string body = null, IScope scope = null)
			: base(engine, baseClass, StdProps)
		{
			this.name = name;
			Code = code;
			CodeAt = codeAt;
			CodeSize = codeSize;
			TypeAt = typeAt;
			Arguments = args;
			ArgsString = args == null ? "" : string.Join(", ", args.Select(x => x.Name).ToArray());
			String = body ?? (args == null ? "function " + Name
				: string.Format(Value.Culture, "function {0} {1}", Name, ArgsString));
			Scope = scope;
		}

		public override Value Call(IObject self, Arguments args)
		{
			var argobj = Engine.CreateContext(self, Scope, args.Length);
			Value result;
			try
			{
				if (Arguments != null)
				{
					for (var i = 0; i < Arguments.Length; i++)
					{
						//TODO: cast/convert to argument type + make this indexable
						argobj.Set(Arguments[i].Name, i < args.Length ? args[i] :
							Arguments[i].Value < 0 ? new Value() :
							Engine.Evaluate(Code, Arguments[i].Value));
					}
				}
				Engine.Execute(Code, CodeAt, CodeSize);
			}
			finally
			{
				result = Engine.DestroyContext();
			}
			return result;
		}

		public override IObject Create(Arguments args)
		{
			IObject baseClass = null;
			if (Get("prototype", out var proto))
				baseClass = Engine.Box(proto);
			var it = new BasicObject(Engine, baseClass);
			Call(it, args);
			return it;
		}

		public static Properties StdProps { get; } = new Properties()
		{
			{ "prototype", Value.Property<FunctionObj>(
				fn => new Value(fn.Prototype),
				(fn, value) => fn.Prototype = fn.Engine.Box(value)) }
		};

		private Dictionary<Type, Delegate> DelegateCache;
		public Delegate GetDelegate(Type type)
		{
			if (DelegateCache != null && DelegateCache.TryGetValue(type, out var value))
				return value;
			var invoke = type.GetMethod("Invoke");
			var mipars = invoke.GetParameters();
			var fnargs = new ParameterExpression[mipars.Length];
			for (int j = 0; j < mipars.Length; j++)
				fnargs[j] = Expression.Parameter(mipars[j].ParameterType, ArgumentName(j));
			var chargs = new Expression[mipars.Length];
			for (int j = 0; j < mipars.Length; j++)
				chargs[j] = Expression.Convert(fnargs[j], typeof(object));
			// (x, y, ...) => FunctionCallHelper<T>(fn, new object[] { x, y, ... })
			var lambda = Expression.Call(
				invoke.ReturnType == typeof(void)
				? typeof(FunctionObj).GetMethod("ActionCallHelper")
				: typeof(FunctionObj).GetMethod("FunctionCallHelper")
				.MakeGenericMethod(invoke.ReturnType),
				Expression.Constant(this),
				Expression.NewArrayInit(typeof(object), chargs));
			var it = Expression.Lambda(type, lambda, fnargs).Compile();
			if (DelegateCache == null)
				DelegateCache = new Dictionary<Type, Delegate>();
			DelegateCache[type] = it;
			return it;
		}

		public static void ActionCallHelper(FunctionObj fn, params object[] args)
		{
			var engine = fn.Engine;
			var arguments = engine.Arguments;
			using (arguments.Guard())
			{
				foreach (var arg in args)
					arguments.Add(ReflectedType.Convert(engine, arg));
				fn.Call(null, new Arguments(arguments, args.Length));
			}
		}
		public static T FunctionCallHelper<T>(FunctionObj fn, params object[] args)
		{
			var engine = fn.Engine;
			var arguments = engine.Arguments;
			using (arguments.Guard())
			{
				foreach (var arg in args)
					arguments.Add(ReflectedType.Convert(engine, arg));
				return ReflectedType.Convert<T>(fn.Call(null, new Arguments(arguments, args.Length)));
			}
		}
	}
}
