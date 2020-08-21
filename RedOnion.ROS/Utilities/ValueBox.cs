using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace RedOnion.ROS.Utilities
{
	public sealed class ValueBox : StrongBox<Value>
	{
		internal ValueBox() { }
		internal ValueBox(Value value) : base(value) { }
		internal ValueBox(ref Value value) => Value = value;

		static int poolLock, poolTop;
		static readonly ValueBox[] pool = new ValueBox[64];

		public static ValueBox Borrow()
		{
			// probably never going to use it in multiple threads,
			// but let us at least protect it with fast spin-lock
			while (Interlocked.CompareExchange(ref poolLock, 1, 0) != 0)
				;

			ValueBox it;
			if (poolTop > 0)
				it = pool[--poolTop];
			else it = new ValueBox();

			// could possibly do simple `poolLock = 0`
			// but this should make it visible to other threads a bit sooner
			Interlocked.Exchange(ref poolLock, 0);
			return it;
		}
		public static void Return(ValueBox it)
		{
			while (Interlocked.CompareExchange(ref poolLock, 1, 0) != 0)
				;

			if (poolTop < pool.Length)
				pool[poolTop++] = it;

			Interlocked.Exchange(ref poolLock, 0);
		}

		public static ValueBox Borrow(ref Value copy)
		{
			var it = Borrow();
			it.Value = copy;
			return it;
		}
		public override string ToString() => Value.ToString();
	}
}
