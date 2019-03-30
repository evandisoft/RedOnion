using System;
using System.Collections.Generic;
using RedOnion.Script;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.Script.Utilities;
using System.Collections;

namespace RedOnion.KSP.API
{
	/// <summary>
	/// Used for <c>Globals</c> and classes derived directly from <c>Table</c>
	/// to avoid disturbing MoonSharp in RedOnion.Builder. See <c>GlobalMembers</c> for example.
	/// </summary>
	public class ProxyDocsAttribute : Attribute
	{
		public Type ForType { get; }
		public ProxyDocsAttribute(Type type)
			=> ForType = type;
	}
	/// <summary>
	/// Ignore this class in RedOnion.Builder (e.g. RuntimeRoot).
	/// </summary>
	public class IgnoreForDocsAttribute : Attribute
	{
	}
	/// <summary>
	/// Sorted list/dictionary of object members with all the documentation.
	/// </summary>
	public class MemberList : IEnumerable<IMember>
	{
		//TODO: function signatures
		public string Help { get; }
		IMember[] list;
		Dictionary<string, IMember> dict;
		class Comparer : IComparer<IMember>, IComparer
		{
			public static Comparer Instance { get; } = new Comparer();
			public int Compare(IMember x, IMember y)
				=> x.Name.CompareTo(y.Name);
			public int Compare(object x, object y)
			{
				var xname = (x as IMember)?.Name ?? x.ToString();
				var yname = (y as IMember)?.Name ?? y.ToString();
				return xname.CompareTo(yname);
			}
		}
		public MemberList(string help, IMember[] members)
		{
			Help = help;
			Array.Sort(members, Comparer.Instance);
			list = members;
			dict = new Dictionary<string, IMember>(list.Length);
			foreach (var member in members)
				dict.Add(member.Name, member);
		}
		public IEnumerator<IMember> GetEnumerator()
		{
			for (int i = 0; i < list.Length; i++)
				yield return list[i];
		}
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
		public int Count => list.Length;
		public int Length => list.Length;
		public IMember this[int i] => list[i];
		public IMember this[string name]
		{
			get => dict.TryGetValue(name, out var it) ? it : null;
		}
		public int IndexOf(string name)
			=> Array.BinarySearch(list, name, Comparer.Instance);
		public bool Contains(string name)
			=> dict.ContainsKey(name);
		public bool TryGetValue(string name, out IMember member)
			=> dict.TryGetValue(name, out member);
	}
	/// <summary>
	/// Documentation and list of members for the type.
	/// </summary>
	public interface IType
	{
		ObjectFeatures Features { get; }
		MemberList Members { get; }
	}
	/// <summary>
	/// Class member (property, methods are read-only properties returning callable object).
	/// </summary>
	public interface IMember
	{
		string Name { get; }
		string Type { get; }
		string Help { get; }
		bool CanRead { get; }
		bool CanWrite { get; }

		Value RosGet(object self);
		DynValue LuaGet(object self);
		void RosSet(object self, Value value);
		void LuaSet(object self, DynValue value);
	}
	/// <summary>
	/// Static boolean property.
	/// </summary>
	public class Bool : IMember
	{
		public string Name { get; }
		public string Type { get; }
		public string Help { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }

		public Func<bool> Get { get; }
		public Action<bool> Set { get; }

		public Value RosGet(object self)
			=> new Value(Get());
		public DynValue LuaGet(object self)
			=> DynValue.NewBoolean(Get());
		public void RosSet(object self, Value value)
			=> Set(value.Bool);
		public void LuaSet(object self, DynValue value)
			=> Set(value.Boolean);

		public Bool(string name, string help,
			Func<bool> read, Action<bool> write = null)
		{
			Name = name;
			Type = "bool";
			Help = help;
			CanRead = read != null;
			CanWrite = write != null;
			Get = read;
			Set = write;
		}
	}
	/// <summary>
	/// Boolean property.
	/// </summary>
	public class Bool<T> : IMember where T : InteropObject
	{
		public string Name { get; }
		public string Type { get; }
		public string Help { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }

