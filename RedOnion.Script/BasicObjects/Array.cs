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
	public class ArrayFun : BasicObject
	{
		/// <summary>
		/// Prototype of all array objects
		/// </summary>
		public ArrayObj Prototype { get; }
		/// <summary>
		/// Empty array
		/// </summary>
		public ArrayObj Empty { get; }

		public override ObjectFeatures Features
			=> ObjectFeatures.Function | ObjectFeatures.Constructor;
		//	| ObjectFeatures.Converter | ObjectFeatures.TypeReference;
		//public override Type Type => typeof(Value[]);

		public ArrayFun(IEngine engine, IObject baseClass, ArrayObj prototype)
			: base(engine, baseClass, new Properties("prototype", prototype))
			=> Empty = new ArrayObj(Engine, Prototype = prototype);

		public override Value Call(IObject self, int argc)
			=> argc == 0 ? new Value(Empty)
			: new ArrayObj(Engine, Prototype, GetArray(Engine.GetArgument(argc)));

		public override IObject Create(int argc)
		{
			var arr = new Value[argc];
			for (int i = 0; i < arr.Length; i++)
				arr[i] = Engine.GetArgument(argc, i);
			return new ArrayObj(Engine, Prototype, arr);
		}

		public override IObject Convert(object value)
			=> new ArrayObj(Engine, Prototype, GetArray(value));

		public Value[] GetArray(Value value)
		{
			value = Value.RValue;
			if (value.Kind == ValueKind.Object)
			{
				var obj = (IObject)value.ptr;
				if (obj is ArrayObj arr)
					return arr.Array;
			}
			return ArrayObj.Empty;
		}
		public Value[] GetArray(object obj)
		{
			if (obj is Value[] arr)
				return arr;
			if (obj is Value val)
				return GetArray(val);
			return ArrayObj.Empty;
		}
	}

	/// <summary>
	/// String object (string box)
	/// </summary>
	[DebuggerDisplay("{Name}")]
	public class ArrayObj : BasicObject, IListObject
	{
		public static readonly Value[] Empty = new Value[0];
		public Value[] Array { get; protected set; }
		public override Value Value => new Value(this);

		/// <summary>
		/// Create Array.prototype
		/// </summary>
		public ArrayObj(IEngine engine, IObject baseClass)
			: base(engine, baseClass)
			=> Array = Empty;

		/// <summary>
		/// Create new string object boxing the string
		/// </summary>
		public ArrayObj(IEngine engine, ArrayObj baseClass, Value[] value)
			: base(engine, baseClass, new Properties(StdProps))
			=> Array = value;

		public override Value Index(IObject self, int argc)
		{
			if (argc == 1)
			{
				var i = Engine.GetArgument(argc);
				if (i.IsNumber)
					return Array[i.Int];
			}
			return base.Index(self, argc);
		}

		int ICollection<Value>.Count => Array.Length;
		bool ICollection<Value>.IsReadOnly => true;

		public bool Contains(Value item)
			=> Array.Contains(item);
		void ICollection<Value>.CopyTo(Value[] array, int arrayIndex)
			=> Array.CopyTo(array, arrayIndex);

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
		public IEnumerator<Value> GetEnumerator()
		{
			foreach (var v in Array)
				yield return v;
		}

		bool IListObject.IsWritable => true;
		bool IListObject.IsFixedSize => true;
		public Value this[int index]
		{
			get => Array[index];
			set => Array[index] = value;
		}
		public int IndexOf(Value item) => ((IList<Value>)Array).IndexOf(item);

		void ICollection<Value>.Add(Value item) => throw new NotImplementedException();
		void ICollection<Value>.Clear() => throw new NotImplementedException();
		bool ICollection<Value>.Remove(Value item) => throw new NotImplementedException();
		void IList<Value>.Insert(int index, Value item) => throw new NotImplementedException();
		void IList<Value>.RemoveAt(int index) => throw new NotImplementedException();

		public static IDictionary<string, Value> StdProps { get; } = new Dictionary<string, Value>()
		{
			{ "length", LengthProp.Value }
		};

		// also used in StringObj and ListObj
		public class LengthProp : IProperty
		{
			public static Value Value { get; } = new Value(new LengthProp());
			public Value Get(IObject obj)
				=> new Value(((ICollection<Value>)obj).Count);
			public bool Set(IObject obj, Value value)
				=> false;
		}
	}
}
