using System;
using System.Collections.Generic;
using RedOnion.Script;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	public abstract class InteropObject : IObject, IUserDataType
	{
		public abstract string Help { get; }

		public ObjectFeatures Features { get; }
		public IMember[] MemberList { get; }
		public Dictionary<string, IMember> Members { get; }
		public InteropObject(ObjectFeatures features, IMember[] members)
		{
			Features = features;
			if (members == null || members.Length == 0)
			{
				MemberList = NoMembers;
				Members = EmptyMemberDictionary;
			}
			else
			{
				Members = new Dictionary<string, IMember>(members.Length);
				foreach (var member in members)
					Members.Add(member.Name, member);
			}
		}

		public static readonly IMember[] NoMembers = new IMember[0];
		public static readonly Dictionary<string, IMember>
			EmptyMemberDictionary = new Dictionary<string, IMember>();

		public interface IMember
		{
			string Name { get; }
			string Type { get; }
			string Help { get; }
			bool CanRead { get; }
			bool CanWrite { get; }

			Value RosGet(InteropObject self);
			DynValue LuaGet(InteropObject self);
			void RosSet(InteropObject self, Value value);
			void LuaSet(InteropObject self, DynValue value);
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

			public Value RosGet(InteropObject self)
				=> new Value(Get());
			public DynValue LuaGet(InteropObject self)
				=> DynValue.NewBoolean(Get());
			public void RosSet(InteropObject self, Value value)
				=> Set(value.Bool);
			public void LuaSet(InteropObject self, DynValue value)
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

			public Value RosGet(InteropObject self)
				=> new Value(Get());
			public DynValue LuaGet(InteropObject self)
				=> DynValue.NewNumber(Get());
			public void RosSet(InteropObject self, Value value)
				=> Set(value.Int);
			public void LuaSet(InteropObject self, DynValue value)
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

			public Value RosGet(InteropObject self)
				=> new Value(Get());
			public DynValue LuaGet(InteropObject self)
				=> DynValue.NewNumber(Get());
			public void RosSet(InteropObject self, Value value)
				=> Set(value.Int);
			public void LuaSet(InteropObject self, DynValue value)
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

		bool IProperties.Has(string name)
			=> Members.ContainsKey(name);
		Value IProperties.Get(string name)
		{
			if (!Get(name, out var value))
				throw new NotImplementedException(name + " does not exist");
			return value;
		}
		public virtual bool Get(string name, out Value value)
		{
			if (Members.TryGetValue(name, out var member) && member.CanRead)
			{
				value = member.RosGet(this);
				return true;
			}
			value = new Value();
			return false;
		}

		public virtual bool Set(string name, Value value)
		{
			if (Members.TryGetValue(name, out var member) && member.CanWrite)
			{
				member.RosSet(this, value);
				return true;
			}
			return false;
		}

		public virtual DynValue Index(MoonSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
		{
			if (Members.TryGetValue(index.String, out var member) && member.CanRead)
				return member.LuaGet(this);
			return DynValue.Nil;
		}

		public virtual bool SetIndex(MoonSharp.Interpreter.Script script, DynValue index, DynValue value, bool isDirectIndexing)
		{
			if (Members.TryGetValue(index.String, out var member) && member.CanWrite)
			{
				member.LuaSet(this, value);
				return true;
			}
			return false;
		}

		public virtual Value Call(IObject self, Arguments args)
			=> new Value();
		public virtual DynValue MetaIndex(MoonSharp.Interpreter.Script script, string metaname)
			=> null;

		bool IObject.Modify(string name, OpCode op, Value value) => false;
		bool IProperties.Delete(string name) => false;
		void IProperties.Reset() { }

		IObject IObject.Create(Arguments args)
			=> null;
		Value IObject.Index(IObject self, int argc)
			=> throw new NotImplementedException();
		Value IObject.IndexGet(Value index)
			=> throw new NotImplementedException();
		bool IObject.IndexSet(Value index, Value value)
			=> throw new NotImplementedException();
		bool IObject.IndexModify(Value index, OpCode op, Value value)
			=> throw new NotImplementedException();
		bool IObject.Operator(OpCode op, Value arg, bool selfRhs, out Value result)
			=> throw new NotImplementedException();

		string IObject.Name => GetType().Name;
		IEngine IObject.Engine => null;
		IObject IObject.BaseClass => null;
		IProperties IObject.BaseProps => null;
		IProperties IObject.MoreProps => null;
		Value IObject.Value => new Value(GetType().FullName);
		Type IObject.Type => null;
		object IObject.Target => null;
		IObject IObject.Convert(object value) => null;
	}
}
