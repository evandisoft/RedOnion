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
		public override Type Type => _type;
		private Type _type;

		public override ObjectFeatures Features
			=> ObjectFeatures.Function | ObjectFeatures.Constructor
			| ObjectFeatures.Converter | ObjectFeatures.TypeReference;

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
			=> _type = type;

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
				if (Type.IsValueType)
					return new ReflectedObject(Engine, Activator.CreateInstance(Type), this, TypeProps);
				var ctor = Type.GetConstructor(new Type[0]);
				if (ctor == null)
				{
					if (!Engine.HasOption(Engine.Option.Silent))
						throw new NotImplementedException(Type.FullName
							+ " cannot be constructed with zero arguments");
					return null;
				}
				return new ReflectedObject(Engine, ctor.Invoke(new object[0]), this, TypeProps);
			}
			var value = new Value();
			foreach (var ctor in Type.GetConstructors())
			{
				if (!ReflectedFunction.TryCall(Engine, ctor, this, argc, ref value))
					continue;
				var obj = value.Object;
				if (obj != null)
					return obj;
				break;
			}
			if (!Engine.HasOption(Engine.Option.Silent))
				throw new NotImplementedException(string.Format(
					"{0} cannot be constructed with {1} argument(s)", Type.FullName, argc));
			return null;
		}

		public override IObject Convert(object value)
			=> new ReflectedObject(Engine, value, this, TypeProps);

		public class MemberComparer : IComparer<MemberInfo>
		{
			public static MemberComparer Instance { get; } = new MemberComparer();
			public int Compare(MemberInfo x, MemberInfo y)
			{
				// BIG letters first to prefer properties over fields
				int cmp = string.CompareOrdinal(x.Name, y.Name);
				// sorting by MetadataToken ensures stability
				// (to avoid problems with reflection cache)
				return cmp != 0 ? cmp
					: x.MetadataToken.CompareTo(y.MetadataToken);
			}
		}
		public static MemberInfo[] GetMembers(Type type, string name, bool instance)
		{
			var flags = BindingFlags.IgnoreCase|BindingFlags.Public
				| (instance ? BindingFlags.Instance : BindingFlags.Static);
			var members = name == null
				? type.GetMembers(flags)
				: type.GetMember(name, flags);
			if (members.Length > 1)
				Array.Sort(members, MemberComparer.Instance);
			return members;
		}

		public override IObject Which(string name)
		{
			if (BaseProps?.Has(name) == true)
				return this;
			foreach (var member in GetMembers(Type, name, instance: false))
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
			var members = GetMembers(Type, name, instance: false);
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
				if (member is FieldInfo field)
				{
					var fld = new StaticField(field);
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
					var prop = new StaticProperty(property);
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
			var members = GetMembers(Type, name, instance: false);
			for (int i = 0; i < members.Length;)
			{
				var member = members[i++];
				if (member is MethodInfo method)
					return false;
				if (member is FieldInfo field)
				{
					var fld = new StaticField(field);
					if (BaseProps == null)
						BaseProps = new Properties();
					BaseProps.Set(name, new Value(fld));
					return fld.Set(this, value);
				}
				if (member is PropertyInfo property)
				{
					if (property.GetIndexParameters().Length > 0)
						continue;
					var prop = new StaticProperty(property);
					if (BaseProps == null)
						BaseProps = new Properties();
					BaseProps.Set(name, new Value(prop));
					return prop.Set(this, value);
				}
			}
			return false;
		}

		public static Value Convert(Engine engine, object value)
			=> value == null ? new Value((IObject)null)
			: Convert(engine, value, value.GetType());
		public static Value Convert(Engine engine, object value, Type type)
		{
			if (type == typeof(void))
				return new Value();
			if (value == null)
				return new Value((IObject)null);
			if (value is string || type == typeof(string))
				return value.ToString();
			if (type.IsPrimitive || type.IsEnum)
			{
				if (value is Enum e)
					value = System.Convert.ChangeType(value, Enum.GetUnderlyingType(type));
				if (value is bool bval)
					return bval;
				if (value is int ival)
					return ival;
				if (value is float fval)
					return fval;
				if (value is double dval)
					return dval;
				if (value is uint uval)
					return uval;
				if (value is long i64)
					return i64;
				if (value is ulong u64)
					return u64;
				if (value is short i16)
					return i16;
				if (value is ushort u16)
					return u16;
				if (value is byte u8)
					return u8;
				if (value is sbyte i8)
					return i8;
				if (value is char c)
					return c;
			}
			var converter = engine.Root[type];
			return converter == null ? new Value()
				: new Value(converter.Convert(value));
		}
		public static object Convert(Value value, Type type)
		{
			if (type == typeof(string))
				return value.String;
			if (type.IsPrimitive || type.IsEnum)
				return System.Convert.ChangeType(value.Native, type);
			var val = value.Native;
			if (val == null)
				return null;
			if (type.IsAssignableFrom(val.GetType()))
				return val;
			throw new NotImplementedException();
		}
		public static T Convert<T>(Value value)
			=> (T)Convert(value, typeof(T));

		public class StaticField : IProperty
		{
			public FieldInfo Info { get; }
			public StaticField(FieldInfo info) => Info = info;
			public Value Get(IObject self)
				=> Convert(self.Engine, Info.GetValue(null));
			public bool Set(IObject self, Value value)
			{
				try
				{
					Info.SetValue(null, Convert(value, Info.FieldType));
					return true;
				}
				catch
				{
					return false;
				}
			}
		}
		public class StaticProperty : IProperty
		{
			public PropertyInfo Info { get; }
			public StaticProperty(PropertyInfo info) => Info = info;
			public Value Get(IObject self)
				=> !Info.CanRead ? new Value()
				: Convert(self.Engine, Info.GetValue(null, new object[0]));
			public bool Set(IObject self, Value value)
			{
				if (!Info.CanWrite)
					return false;
				try
				{
					Info.SetValue(null, Convert(value, Info.PropertyType), new object[0]);
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
