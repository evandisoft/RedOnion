using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script.BasicObjects
{
	/// <summary>
	/// Simple implementation of IObject suitable for custom objects.
	/// Does not allow to add new properties by script.
	/// </summary>
	public class SimpleObject : IObject
	{
		/// <summary>
		/// Engine this object belongs to
		/// </summary>
		public IEngine Engine { get; }

		/// <summary>
		/// Name of the object (or full name of the type)
		/// </summary>
		public virtual string Name => GetType().Name;

		/// <summary>
		/// No base class
		/// </summary>
		public IObject BaseClass => null;

		/// <summary>
		/// Basic properties - not enumerable, not writable unless iProp with set returning true
		/// </summary>
		public IProperties BaseProps { get; protected set; }

		/// <summary>
		/// Added properties - none in simple object
		/// </summary>
		public IProperties MoreProps => null;

		public virtual Value Value
			=> new Value(GetType().FullName);
		public virtual ObjectFeatures Features
			=> ObjectFeatures.None;
		public virtual Type Type => null;
		public virtual object Target => null;
		public virtual IObject Convert(object value) => null;

		/// <summary>
		/// Create object with some base properties
		/// </summary>
		public SimpleObject(IEngine engine, IProperties properties)
		{
			Engine = engine;
			BaseProps = properties;
		}

		public virtual bool Has(string name)
			=> BaseProps.Has(name);

		public Value Get(string name)
		{
			if (!Get(name, out var value) && Engine?.HasOption(EngineOption.Silent) != false)
				throw new NotImplementedException(name + " does not exist");
			return value;
		}

		public virtual bool Get(string name, out Value value)
		{
			if (BaseProps == null)
			{
				value = new Value();
				return false;
			}
			if (!BaseProps.Get(name, out value))
				return false;
			if (value.Kind == ValueKind.Create)
			{
				value = new Value(((CreateObject)value.ptr)(Engine), value.flag);
				BaseProps.Set(name, value);
			}
			else if (value.IsProperty)
			{
				value = value.Get(this);
			}
			return true;
		}

		public virtual bool Set(string name, Value value)
		{
			if (BaseProps == null)
				return false;
			if (!BaseProps.Get(name, out var query))
				return false;
			return query.Set(this, value);
		}

		public virtual bool Modify(string name, OpCode op, Value value)
		{
			if (BaseProps == null)
				return false;
			if (!BaseProps.Get(name, out var query))
				return false;
			return query.Modify(this, op, value);
		}

		public bool Delete(string name)
			=> false;

		public void Reset()
		{ }

		public virtual Value Call(IObject self, Arguments args)
			=> new Value();

		public virtual IObject Create(Arguments args)
			=> null;

		public virtual Value Index(Arguments args)
		{
			switch (args.Length)
			{
			case 0:
				return new Value();
			case 1:
				return Value.IndexRef(this, args[0]);
			default:
				return Engine.Box(IndexGet(args[0])).Index(new Arguments(args, args.Length - 1));
			}
		}
		public virtual Value IndexGet(Value index) => Get(index.String);
		public virtual bool IndexSet(Value index, Value value) => Set(index.String, value);
		public virtual bool IndexModify(Value index, OpCode op, Value value) => Modify(index.String, op, value);
		public virtual bool Operator(OpCode op, Value arg, bool selfRhs, out Value result)
		{
			result = new Value();
			return false;
		}
	}
}
