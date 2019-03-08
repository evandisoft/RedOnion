using System;
using System.Reflection;

//TODO: Use FastMember or Reflection.Emit

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedType : BasicObjects.SimpleObject
	{
		/// <summary>
		/// Reflected type this object represents
		/// </summary>
		public Type Class { get; }

		/// <summary>
		/// Properties of the created object
		/// (shared via BaseProps)
		/// </summary>
		public IProperties ClassProps { get; set; }

		/// <summary>
		/// Create object (with some static properties)
		/// </summary>
		public ReflectedType(Engine engine, Type type, IProperties staticProps = null)
			: base(engine, staticProps)
			=> Class = type;

		/// <summary>
		/// Create object with both static and class/instance properties
		/// </summary>
		public ReflectedType(Engine engine, Type type,
			IProperties staticProps, IProperties classProps)
			: this(engine, type, staticProps)
			=> ClassProps = classProps;

		public override Value Call(IObject self, int argc)
			=> new Value(Create(argc));

		public override IObject Create(int argc)
		{
			if (Class.IsAbstract)
				return null;
			if (argc == 0)
			{
				var ctor = Class.GetConstructor(new Type[0]);
				if (ctor == null)
					return null;
				return new ReflectedObject(Engine, ctor.Invoke(new object[0]), this, ClassProps);
			}
			foreach (var ctor in Class.GetConstructors())
			{
				var para = ctor.GetParameters();
				if (para.Length != argc)
					continue;
				//TODO
			}
			return null;
		}

		public override IObject Which(string name)
		{
			if (BaseProps?.Has(name) == true)
				return this;
			foreach (var member in Class.GetMember(name,
				BindingFlags.IgnoreCase|BindingFlags.Static|BindingFlags.Public))
			{
				if (member is MethodInfo method)
					return this;
				if (member is FieldInfo field)
					return this;
				if (member is PropertyInfo property)
				{
					if (property.GetIndexParameters().Length > 0)
						continue;
					return this;
				}
			}
			return null;
		}

		public override bool Get(string name, out Value value)
		{
			if (base.Get(name, out value))
				return true;
			foreach (var member in Class.GetMember(name,
				BindingFlags.IgnoreCase|BindingFlags.Static|BindingFlags.Public))
			{
				if (member is MethodInfo method)
				{
					var func = new ReflectedFunction(Engine, this, name);
					if (BaseProps == null)
						BaseProps = new Properties();
					BaseProps.Set(name, value = new Value(func));
					return true;
				}
				//TODO
				if (member is FieldInfo field)
					return true;
				if (member is PropertyInfo property)
				{
					if (property.GetIndexParameters().Length > 0)
						continue;
					return true;
				}
			}
			return false;
		}
	}
}
