using System;
using System.Collections.Generic;
using System.Threading;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
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
			it = new Reflected(type);
			_of[type] = it;
			_lock.ExitWriteLock();
			_lock.ExitUpgradeableReadLock();
			return it;
		}
		public static Descriptor Of(Type type, Descriptor value)
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
			it = value ?? new Reflected(type);
			_of[type] = it;
			_lock.ExitWriteLock();
			_lock.ExitUpgradeableReadLock();
			return it;
		}
		public static Descriptor Of(Type type, Func<Descriptor> create)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			_lock.EnterReadLock();
			if (_of.TryGetValue(type, out var it))
			{
				_lock.ExitReadLock();
				return it;
			}
			_lock.EnterUpgradeableReadLock();
			if (_of.TryGetValue(type, out it))
			{
				_lock.ExitUpgradeableReadLock();
				return it;
			}
			_lock.EnterWriteLock();
			try
			{
				it = create() ?? new Reflected(type);
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
