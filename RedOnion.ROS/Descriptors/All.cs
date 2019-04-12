using System;
using System.Collections.Generic;
using System.Threading;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static Func<Type,Descriptor> Create = type => new Reflected(type);

		public static readonly Descriptor Double = new OfDouble();
		public static readonly Descriptor Float = new OfFloat();
		public static readonly Descriptor Long = new OfLong();
		public static readonly Descriptor ULong = new OfULong();
		public static readonly Descriptor Int = new OfInt();
		public static readonly Descriptor UInt = new OfUInt();
		public static readonly Descriptor Short = new OfShort();
		public static readonly Descriptor UShort = new OfUShort();
		public static readonly Descriptor SByte = new OfSByte();
		public static readonly Descriptor Byte = new OfByte();
		public static readonly Descriptor Char = new OfChar();
		public static readonly Descriptor String = new OfString();

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
			try
			{
				it = value ?? Create(type);
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
				it = create() ?? Create(type);
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
		public static Descriptor Of(Type type, Func<Type,Descriptor> create)
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
				it = create(type) ?? Create(type);
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
