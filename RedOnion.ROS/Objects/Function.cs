using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RedOnion.ROS.Objects
{
	public class Function : UserObject
	{
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
		/// Context with captured variables
		/// </summary>
		public Context Context { get; protected set; }
		/// <summary>
		/// The processor this function belongs to
		/// (needed for ExecuteLater action)
		/// </summary>
		public Processor Processor { get; protected internal set; }
		/// <summary>
		/// Prototype of this function object (base class for created objects)
		/// </summary>
		public UserObject Prototype
		{
			get
			{
				if (prototype == null)
				{
					prototype = OriginalFunction == null
						? new UserObject() : OriginalFunction.Prototype;
					prop.items[0].value = new Value(prototype);
				}
				return prototype;
			}
			protected set
			{
				prototype = value;
				prop.items[0].value = new Value(prototype);
			}
		}
		UserObject prototype;

		/// <summary>
		/// Create new function object
		/// </summary>
		public Function(string name, UserObject baseClass,
			CompiledCode code, int codeAt, int codeSize, int typeAt,
			ArgumentInfo[] args, Context context, HashSet<string> cvars, Processor processor)
			: base(name ?? "lambda", typeof(Function), ExCode.Function, TypeCode.Object, baseClass)
		{
			Code = code;
			CodeAt = codeAt;
			CodeSize = codeSize;
			TypeAt = typeAt;
			Arguments = args;
			ArgsString = args == null ? "" : string.Join(", ", args.Select(x => x.Name).ToArray());
			Context = new Context(this, context, cvars);
			Processor = processor;
			Add("prototype", Value.Null);
		}

		public override bool Get(ref Value self, int at)
		{
			if (at == 0 && prototype == null)
			{
				prototype = OriginalFunction == null
					? new UserObject() : OriginalFunction.Prototype;
				prop.items[0].value = new Value(prototype);
			}
			return base.Get(ref self, at);
		}

		public override bool Call(ref Value result, object self, Arguments args, bool create)
			=> throw InvalidOperation("Function objects cannot be called via Descriptor.Call");

		Action executeLater;
		public Action ExecuteLater => executeLater
			?? (executeLater = () => Processor.Once.Add(new Value(this)));

		public void ExecuteLater1(Value arg)
			=> Processor.Once.Add(new Value(this.Bind(arg)));
		public void ExecuteLater1Gen<Arg>(Arg arg)
			=> Processor.Once.Add(new Value(this.Bind(new Value(arg))));
		static MethodInfo executeLater1 = typeof(Function).GetMethod("ExecuteLater1");
		static MethodInfo executeLater1Gen = typeof(Function).GetMethod("ExecuteLater1Gen");

		public void ExecuteLater2(Value a, Value b)
			=> Processor.Once.Add(new Value(this.Bind(a, b)));
		public void ExecuteLater2Gen<A, B>(A a, B b)
			=> Processor.Once.Add(new Value(this.Bind(new Value(a), new Value(b))));
		static MethodInfo executeLater2 = typeof(Function).GetMethod("ExecuteLater2");
		static MethodInfo executeLater2Gen = typeof(Function).GetMethod("ExecuteLater2Gen");

		public void ExecuteLater3(Value a, Value b, Value c)
			=> Processor.Once.Add(new Value(this.Bind(a, b, c)));
		public void ExecuteLater3Gen<A, B, C>(A a, B b, C c)
			=> Processor.Once.Add(new Value(this.Bind(new Value(a), new Value(b), new Value(c))));
		static MethodInfo executeLater3 = typeof(Function).GetMethod("ExecuteLater3");
		static MethodInfo executeLater3Gen = typeof(Function).GetMethod("ExecuteLater3Gen");

		public void ExecuteLater4(Value a, Value b, Value c, Value d)
			=> Processor.Once.Add(new Value(this.Bind(a, b, c, d)));
		public void ExecuteLater4Gen<A, B, C, D>(A a, B b, C c, D d)
			=> Processor.Once.Add(new Value(this.Bind(new Value(a), new Value(b), new Value(c), new Value(d))));
		static MethodInfo executeLater4 = typeof(Function).GetMethod("ExecuteLater4");
		static MethodInfo executeLater4Gen = typeof(Function).GetMethod("ExecuteLater4Gen");

		public override bool Convert(ref Value self, Descriptor to)
		{
			if (to.Type.IsSubclassOf(typeof(Delegate)))
			{
				if (to == Descriptor.Actions[0])
				{
					self = new Value(to, ExecuteLater);
					return true;
				}
				var info = to.Type.GetMethod("Invoke");
				var pars = info.GetParameters();
				if (pars.Length == 0)
				{
					self = new Value(to, ExecuteLater);
					return true;
				}
				if (pars.Length == 1)
				{
					if (pars[0].ParameterType == typeof(Value))
					{
						self = new Value(to, Delegate.CreateDelegate(to.Type, this, executeLater1));
						return true;
					}
					self = new Value(to, Delegate.CreateDelegate(to.Type, this,
						executeLater1Gen.MakeGenericMethod(pars[0].ParameterType)));
					return true;
				}
				if (pars.Length == 2)
				{
					if (pars[0].ParameterType == typeof(Value)
						&& pars[1].ParameterType == typeof(Value))
					{
						self = new Value(to, Delegate.CreateDelegate(to.Type, this, executeLater2));
						return true;
					}
					self = new Value(to, Delegate.CreateDelegate(to.Type, this,
						executeLater2Gen.MakeGenericMethod(pars[0].ParameterType, pars[1].ParameterType)));
					return true;
				}
				if (pars.Length == 3)
				{
					if (pars[0].ParameterType == typeof(Value)
						&& pars[1].ParameterType == typeof(Value)
						&& pars[2].ParameterType == typeof(Value))
					{
						self = new Value(to, Delegate.CreateDelegate(to.Type, this, executeLater3));
						return true;
					}
					self = new Value(to, Delegate.CreateDelegate(to.Type, this,
						executeLater3Gen.MakeGenericMethod(pars[0].ParameterType, pars[1].ParameterType, pars[2].ParameterType)));
					return true;
				}
				if (pars.Length == 4)
				{
					if (pars[0].ParameterType == typeof(Value)
						&& pars[1].ParameterType == typeof(Value)
						&& pars[2].ParameterType == typeof(Value)
						&& pars[3].ParameterType == typeof(Value))
					{
						self = new Value(to, Delegate.CreateDelegate(to.Type, this, executeLater4));
						return true;
					}
					self = new Value(to, Delegate.CreateDelegate(to.Type, this,
						executeLater4Gen.MakeGenericMethod(pars[0].ParameterType, pars[1].ParameterType, pars[2].ParameterType, pars[3].ParameterType)));
					return true;
				}
			}
			return base.Convert(ref self, to);
		}

		public Value[] BoundArguments { get; set; }
		public Function OriginalFunction { get; }
		public Function Bind(params Value[] args)
			=> new Function(this, args);
		public Function(Function src, params Value[] args)
			: base(src.Name, typeof(Function), ExCode.Function, TypeCode.Object, src.parent)
		{
			OriginalFunction = src.OriginalFunction ?? src;
			BoundArguments = args;
			if (src.BoundArguments != null)
			{
				BoundArguments = new Value[src.BoundArguments.Length + args.Length];
				src.BoundArguments.CopyTo(BoundArguments, 0);
				args.CopyTo(BoundArguments, src.BoundArguments.Length);
			}
			Code = src.Code;
			CodeAt = src.CodeAt;
			CodeSize = src.CodeSize;
			TypeAt = src.TypeAt;
			Arguments = src.Arguments;
			ArgsString = src.ArgsString; //TODO: remove bound args
			Context = src.Context;
			Processor = src.Processor;
			Add("prototype", Value.Null);
		}

		public class Creator : UserObject
		{
			public UserObject Prototype { get; protected set; }
			public Creator(UserObject baseClass) : base("Function", baseClass)
			{
				Add("prototype", Prototype = new UserObject(baseClass));
				Prototype.Add("bind", new Bind(baseClass));
				Prototype.Lock();
			}

			/* TODO: compile function from a string (like in JavaScript)
			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				return base.Call(ref result, self, args, create);
			}
			*/

			public class Bind : UserObject
			{
				public Bind(UserObject baseClass) : base("Function.bind", baseClass) { }
				public override bool Call(ref Value result, object self, Arguments args, bool create)
				{
					if (!create && self is Function fn)
					{
						result = new Value(fn.Bind(args.ToArray()));
						return true;
					}
					return false;
				}
			}
		}
	}
}
