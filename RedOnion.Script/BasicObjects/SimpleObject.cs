using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script.BasicObjects
{
	/// <summary>
	/// Simple implementation of IObject
	/// suitable for custom objects
	/// </summary>
	public class SimpleObject : IObject
	{
		/// <summary>
		/// Engine this object belongs to
		/// </summary>
		public Engine Engine { get; }

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
			=> new Value("[internal]");

		/// <summary>
		/// Create object with some base properties
		/// </summary>
		public SimpleObject(Engine engine, IProperties properties)
		{
			Engine = engine;
			BaseProps = properties;
		}

		public bool Has(string name)
			=> Which(name) != null;

		public virtual IObject Which(string name)
			=> BaseProps.Has(name) ? this : null;

		public Value Get(string name)
		{
			Get(name, out var value);
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
			if (value.Type == ValueKind.Create)
			{
				value = new Value(((CreateObj)value.ptr)(Engine));
				BaseProps.Set(name, value);
			}
			else if (value.Type == ValueKind.Property)
			{
				value = ((IProperty)value.ptr).Get(this);
			}
			return true;
		}

		public virtual bool Set(string name, Value value)
		{
			if (BaseProps == null)
				return false;
			if (BaseProps.Get(name, out var query))
				return false;
			if (query.Type != ValueKind.Property)
				return false;
			((IProperty)query.ptr).Set(this, value);
			return true;
		}

		public bool Delete(string name)
			=> false;

		public void Reset()
		{ }

		public virtual Value Call(IObject self, int argc)
			=> new Value();

		public virtual IObject Create(int argc)
			=> null;

		public virtual Value Index(IObject self, int argc)
		{
			switch (argc)
			{
			case 0:
				return new Value();
			case 1:
				return new Value(this, Arg(argc, 0).String);
			default:
				self = Engine.Box(new Value(this, Arg(argc, 0).String));
				return self.Index(this, argc - 1);
			}
		}

		/// <summary>
		/// Get n-th argument (for call/create implementation)
		/// </summary>
		protected Value Arg(int argc, int n = 0)
			=> Engine.Args.Arg(argc, n);
	}
}
