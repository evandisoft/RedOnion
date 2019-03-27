using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RedOnion.Script.BasicObjects
{
	/// <summary>
	/// Array function (used to create new array objects)
	/// </summary>
	public class ListFun : BasicObject
	{
		/// <summary>
		/// Prototype of all array objects
		/// </summary>
		public ListObj Prototype { get; }

		public override ObjectFeatures Features
			=> ObjectFeatures.Function | ObjectFeatures.Constructor;
		//	| ObjectFeatures.Converter | ObjectFeatures.TypeReference;
		//public override Type Type => typeof(List<Value>);

		public ListFun(IEngine engine, IObject baseClass, ListObj prototype)
			: base(engine, baseClass, new Properties("prototype", prototype))
			=> Prototype = prototype;

		public override Value Call(IObject self, int argc)
			=> new Value(Create(argc));

		public override IObject Create(int argc)
		{
			var list = new List<Value>(argc);
			for (int i = 0; i < argc; i++)
				list[i] = Engine.GetArgument(argc, i);
			return new ListObj(Engine, Prototype, list);
		}

		public override IObject Convert(object value)
			=> new ListObj(Engine, Prototype, GetList(value));

		public List<Value> GetList(Value value)
		{
			value = Value.RValue;
			if (value.Kind == ValueKind.Object)
			{
				var obj = (IObject)value.ptr;
				if (obj is ListObj list)
					return list.List;
			}
			return new List<Value>();
		}
		public List<Value> GetList(object obj)
		{
			if (obj is List<Value> list)
				return list;
			if (obj is Value val)
				return GetList(val);
			return new List<Value>();
		}
	}

	/// <summary>
	/// String object (string box)
	/// </summary>
	[DebuggerDisplay("{Name}")]
	public class ListObj : BasicObject, IListObject
	{
		public static readonly Value[] Empty = new Value[0];
		public List<Value> List { get; protected set; }
		public override Value Value => Name;

		/// <summary>
		/// Create List.prototype
		/// </summary>
		public ListObj(IEngine engine, IObject baseClass)
			: base(engine, baseClass)
		{ }

		/// <summary>
		/// Create new string object boxing the string
		/// </summary>
		public ListObj(IEngine engine, ListObj baseClass, List<Value> value)
			: base(engine, baseClass, new Properties(StdProps))
			=> List = value;

		public override Value Index(IObject self, int argc)
		{
			if (argc == 1)
			{
				var i = Engine.GetArgument(argc);
				if (i.IsNumber)
					return List[i.Int];
			}
			return base.Index(self, argc);
		}

		int ICollection<Value>.Count => List.Count;
		bool ICollection<Value>.IsReadOnly => false;

		bool ICollection<Value>.Contains(Value item)
			=> List.Contains(item);
		void ICollection<Value>.CopyTo(Value[] array, int arrayIndex)
			=> List.CopyTo(array, arrayIndex);

		IEnumerator IEnumerable.GetEnumerator()
			=> List.GetEnumerator();
		public IEnumerator<Value> GetEnumerator()
			=> List.GetEnumerator();

		bool IListObject.IsWritable => true;
		bool IListObject.IsFixedSize => false;
		Value IList<Value>.this[int index]
		{
			get => List[index];
			set => List[index] = value;
		}

		public void Add(Value item) => List.Add(item);
		public void Clear() => List.Clear();
		public int IndexOf(Value item) => List.IndexOf(item);
		public void Insert(int index, Value item) => List.Insert(index, item);
		public bool Remove(Value item) => List.Remove(item);
		public void RemoveAt(int index) => List.RemoveAt(index);

		public static IDictionary<string, Value> StdProps { get; } = new Dictionary<string, Value>()
		{
			{ "length",	ArrayObj.LengthProp.Value },
			{ "add",	Value.Method<ListObj>((list, item) => list.Add(item)) },
			{ "clear",  Value.Method<ListObj>(list => list.Clear()) },
			{ "indexOf", Value.Method<ListObj>((list, index) => list.IndexOf(index.Int)) },
			{ "insert", Value.Method<ListObj>((list, index, item) => list.Insert(index.Int, item)) },
			{ "remove",	Value.Func<ListObj>((list, item) => list.Remove(item)) },
			{ "removeAt", Value.Method<ListObj>((list, index) => list.RemoveAt(index.Int)) },
		};
	}
}
