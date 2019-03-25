using System;
using System.Collections;
using System.Collections.Generic;

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedEnumerable : ReflectedEnumerable<IEnumerable>
	{
		public ReflectedEnumerable(IEngine engine, IEnumerable target, IProperties properties = null)
			: base(engine, target, properties) { }
		public ReflectedEnumerable(IEngine engine, IEnumerable target, ReflectedType type, IProperties properties = null)
			: base(engine, target, type, properties) { }

	}
	public class ReflectedEnumerable<T> : ReflectedObject<T>, IEnumerableObject where T: IEnumerable
	{
		public ReflectedEnumerable(IEngine engine, T target, IProperties properties = null)
			: base(engine, target, properties) { }
		public ReflectedEnumerable(IEngine engine, T target, ReflectedType type, IProperties properties = null)
			: base(engine, target, type, properties) { }

		public override string Name => PureName + " [enumerable]";

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<Value> GetEnumerator()
		{
			foreach (var e in It)
				yield return ReflectedType.Convert(Engine, e);
		}
	}
}