		public Func<T, bool> Get { get; }
		public Action<T, bool> Set { get; }

		public Value RosGet(object self)
			=> new Value(Get((T)self));
		public DynValue LuaGet(object self)
			=> DynValue.NewBoolean(Get((T)self));
		public void RosSet(object self, Value value)
			=> Set((T)self, value.Bool);
		public void LuaSet(object self, DynValue value)
			=> Set((T)self, value.Boolean);

		public Bool(string name, string help,
			Func<T, bool> read, Action<T, bool> write = null)
		{
			Name = name;
			Type = "bool";
			Help = help;
			CanRead = read != null;
			CanWrite = write != null;
			Get = read;
			Set = write;
		}
	}
	/// <summary>
	/// Static integer property.
	/// </summary>
	public class Int : IMember
	{
		public string Name { get; }
		public string Type { get; }
		public string Help { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }

		public Func<int> Get { get; }
		public Action<int> Set { get; }

		public Value RosGet(object self)
			=> new Value(Get());
		public DynValue LuaGet(object self)
			=> DynValue.NewNumber(Get());
		public void RosSet(object self, Value value)
			=> Set(value.Int);
		public void LuaSet(object self, DynValue value)
			=> Set((int)value.Number);

		public Int(string name, string help,
			Func<int> read, Action<int> write = null)
		{
			Name = name;
			Type = "int";
			Help = help;
			CanRead = read != null;
			CanWrite = write != null;
			Get = read;
			Set = write;
		}
	}
	/// <summary>
	/// Integer property.
	/// </summary>
	public class Int<T> : IMember where T : InteropObject
	{
		public string Name { get; }
		public string Type { get; }
		public string Help { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }

		public Func<T, int> Get { get; }
		public Action<T, int> Set { get; }

		public Value RosGet(object self)
			=> new Value(Get((T)self));
		public DynValue LuaGet(object self)
			=> DynValue.NewNumber(Get((T)self));
		public void RosSet(object self, Value value)
			=> Set((T)self, value.Int);
		public void LuaSet(object self, DynValue value)
			=> Set((T)self, (int)value.Number);

		public Int(string name, string help,
			Func<T, int> read, Action<T, int> write = null)
		{
			Name = name;
			Type = "int";
			Help = help;
			CanRead = read != null;
			CanWrite = write != null;
			Get = read;
			Set = write;
		}
	}
	/// <summary>
	/// Static floating-point property.
	/// </summary>
	public class Float : IMember
	{
		public string Name { get; }
		public string Type { get; }
		public string Help { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }

		public Func<float> Get { get; }
		public Action<float> Set { get; }

		public Value RosGet(object self)
			=> new Value(Get());
		public DynValue LuaGet(object self)
			=> DynValue.NewNumber(Get());
		public void RosSet(object self, Value value)
			=> Set(value.Float);
		public void LuaSet(object self, DynValue value)
			=> Set((float)value.Number);

		public Float(string name, string help,
			Func<float> read, Action<float> write = null)
		{
			Name = name;
			Type = "float";
			Help = help;
			CanRead = read != null;
			CanWrite = write != null;
			Get = read;
			Set = write;
		}
	}
	/// <summary>
	/// Floating-point property.
	/// </summary>
	public class Float<T> : IMember where T : InteropObject
	{
		public string Name { get; }
		public string Type { get; }
		public string Help { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }

		public Func<T, float> Get { get; }
		public Action<T, float> Set { get; }

		public Value RosGet(object self)
			=> new Value(Get((T)self));
		public DynValue LuaGet(object self)
			=> DynValue.NewNumber(Get((T)self));
		public void RosSet(object self, Value value)
			=> Set((T)self, value.Float);
		public void LuaSet(object self, DynValue value)
			=> Set((T)self, (float)value.Number);

