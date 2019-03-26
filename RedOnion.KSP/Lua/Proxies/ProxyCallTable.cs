using System;
using System.Reflection;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace RedOnion.KSP.Lua.Proxies
{
	/// <summary>
	/// Not needed anymore.
	/// </summary>
	public class ProxyCallTable : Table
	{
		public object ProxiedObject;
		MoonSharp.Interpreter.Script script;
		public string MemberName;
		public ProxyCallTable(MoonSharp.Interpreter.Script script,object proxiedObject,string memberName) : base(script)
		{
			this.script = script;
			MemberName = memberName;
			ProxiedObject = proxiedObject;

			DynValue createInvoker = script.DoString(
			@"
				return function(callFunc)
					return function(table,...)
						return callFunc({...})
					end
				end
			");
			DynValue invoker = script.Call(createInvoker, new Func<Table, object>(CallFunc));

			var metatable = new Table(script);
			metatable["__call"] = invoker;
			MetaTable = metatable;
		}



		object CallFunc(Table argTable)
		{
			Type t = ProxiedObject.GetType();
			MethodInfo mi = null;
			try
			{
				mi = t.GetMethod(MemberName);
			}
			catch (AmbiguousMatchException)
			{
				MethodInfo[] mis = t.GetMethods();
				foreach (var methodInfo in mis)
				{
					if (methodInfo.Name == MemberName)
					{
						mi = methodInfo;
						break;
					}
				}
			}
			UnityEngine.Debug.Log("args");
			List<object> args = new List<object>();
			foreach(var value in argTable.Values)
			{
				if (value.Type == DataType.Nil)
				{
					break;
				}
				args.Add(value.ToObject());
			}

			object[] arrayArgs = args.ToArray();

			return mi.Invoke(ProxiedObject, arrayArgs);
		}
	}
}
