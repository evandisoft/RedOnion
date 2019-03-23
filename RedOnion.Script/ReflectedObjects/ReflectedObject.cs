using System;
using System.Collections.Generic;
using System.Reflection;

// TODO: Better support for structures in properties (having both get and set)
// ----- Allow the changes to inner fields/properties be propagated back
// ----- Will need precise lvalue/rvalue tracking (rvalue does not propagate back)

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedObject : BasicObjects.SimpleObject
	{
		public override ObjectFeatures Features
			=> ObjectFeatures.Proxy | ObjectFeatures.Operators;

		private object _target;
		private Type _type;
		public ReflectedType Creator { get; }

		public override object Target => _target;
		public override Type Type => _type;

		public ReflectedObject(IEngine engine, object target, IProperties properties = null)
			: base(engine, properties)
			=> _type = (_target = target)?.GetType();
		public ReflectedObject(IEngine engine, object target, ReflectedType type, IProperties properties = null)
			: this(engine, target, properties)
		{
			_type = (Creator = type)?.Type ?? target.GetType();
			BaseProps = type.TypeProps;
		}

		protected MemberInfo[] GetMembers(string name)
			=> ReflectedType.GetMembers(Type, name, instance: true);

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
					BaseProps.Set(name, value = func);
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
				if (member is EventInfo)
					return false;
			}
			return false;
		}

		public override bool Set(string name, Value value)
		{
			if (BaseProps != null && BaseProps.Get(name, out var query))
				return query.Set(this, value);
			var members = GetMembers(name);
			for (int i = 0; i < members.Length;)
			{
				var member = members[i++];
				if (member is MethodInfo method)
					return false;
				if (member is EventInfo)
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

		public override bool Modify(string name, OpCode op, Value value)
		{
			if (BaseProps != null && BaseProps.Get(name, out var query))
				return query.Modify(this, op, value);
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
					var tmp = fld.Get(this);
					tmp.Modify(op, value);
					return fld.Set(this, tmp);
				}
				if (member is PropertyInfo property)
				{
					if (property.GetIndexParameters().Length > 0)
						continue;
					var prop = new Property(property);
					if (BaseProps == null)
						BaseProps = new Properties();
					BaseProps.Set(name, new Value(prop));
					var tmp = prop.Get(this);
					tmp.Modify(op, value);
					return prop.Set(this, value);
				}
				if (member is EventInfo evt)
				{
					var devt = new Event(evt);
					if (BaseProps == null)
						BaseProps = new Properties();
					BaseProps.Set(name, new Value(devt));
					return devt.Modify(this, op, value);
				}
			}
			return false;
		}

		private KeyValuePair<PropertyInfo, ParameterInfo[]>[] indexers;
		protected KeyValuePair<PropertyInfo, ParameterInfo[]>[] Indexers
		{
			get
			{
				if (indexers == null)
				{
					if (Creator != null && Creator.indexers != null)
						return indexers = Creator.indexers;
					List<KeyValuePair<PropertyInfo, ParameterInfo[]>> list = null;
					foreach (var info in Type.GetProperties())
					{
						var pars = info.GetIndexParameters();
						if (pars.Length == 0)
							continue;
						if (list == null)
							list = new List<KeyValuePair<PropertyInfo, ParameterInfo[]>>();
						list.Add(new KeyValuePair<PropertyInfo, ParameterInfo[]>(info, pars));
					}
					indexers = list?.ToArray() ?? new KeyValuePair<PropertyInfo, ParameterInfo[]>[0];
					if (Creator != null)
						Creator.indexers = indexers;
				}
				return indexers;
			}
		}
		public override Value Index(IObject self, int argc)
		{
			if (argc <= 0)
				return new Value();
			var indexers = Indexers;
			var value = Arg(argc, 0);
			if (indexers != null && argc == 1) // TODO multi-indexers
			{
				foreach (var pair in indexers)
				{
					if (pair.Value.Length != argc || !pair.Key.CanRead)
						continue;
					if (value.IsString && pair.Value[0].ParameterType == typeof(string))
						return Convert(Engine, pair.Key.GetGetMethod()
							.Invoke(Target, new object[] { value.String }));
				}
			}
			value = new Value(this, value.String);
			return argc == 1 ? value
				: Engine.Box(new Value(this, value.String)).Index(this, argc - 1);
		}

		internal static readonly Dictionary<OpCode, string> OperatorNames = new Dictionary<OpCode, string>()
		{
			{ OpCode.Plus,		"op_UnaryPlus" },
			{ OpCode.Neg,		"op_UnaryNegation" },
			{ OpCode.Inc,		"op_Increment" },
			{ OpCode.Dec,		"op_Decrement" },
			{ OpCode.Not,		"op_LogicalNot" },
			{ OpCode.Add,		"op_Addition" },
			{ OpCode.Sub,		"op_Subtraction" },
			{ OpCode.Mul,		"op_Multiply" },
			{ OpCode.Div,		"op_Division" },
			{ OpCode.BitAnd,	"op_BitwiseAnd" },
			{ OpCode.BitOr,		"op_BitwiseOr" },
			{ OpCode.BitXor,	"op_ExclusiveOr" },
			{ OpCode.Equals,	"op_Equality" },
			{ OpCode.Differ,	"op_Inequality" },
			{ OpCode.Less,		"op_LessThan" },
			{ OpCode.More,		"op_GreaterThan" },
			{ OpCode.LessEq,	"op_LessThanOrEqual" },
			{ OpCode.MoreEq,	"op_GreaterThanOrEqual" },
			{ OpCode.ShiftLeft,	"op_LeftShift" },
			{ OpCode.ShiftRight,"op_RightShift" },
			{ OpCode.Mod,		"op_Modulus" },
			// cast: op_Implicit/op_Explicit, bool: op_True/op_False
		};
		Dictionary<OpCode, MethodInfo[]> operators;
		protected MethodInfo[] GetOperators(OpCode op)
		{
			if (operators == null)
			{
				if (Creator != null)
					operators = Creator.operators;
				if (operators == null)
				{
					operators = new Dictionary<OpCode, MethodInfo[]>();
					if (Creator != null)
						Creator.operators = operators;
				}
			}
			if (operators.TryGetValue(op, out var it))
				return it;
			if (!OperatorNames.TryGetValue(op, out var name))
				return null;
			var members = ReflectedType.GetMembers(Type, name, instance: false);
			var methods = new MethodInfo[members.Length];
			Array.Copy(members, methods, members.Length);
			return operators[op] = methods;
		}
		public override bool Operator(OpCode op, Value arg, bool selfRhs, out Value result)
		{
			result = new Value();
			var methods = GetOperators(op);
			if (methods == null || methods.Length == 0)
				return false;
			if (op.Unary())
			{
				var arguments = Engine.Arguments;
				var startLength = arguments.Length;
				try
				{
					arguments.Add(new Value(this));
					foreach (var method in methods)
						if (ReflectedFunction.TryCall(Engine, method, null, 1, ref result))
							return true;
				}
				finally
				{
					arguments.Remove(arguments.Length - startLength);
				}
				return false;
			}
			else
			{
				var arguments = Engine.Arguments;
				var startLength = arguments.Length;
				try
				{
					if (!selfRhs)
						arguments.Add(new Value(this));
					arguments.Add(arg);
					if (selfRhs)
						arguments.Add(new Value(this));
					foreach (var method in methods)
						if (ReflectedFunction.TryCall(Engine, method, null, 2, ref result))
							return true;
				}
				finally
				{
					arguments.Remove(arguments.Length - startLength);
				}
				return false;
			}
		}

		public static Value Convert(IEngine engine, object value)
			=> ReflectedType.Convert(engine, value);
		public static Value Convert(IEngine engine, object value, Type type)
			=> ReflectedType.Convert(engine, value, type);
		public static object Convert(Value value, Type type)
			=> ReflectedType.Convert(value, type);

		public class Field : IProperty
		{
			public FieldInfo Info { get; }
			public Field(FieldInfo info) => Info = info;
			public Value Get(IObject self)
				=> Convert(self.Engine, Info.GetValue(self.Target));
			public bool Set(IObject self, Value value)
			{
				try
				{
					Info.SetValue(self.Target, Convert(value, Info.FieldType));
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
				: Convert(self.Engine, Info.GetValue(self.Target, new object[0]));
			public bool Set(IObject self, Value value)
			{
				if (!Info.CanWrite)
					return false;
				try
				{
					Info.SetValue(self.Target, Convert(value, Info.PropertyType), new object[0]);
					return true;
				}
				catch
				{
					return false;
				}
			}
		}
		public class Event : IPropertyEx
		{
			public EventInfo Info { get; }
			public Event(EventInfo info) => Info = info;
			public Value Get(IObject self) => new Value();
			public bool Set(IObject self, Value value) => false;
			public bool Modify(IObject self, OpCode op, Value value)
			{
				switch (op)
				{
				case OpCode.AddAssign:
					Info.AddEventHandler(self.Target, (Delegate)Convert(value, Info.EventHandlerType));
					return true;
				case OpCode.SubAssign:
					Info.RemoveEventHandler(self.Target, (Delegate)Convert(value, Info.EventHandlerType));
					return true;
				}
				return false;
			}
		}
	}
}