		public Float(string name, string help,
			Func<T, float> read, Action<T, float> write = null)
		{
			Name = name;
			Type = "float";
			Help = help;
			CanRead = read != null;
			CanWrite = write != null;
			Get = read;
			Set = write;
		}
	}
	/// <summary>
	/// Static property getting/setting C#-native object.
	/// </summary>
	public class Native : IMember
	{
		public string Name { get; }
		public string Type { get; }
		public string Help { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }

		public Func<object> Get { get; }
		public Action<object> Set { get; }

		public Value RosGet(object self)
			=> Value.AsNative(Get());
		public DynValue LuaGet(object self)
			=> DynValue.FromObject(null, Get());
		public void RosSet(object self, Value value)
			=> Set(value.Native);
		public void LuaSet(object self, DynValue value)
			=> Set(value.ToObject());

		public Native(string name, string type, string help,
			Func<object> read, Action<object> write = null)
		{
			Name = name;
			Type = type;
			Help = help;
			CanRead = read != null;
			CanWrite = write != null;
			Get = read;
			Set = write;
		}
	}
	/// <summary>
	/// Property getting/setting C#-native object.
	/// </summary>
	public class Native<T> : IMember where T : InteropObject
	{
		public string Name { get; }
		public string Type { get; }
		public string Help { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }

		public Func<T, object> Get { get; }
		public Action<T, object> Set { get; }

		public Value RosGet(object self)
			=> Value.AsNative(Get((T)self));
		public DynValue LuaGet(object self)
			=> DynValue.FromObject(null, Get((T)self));
		public void RosSet(object self, Value value)
			=> Set((T)self, value.Native);
		public void LuaSet(object self, DynValue value)
			=> Set((T)self, value.ToObject());

		public Native(string name, string type, string help,
			Func<T, object> read, Action<T, object> write = null)
		{
			Name = name;
			Type = type;
			Help = help;
			CanRead = read != null;
			CanWrite = write != null;
			Get = read;
			Set = write;
		}
	}
	/// <summary>
	/// Static property getting/setting interop object (usable by both ROS and LUA).
	/// </summary>
	public class Interop : IMember
	{
		public string Name { get; }
		public string Type { get; }
		public string Help { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }

		public Func<InteropObject> Get { get; }
		public Action<InteropObject> Set { get; }

		public Value RosGet(object self)
			=> new Value(Get());
		public DynValue LuaGet(object self)
			=> DynValue.FromObject(null, Get());
		public void RosSet(object self, Value value)
			=> Set((InteropObject)value.Object);
		public void LuaSet(object self, DynValue value)
			=> Set((InteropObject)value.ToObject());

		public Interop(string name, string type, string help,
			Func<InteropObject> read, Action<InteropObject> write = null)
		{
			Name = name;
			Type = type;
			Help = help;
			CanRead = read != null;
			CanWrite = write != null;
			Get = read;
			Set = write;
		}
	}
	/// <summary>
	/// Property getting/setting interop object (usable by both ROS and LUA).
	/// </summary>
	public class Interop<T> : IMember where T : InteropObject
	{
		public string Name { get; }
		public string Type { get; }
		public string Help { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }

		public Func<T, InteropObject> Get { get; }
		public Action<T, InteropObject> Set { get; }

		public Value RosGet(object self)
			=> new Value(Get((T)self));
		public DynValue LuaGet(object self)
			=> DynValue.FromObject(null, Get((T)self));
		public void RosSet(object self, Value value)
			=> Set((T)self, (InteropObject)value.Object);
		public void LuaSet(object self, DynValue value)
			=> Set((T)self, (InteropObject)value.ToObject());

		public Interop(string name, string type, string help,
			Func<T, InteropObject> read, Action<T, InteropObject> write = null)
		{
			Name = name;
			Type = type;
			Help = help;
			CanRead = read != null;
			CanWrite = write != null;
			Get = read;
			Set = write;
		}
	}
}
