using System;
using System.Collections.Generic;
using System.Reflection;

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedObject : BasicObjects.SimpleObject, IObjectProxy
	{
		public object Target { get; }
		public ReflectedType Creator { get; }

		public ReflectedObject(Engine engine, object target, IProperties properties = null)
			: base(engine, properties)
			=> Target = target;
		public ReflectedObject(Engine engine, object target, ReflectedType type, IProperties properties = null)
			: this(engine, target, properties)
		{
			Creator = type;
			BaseProps = type.TypeProps;
		}

		protected MemberInfo[] GetMembers(string name)
			=> ReflectedType.GetMembers(Target.GetType(), name, instance: true);

		public override IObject Which(string name)
		{
			if (BaseProps?.Has(name) == true)
				return this;
			foreach (var member in GetMembers(name))
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
			var members = GetMembers(name);
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
					var func = new ReflectedMethod(Engine, Creator, name,
						allMethods == null ? new MethodInfo[] { method } : allMethods.ToArray());
					if (BaseProps == null)
						BaseProps = new Properties();
					BaseProps.Set(name, value = new Value(func));
					return true;
				}
				if (member is FieldInfo field)
				{
					var fld = new Field(field);
					if (BaseProps == null)
						BaseProps = new Properties();
					BaseProps.Set(name, new Value(fld));
					value = fld.Get(this);
					return true;
				}
				if (member is PropertyInfo property)
				{
					if (property.GetIndexParameters().Length > 0)
						continue;
					var prop = new Property(property);
					if (BaseProps == null)
						BaseProps = new Properties();
					BaseProps.Set(name, new Value(prop));
					value = prop.Get(this);
					return true;
				}
			}
			return false;
		}

		public override bool Set(string name, Value value)
		{
			if (BaseProps != null && BaseProps.Get(name, out var query))
			{
				if (query.Type != ValueKind.Property)
					return false;
				((IProperty)query.ptr).Set(this, value);
				return true;
			}
			var members = GetMembers(name);
			for (int i = 0; i < members.Length;)
			{
				var member = members[i++];
				if (member is MethodInfo method)
					return false;
				if (member is FieldInfo field)
				{
					var fld = new Field(field);
					if (BaseProps == null)
						BaseProps = new Properties();
					BaseProps.Set(name, new Value(fld));
					return fld.Set(this, value);
				}
				if (member is PropertyInfo property)
				{
					if (property.GetIndexParameters().Length > 0)
						continue;
					var prop = new Property(property);
					if (BaseProps == null)
						BaseProps = new Properties();
					BaseProps.Set(name, new Value(prop));
					return prop.Set(this, value);
				}
			}
			return false;
		}

		public static Value Convert(Engine engine, object value)
			=> ReflectedType.Convert(engine, value);
		public static Value Convert(Engine engine, object value, Type type)
			=> ReflectedType.Convert(engine, value, type);
		public static object Convert(Value value, Type type)
			=> ReflectedType.Convert(value, type);

		public class Field : IProperty
		{
			public FieldInfo Info { get; }
			public Field(FieldInfo info) => Info = info;
			public Value Get(IObject self)
				=> Convert(self.Engine, Info.GetValue(((IObjectProxy)self).Target));
			public bool Set(IObject self, Value value)
			{
				try
				{
					Info.SetValue(((IObjectProxy)self).Target, Convert(value, Info.FieldType));
					return true;
				}
				catch
				{
					return false;
				}
			}
		}
		public class Property : IProperty
		{
			public PropertyInfo Info { get; }
			public Property(PropertyInfo info) => Info = info;
			public Value Get(IObject self)
				=> !Info.CanRead ? new Value()
				: Convert(self.Engine, Info.GetValue(((IObjectProxy)self).Target, new object[0]));
			public bool Set(IObject self, Value value)
			{
				if (!Info.CanWrite)
					return false;
				try
				{
					Info.SetValue(((IObjectProxy)self).Target, Convert(value, Info.PropertyType), new object[0]);
					return true;
				}
				catch
				{
					return false;
				}
			}
		}
	}
}
