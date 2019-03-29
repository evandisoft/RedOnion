using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.Script.BasicObjects
{
	/// <summary>
	/// Object function (used to create new objects)
	/// </summary>
	public class ObjectFun : BasicObject
	{
		/// <summary>
		/// Prototype of all objects
		/// </summary>
		public IObject Prototype { get; }

		public ObjectFun(IEngine engine, IObject baseClass, IObject prototype)
			: base(engine, baseClass, new Properties("prototype", prototype))
			=> Prototype = prototype;

		public override Value Call(IObject self, Arguments args)
			=> new Value(Create(args));

		public override IObject Create(Arguments args)
		{
			if (args.Length == 0)
				return new BasicObject(Engine, Prototype);
			return Engine.Box(args[0]);
		}
	}

	/// <summary>
	/// Basic object created by `new object`
	/// </summary>
	/// <remarks>
	/// New properties can be added in runtime
	/// and also accessed by indexing (by name)
	/// which can be used like string-keyed dictionary.
	/// </remarks>
	[DebuggerDisplay("{Name}")]
	public class BasicObject : BasicObject<IEngine>
	{
		/// <summary>
		/// Create empty object with no base class
		/// </summary>
		public BasicObject(IEngine engine)
			: base(engine) { }
		/// <summary>
		/// Create empty object with base class
		/// </summary>
		public BasicObject(IEngine engine, IObject baseClass)
			: base(engine, baseClass) { }
		/// <summary>
		/// Create object with prototype and some base properties
		/// </summary>
		public BasicObject(IEngine engine, IObject baseClass, IProperties baseProps)
			: base(engine, baseClass, baseProps) { }
		/// <summary>
		/// Create object with prototype, some base properties and more properties
		/// </summary>
		public BasicObject(IEngine engine, IObject baseClass, IProperties baseProps, IProperties moreProps)
			: base(engine, baseClass, baseProps, moreProps) { }
	}
	[DebuggerDisplay("{Name}")]
	public class BasicObject<TEngine> : IScope where TEngine: IEngine
	{
		/// <summary>
		/// Engine this object belongs to
		/// </summary>
		public TEngine Engine { get; }
		IEngine IObject.Engine => Engine;

		/// <summary>
		/// Name of the object (or full name of the type)
		/// </summary>
		public virtual string Name => GetType().FullName;

		/// <summary>
		/// Base class (to search properties in this object next)
		/// </summary>
		public IObject BaseClass { get; }

		/// <summary>
		/// Basic properties - not enumerable, not writable unless IProperty with set returning true
		/// </summary>
		public IProperties BaseProps { get; protected set; }

		/// <summary>
		/// Added properties - enumerable and writable (unless same exist in baseProps)
		/// </summary>
		public IProperties MoreProps { get; protected set; }

		public virtual Value Value
			=> new Value(GetType().FullName);
		public virtual ObjectFeatures Features
			=> ObjectFeatures.Collection;
		public virtual Type Type => null;
		public virtual object Target => null;

		public virtual IObject Convert(object value) => null;

		/// <summary>
		/// Create empty object with no base class
		/// </summary>
		public BasicObject(TEngine engine)
		{
			Engine = engine;
		}

		/// <summary>
		/// Create empty object with base class
		/// </summary>
		public BasicObject(TEngine engine, IObject baseClass)
		{
			Engine = engine;
			BaseClass = baseClass;
		}

		/// <summary>
		/// Create object with prototype and some base properties
		/// </summary>
		public BasicObject(TEngine engine, IObject baseClass, IProperties baseProps)
		{
			Engine = engine;
			BaseClass = baseClass;
			BaseProps = baseProps;
		}

		/// <summary>
		/// Create object with prototype, some base properties and more properties
		/// </summary>
		public BasicObject(TEngine engine, IObject baseClass, IProperties baseProps, IProperties moreProps)
		{
			Engine = engine;
			BaseClass = baseClass;
			BaseProps = baseProps;
			MoreProps = moreProps;
		}

		public bool Has(string name)
			=> Which(name) != null;

		public virtual IObject Which(string name)
		{
			IProperties props;
			for (IObject obj = this; ;)
			{
				props = obj.BaseProps;
				if (props != null && props.Has(name))
					return obj;
				props = obj.MoreProps;
				if (props != null && props.Has(name))
					return obj;
				if ((obj = obj.BaseClass) == null)
					return null;
			}
		}

		public Value Get(string name)
		{
			if (!Get(name, out var value) && !Engine.HasOption(EngineOption.Silent))
				throw new NotImplementedException(name + " does not exist");
			return value;
		}

		public virtual bool Get(string name, out Value value)
		{
			value = new Value();
			IProperties props;
			IObject obj = this;
			for (; ; )
			{
				props = obj.MoreProps;
				if (props != null && props.Get(name, out value))
					break;
				props = obj.BaseProps;
				if (props != null && props.Get(name, out value))
					break;
				if ((obj = obj.BaseClass) == null)
					return false;
			}
			if (value.Kind == ValueKind.Create)
			{
				value = new Value(((CreateObject)value.ptr)(Engine), value.flag);
				props.Set(name, value);
			}
			else if (value.IsProperty)
			{
				value = value.Get(obj);
			}
			return true;
		}

		public virtual bool Set(string name, Value value)
		{
			IProperties props;
			Value query;
			for (IObject obj = this; ;)
			{
				props = obj.MoreProps;
				if (props != null && props.Get(name, out query))
					break;
				props = obj.BaseProps;
				if (props != null && props.Get(name, out query))
				{
					if (query.IsProperty)
						return query.Set(obj, value);
					if (obj == this)
						return false;
					break;
				}
				if ((obj = obj.BaseClass) == null)
					break;
			}
			if (MoreProps == null)
				MoreProps = new Properties();
			return MoreProps.Set(name, value);
		}
		public void Add(string name, Value value)
		{
			if (Engine.HasOption(EngineOption.Strict) && BaseProps?.Has(name) == true)
				throw new InvalidOperationException("Cannot shadow " + name + " in " + Name);
			if (MoreProps == null)
				MoreProps = new Properties();
			MoreProps.Set(name, value.RValue);
		}

		public virtual bool Modify(string name, OpCode op, Value value)
		{
			IProperties props;
			Value query;
			for (IObject obj = this; ;)
			{
				props = obj.BaseProps;
				if (props != null && props.Get(name, out query))
					return query.Modify(this, op, value);
				props = obj.MoreProps;
				if (props != null && props.Get(name, out query))
				{
					query.Modify(op, value);
					return props.Set(name, query);
				}
				if ((obj = obj.BaseClass) == null)
					return false;
			}
		}


		public bool Delete(string name)
			=> MoreProps == null ? false : MoreProps.Delete(name);

		public virtual void Reset()
			=> MoreProps = null;

		public virtual Value Call(IObject self, Arguments args)
		{
			if (!Engine.HasOption(EngineOption.Silent))
				throw new NotImplementedException(GetType().FullName + " is not a function");
			return new Value();
		}

		public virtual IObject Create(Arguments args)
		{
			if (!Engine.HasOption(EngineOption.Silent))
				throw new NotImplementedException(GetType().FullName + " is not a constructor");
			return null;
		}

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
