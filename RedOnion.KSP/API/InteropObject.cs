using System;
using System.Collections.Generic;
using RedOnion.Script;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	public abstract class InteropObject : IObject, IUserDataType, IType
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
			return null;
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
		Value IObject.Index(Arguments args)
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
