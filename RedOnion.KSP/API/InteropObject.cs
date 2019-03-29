using System;
using System.Collections.Generic;
using RedOnion.Script;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.API
{
	public abstract class InteropObject : Table, IObject
	{
		public abstract string Help { get; }

		public ObjectFeatures Features { get; }
		public Member[] MemberList { get; }
		public Dictionary<string, Member> Members { get; }
		public InteropObject(ObjectFeatures features, Member[] members)
			: base(null)
		{
			Features = features;
			MemberList = members;
			Members = new Dictionary<string, Member>(members.Length);
			foreach (var member in members)
				Members.Add(member.Name, member);
			MetaTable = new Table(null)
			{
				["__index"] = new Func<Table, DynValue, DynValue>(LuaGet)
			};
		}

		public class Member
		{
			public string Name { get; }
			public string Help { get; }
			public Func<object> Get { get; }
			public Action<object> Set { get; }

			public Member(string name, string help, Func<object> read, Action<object> write = null)
			{
				Name = name;
				Help = help;
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
		public bool Get(string name, out Value value)
		{
			if (Members.TryGetValue(name, out var member)
				&& member.Get != null)
			{
				value = Value.FromPrimitive(member.Get());
				return true;
			}
			value = new Value();
			return false;
		}

		DynValue LuaGet(Table table, DynValue key)
		{
			if (Members.TryGetValue(key.String, out var member)
				&& member.Get != null)
				return DynValue.FromObject(table.OwnerScript, member.Get());
			return DynValue.Nil;
		}

		public bool Set(string name, Value value)
		{
			if (Members.TryGetValue(name, out var member)
				&& member.Set != null)
			{
				member.Set(value.Native);
				return true;
			}
			return false;
		}

		public virtual Value Call(IObject self, int argc)
			=> new Value();

		bool IObject.Modify(string name, OpCode op, Value value) => false;
		bool IProperties.Delete(string name) => false;
		void IProperties.Reset() { }

		IObject IObject.Create(int argc)
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
