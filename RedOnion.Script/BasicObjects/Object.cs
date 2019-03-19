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

		public override Value Call(IObject self, int argc)
			=> new Value(Create(argc));

		public override IObject Create(int argc)
		{
			if (argc == 0)
				return new BasicObject(Engine, Prototype);
			return Engine.Box(Engine.GetArgument(argc));
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
	public class BasicObject : IObject
	{
		/// <summary>
		/// Engine this object belongs to
		/// </summary>
		public IEngine Engine { get; }

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
		public BasicObject(IEngine engine)
		{
			Engine = engine;
		}

		/// <summary>
		/// Create empty object with base class
		/// </summary>
		public BasicObject(IEngine engine, IObject baseClass)
		{
			Engine = engine;
			BaseClass = baseClass;
		}

		/// <summary>
		/// Create object with prototype and some base properties
		/// </summary>
		public BasicObject(IEngine engine, IObject baseClass, IProperties baseProps)
		{
			Engine = engine;
			BaseClass = baseClass;
			BaseProps = baseProps;
		}

		/// <summary>
		/// Create object with prototype, some base properties and more properties
		/// </summary>
		public BasicObject(IEngine engine, IObject baseClass, IProperties baseProps, IProperties moreProps)
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
				props = obj.BaseProps;
				if (props != null && props.Get(name, out value))
					break;
				props = obj.MoreProps;
				if (props != null && props.Get(name, out value))
					break;
				if ((obj = obj.BaseClass) == null)
					return false;
			}
			if (value.Type == ValueKind.Create)
			{
				value = new Value(((CreateObject)value.ptr)(Engine));
				props.Set(name, value);
			}
			else if (value.Type == ValueKind.Property)
			{
				value = ((IProperty)value.ptr).Get(obj);
			}
			return true;
		}

		public virtual bool Set(string name, Value value)
		{
			IProperties props;
			Value query;
			for (IObject obj = this; ;)
			{
				props = obj.BaseProps;
				if (props != null && props.Get(name, out query))
				{
					if (query.Type == ValueKind.Property)
						return ((IProperty)query.ptr).Set(obj, value);
					if (obj == this)
						return false;
					break;
				}
				props = obj.MoreProps;
				if (props != null && props.Get(name, out query))
					break;
				if ((obj = obj.BaseClass) == null)
					break;
			}
			if (MoreProps == null)
				MoreProps = new Properties();
			return MoreProps.Set(name, value);
		}

		public virtual bool Modify(string name, OpCode op, Value value)
		{
			IProperties props;
			Value query;
			for (IObject obj = this; ;)
			{
				props = obj.BaseProps;
				if (props != null && props.Get(name, out query))
				{
					if (query.Type == ValueKind.Property)
					{
						var prop = (IProperty)query.ptr;
						if (prop is IPropertyEx ex)
							return ex.Modify(this, op, value);
						var tmp = prop.Get(this);
						tmp.Modify(op, value);
						return prop.Set(this, tmp);
					}
					return false;
				}
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

		public virtual Value Call(IObject self, int argc)
		{
			if (!Engine.HasOption(EngineOption.Silent))
				throw new NotImplementedException(GetType().FullName + " is not a function");
			return new Value();
		}

		public virtual IObject Create(int argc)
		{
			if (!Engine.HasOption(EngineOption.Silent))
				throw new NotImplementedException(GetType().FullName + " is not a constructor");
			return null;
		}

		public virtual Value Index(IObject self, int argc)
		{
			switch (argc)
			{
			case 0:
				return new Value();
			case 1:
				return new Value(this, Engine.GetArgument(argc, 0).String);
			default:
				self = Engine.Box(new Value(this, Engine.GetArgument(argc, 0).String));
				return self.Index(this, argc - 1);
			}
		}
	}
}
