using System;
using System.Collections.Generic;
using System.Linq;
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
		public Value ArgumentDefault(int i)
			=> throw new NotImplementedException();
		/// <summary>
		/// Private variables/fields
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
					prototype = new UserObject();
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
			Context = new Context(context, this, cvars);
			Processor = processor;
			Add("prototype", Value.Null);
		}

		public override bool Get(ref Value self, int at)
		{
			if (at == 0 && prototype == null)
			{
				prototype = new UserObject();
				prop.items[0].value = new Value(prototype);
			}
			return base.Get(ref self, at);
		}

		public override bool Call(ref Value result, object self, Arguments args, bool create)
			=> throw InvalidOperation("Function objects cannot be called via Descriptor.Call");

		Action executeLater;
		public Action ExecuteLater => executeLater
			?? (executeLater = () => Processor.OneShot.Add(this));
		public override bool Convert(ref Value self, Descriptor to)
		{
			if (to == Descriptor.Actions[0])
			{
				self = new Value(ExecuteLater);
				return true;
			}
			return base.Convert(ref self, to);
		}
	}
}
