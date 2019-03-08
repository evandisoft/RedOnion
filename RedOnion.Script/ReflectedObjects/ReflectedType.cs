using System;
using System.Collections.Generic;
using System.Reflection;

//TODO: Use FastMember or Reflection.Emit

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedType : BasicObjects.SimpleObject
	{
		/// <summary>
		/// Reflected type this object represents
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Properties of the created object
		/// (shared via BaseProps)
		/// </summary>
		public IProperties TypeProps { get; set; }

		/// <summary>
		/// Create object (with some static properties)
		/// </summary>
		public ReflectedType(Engine engine, Type type, IProperties staticProps = null)
			: base(engine, staticProps)
			=> Type = type;

		/// <summary>
		/// Create object with both static and class/instance properties
		/// </summary>
		public ReflectedType(Engine engine, Type type,
			IProperties staticProps, IProperties typeProps)
			: this(engine, type, staticProps)
			=> TypeProps = typeProps;

		public override Value Call(IObject self, int argc)
			=> new Value(Create(argc));

		public override IObject Create(int argc)
		{
			if (Type.IsAbstract)
				return null;
			if (argc == 0)
			{
				var ctor = Type.GetConstructor(new Type[0]);
				if (ctor == null)
					return null;
				return new ReflectedObject(Engine, ctor.Invoke(new object[0]), this, TypeProps);
			}
			foreach (var ctor in Type.GetConstructors())
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
			foreach (var member in Type.GetMember(name,
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
			var members = Type.GetMember(name,
				BindingFlags.IgnoreCase|BindingFlags.Static|BindingFlags.Public);
			for (int i = 0; i < members.Length;)
			{
				var member = members[i++];
				if (member is MethodInfo method)
				{
					List<MethodInfo> allMethods = null;
					while (i < members.Length)
					{
						if (members[i++] is MethodInfo method2)
						{
							if (allMethods == null)
							{
								allMethods = new List<MethodInfo>();
								allMethods.Add(method);
							}
							allMethods.Add(method2);
						}
					}
					var func = new ReflectedFunction(Engine, this, name,
						allMethods == null ? new MethodInfo[] { method } : allMethods.ToArray());
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
