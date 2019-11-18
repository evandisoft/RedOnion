using System;
using MoonSharp.Interpreter;
using NUnit.Framework;

namespace KerbaluaNUnit
{
	public class ScriptTester
	{
		Script script;
		public ScriptTester(Script script)
		{
			this.script=script;
		}

		public DynValue MemberCheck(Table table, string key, Type type)
		{
			var d=GetCheck(table,key);
			TypeCheck(d, type);
			return d;
		}

		public DynValue MethodCheck(Table table, string key)
		{
			var d=GetCheck(table,key);
			TypeCheck(d, typeof(CallbackFunction));
			return d;
		}

		public DynValue StaticCheck(Table table, string key)
		{
			var d=GetCheck(table,key);
			Assert.True(IsStatic(d), key+" should have been a static");
			return d;
		}

		public DynValue GetCheck(Table table, string key)
		{
			script.Globals["t"]=table;
			DynValue d=script.DoString($"return t['{key}']");
			Assert.False(d.IsNil(), key+" should not have been nil");
			return d;
		}

		public Table NamespaceCheck(Table table, string key)
		{
			var d=GetCheck(table,key);
			Assert.NotNull(d.Table, key+" should have been a table");
			return d.Table;
		}

		void TypeCheck(DynValue d,Type type)
		{
			object o=d.ToObject();
			Assert.IsInstanceOf(type, o);
		}

		public bool IsStatic(DynValue d)
		{
			return d.UserData!=null && d.UserData.Object==null;
		}
	}
}
