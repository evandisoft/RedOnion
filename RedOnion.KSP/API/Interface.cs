using System;
using System.Collections.Generic;
using RedOnion.Script;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	public class ProxyDocsAttribute : Attribute
	{
		public Type ForType { get; }
		public ProxyDocsAttribute(Type type)
		{
			ForType = type;
		}
	}
	public class IgnoreForDocsAttribute : Attribute
	{
	}
	public interface IType
	{
		string Help { get; }
		ObjectFeatures Features { get; }
		IMember[] MemberList { get; }
		Dictionary<string, IMember> Members { get; }
	}
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
			=> Set(value.Int);
		public void LuaSet(object self, DynValue value)
			=> Set((int)value.Number);

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
}
