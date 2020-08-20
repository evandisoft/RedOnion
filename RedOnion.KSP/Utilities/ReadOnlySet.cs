using MunSharp.Interpreter;
using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace RedOnion.KSP.API
{
	public class ReadOnlySet<T> : ReadOnlyList<T>
	{
		protected HashSet<T> hset = new HashSet<T>();
		protected internal ReadOnlySet() { }
		protected internal ReadOnlySet(Action refresh) : base(refresh) { }

		public override bool Contains(T item)
		{
			Update();
			return hset.Contains(item);
		}

		protected internal override void Clear()
		{
			list.Clear();
			hset.Clear();
		}
		protected internal override bool Add(T item)
		{
			if (!hset.Add(item))
				return false;
			list.Add(item);
			return true;
		}
	}
}
