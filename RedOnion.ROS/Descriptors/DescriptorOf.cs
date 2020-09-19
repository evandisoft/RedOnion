using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static Descriptor StandardCreate(Type type)
		{
			if (typeof(Delegate).IsAssignableFrom(type))
				return Callable.FromType(type);
			return new Reflected(type);
		}

		// watch out not to mess the order!

		#region 1. Independent stuff

		public static CultureInfo Culture = CultureInfo.InvariantCulture;
		public static Func<Type,Descriptor> Create = StandardCreate;

		internal static readonly MethodInfo Constructor
			= typeof(Descriptor).GetMethod("Construct");

		internal static Dictionary<Type, ConstructorInfo>
			PrimitiveValueConstructors = GetPrimitiveValueConstructors();
		internal static readonly ConstructorInfo DefaultValueConstructor
			= typeof(Value).GetConstructor(new Type[] { typeof(object) });
		internal static readonly ConstructorInfo IntValueConstructor
			= typeof(Value).GetConstructor(new Type[] { typeof(int) });
		internal static readonly ConstructorInfo StrValueConstructor
			= typeof(Value).GetConstructor(new Type[] { typeof(string) });

		internal static readonly MethodInfo ValueToObject
			= typeof(Value).GetMethod("Box");
		internal static readonly MethodInfo ValueToInt
			= typeof(Value).GetMethod("ToInt");
		internal static readonly MethodInfo ValueToUInt
			= typeof(Value).GetMethod("ToUInt");
		internal static readonly MethodInfo ValueToLong
			= typeof(Value).GetMethod("ToLong");
		internal static readonly MethodInfo ValueToULong
			= typeof(Value).GetMethod("ToULong");
		internal static readonly MethodInfo ValueToDouble
			= typeof(Value).GetMethod("ToDouble");
		internal static readonly MethodInfo ValueToStr
			= typeof(Value).GetMethod("ToStr");
		internal static readonly MethodInfo ValueToBool
			= typeof(Value).GetMethod("ToBool");
		internal static readonly MethodInfo ValueToChar
			= typeof(Value).GetMethod("ToChar");
		internal static readonly MethodInfo ValueToType
			= typeof(Value).GetMethod("ToType", new Type[] { typeof(Type) });

		internal static readonly ParameterExpression SelfParameter
			= Expression.Parameter(typeof(object), "self");
		internal static readonly ParameterExpression ValueParameter
			= Expression.Parameter(typeof(Value), "value");
		internal static readonly ParameterExpression IntIndexParameter
			= Expression.Parameter(typeof(int), "index");
		internal static readonly ParameterExpression StrIndexParameter
			= Expression.Parameter(typeof(string), "index");
		internal static readonly ParameterExpression ValIndexParameter
			= Expression.Parameter(typeof(Value), "index");
		internal static readonly ParameterExpression ValueArg0Parameter
			= Expression.Parameter(typeof(Value), "arg0");
		internal static readonly ParameterExpression ValueArg1Parameter
			= Expression.Parameter(typeof(Value), "arg1");
		internal static readonly ParameterExpression ValueArg2Parameter
			= Expression.Parameter(typeof(Value), "arg2");

		#endregion

		#region 2. Constants: Void, Null, NaN, False, True

		public static readonly Descriptor Void = new OfVoid();
		public static readonly Value VoidValue = new Value(Void, null);

		public static readonly Descriptor Null = new OfNull();
		public static readonly Value NullValue = new Value(Null, null);
		internal static readonly Descriptor NullSelf = new OfNullSelf();

		public static readonly Descriptor Double = new OfDouble();
		public static readonly Value NaNValue = new Value(double.NaN);

		public static readonly Descriptor Bool = new OfBool();
		public static readonly Value FalseValue = new Value(false);
		public static readonly Value TrueValue = new Value(true);

		#endregion

		#region 3. Other descriptors

		public static readonly Descriptor Int = new OfInt();
		public static readonly Descriptor Char = new OfChar();
		public static readonly Descriptor String = new OfString();

		public static readonly Descriptor Float = new OfFloat();
		public static readonly Descriptor Long = new OfLong();
		public static readonly Descriptor ULong = new OfULong();
		public static readonly Descriptor UInt = new OfUInt();
		public static readonly Descriptor Short = new OfShort();
		public static readonly Descriptor UShort = new OfUShort();
		public static readonly Descriptor SByte = new OfSByte();
		public static readonly Descriptor Byte = new OfByte();

		public static readonly Descriptor[] Actions = new Descriptor[] {
			new Action0("Action (0 args)"),
			new Action1("Action (1 arg)"),
			new Action2("Action (2 args)"),
			new Action3("Action (3 args)"),
		};
		public static readonly Descriptor[] Functions = new Descriptor[] {
			new Function0("Function (0 args)"),
			new Function1("Function (1 arg)"),
			new Function2("Function (2 args)"),
			new Function3("Function (3 args)")
		};

		#endregion

		#region 4. The Descriptor.Of itself

		private readonly static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
		private readonly static Dictionary<Type, Descriptor> _of = new Dictionary<Type, Descriptor>()
		{
			{ typeof(double), Double },
			{ typeof(float),  Float },
			{ typeof(long),   Long },
			{ typeof(ulong),  ULong },
			{ typeof(int),    Int },
			{ typeof(uint),   UInt },
			{ typeof(short),  Short },
			{ typeof(ushort), UShort },
			{ typeof(sbyte),  SByte },
			{ typeof(byte),   Byte },
			{ typeof(bool),   Bool },
			{ typeof(char),   Char },
			{ typeof(string), String },
		};
		public static Descriptor Of(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			_lock.EnterReadLock();
			if (_of.TryGetValue(type, out var it))
			{
				_lock.ExitReadLock();
				return it;
			}
			_lock.ExitReadLock();
			_lock.EnterUpgradeableReadLock();
			if (_of.TryGetValue(type, out it))
			{
				_lock.ExitUpgradeableReadLock();
				return it;
			}
			_lock.EnterWriteLock();
			try
			{
				it = Create(type);
				if (it != null)
					_of[type] = it;
			}
			finally
			{
				_lock.ExitWriteLock();
				_lock.ExitUpgradeableReadLock();
			}
			return it;
		}
		#endregion
	}
}
