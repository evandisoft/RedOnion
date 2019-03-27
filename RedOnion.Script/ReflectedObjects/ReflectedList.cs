using System;
using System.Collections;
using System.Collections.Generic;

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedList : ReflectedList<IList>
	{
		public ReflectedList(IEngine engine, IList target, IProperties properties = null)
			: base(engine, target, properties) { }
		public ReflectedList(IEngine engine, IList target, ReflectedType type)
			: base(engine, target, type) { }

	}
	public class ReflectedList<T> : ReflectedEnumerable<T>, IListObject where T: IList
	{
		public ReflectedList(IEngine engine, T target, IProperties properties = null)
			: base(engine, target, properties) { }
		public ReflectedList(IEngine engine, T target, ReflectedType type)
			: base(engine, target, type) { }

		public override string Name => PureName + " [list]";

		public Value this[int index]
		{
			get => ReflectedType.Convert(Engine, It[index]);
			set => throw new NotImplementedException();
		}

		bool ICollection<Value>.IsReadOnly => true;
		bool IListObject.IsWritable => false;
		bool IListObject.IsFixedSize => true;
		int ICollection<Value>.Count => It.Count;

		void ICollection<Value>.Add(Value item) => throw new NotImplementedException();
		void ICollection<Value>.Clear() => throw new NotImplementedException();
		bool ICollection<Value>.Contains(Value item) => It.Contains(item.Native);
		void ICollection<Value>.CopyTo(Value[] array, int arrayIndex) => throw new NotImplementedException();
		int IList<Value>.IndexOf(Value item) => It.IndexOf(item.Native);
		void IList<Value>.Insert(int index, Value item) => throw new NotImplementedException();
		bool ICollection<Value>.Remove(Value item) => throw new NotImplementedException();
		void IList<Value>.RemoveAt(int index) => throw new NotImplementedException();
	}
}
