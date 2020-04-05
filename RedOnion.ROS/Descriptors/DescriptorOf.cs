using System;
using System.Collections.Generic;
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
		public static Func<Type,Descriptor> Create = StandardCreate;

		// watch out not to mess the order!

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
		// TODO: single descriptor for generic types
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
	}
}
