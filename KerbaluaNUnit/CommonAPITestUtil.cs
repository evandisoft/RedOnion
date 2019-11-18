using System;
using MoonSharp.Interpreter;
using NUnit.Framework;

namespace KerbaluaNUnit
{
	public class CommonAPITestUtil
	{
		public static object MemberCheck(Table table, string key, Type type)
		{
			var d=GetCheck(table,key);
			Assert.NotNull(d.UserData, key+" should have been a UserData");
			Assert.NotNull(d.UserData.Object, key+" should not have been Static");
			object o=d.UserData.Object;
			Assert.IsInstanceOf(type, o);
			return o;
		}

		public static DynValue StaticCheck(Table table, string key)
		{
			var d=GetCheck(table,key);
			Assert.True(IsStatic(d), key+" should have been a static");
			return d;
		}

		public static DynValue GetCheck(Table table, string key)
		{
			DynValue d=table.Get(key);
			Assert.False(d.IsNil(), key+" should not have been nil");
			return d;
		}

		public static Table NamespaceCheck(Table table, string key)
		{
			var d=GetCheck(table,key);
			Assert.NotNull(d.Table, key+" should have been a table");
			return d.Table;
		}

		public static bool IsStatic(DynValue d)
		{
			return d.UserData!=null && d.UserData.Object==null;
		}
	}
}
