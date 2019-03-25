using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedList : ReflectedObject, IArray
	{
		IList list;
		public ReflectedList(IEngine engine, object target, IProperties properties = null)
			: base(engine, target, properties)
			=> list = target as IList;
		public ReflectedList(IEngine engine, object target, ReflectedType type, IProperties properties = null)
			: base(engine, target, type, properties)
			=> list = target as IList;

		public Value this[int index]
		{
			get => ReflectedType.Convert(Engine, list[index]);
			set => throw new NotImplementedException();
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<Value> GetEnumerator()
		{
			foreach (var e in list)
				yield return ReflectedType.Convert(Engine, e);
		}

		bool ICollection<Value>.IsReadOnly => true;
		bool IArray.IsWritable => false;
		bool IArray.IsFixedSize => true;
		int ICollection<Value>.Count => list.Count;

		void ICollection<Value>.Add(Value item) => throw new NotImplementedException();
		void ICollection<Value>.Clear() => throw new NotImplementedException();
		bool ICollection<Value>.Contains(Value item) => list.Contains(item.Native);
		void ICollection<Value>.CopyTo(Value[] array, int arrayIndex) => throw new NotImplementedException();
		int IList<Value>.IndexOf(Value item) => list.IndexOf(item.Native);
		void IList<Value>.Insert(int index, Value item) => throw new NotImplementedException();
		bool ICollection<Value>.Remove(Value item) => throw new NotImplementedException();
		void IList<Value>.RemoveAt(int index) => throw new NotImplementedException();
	}
}
