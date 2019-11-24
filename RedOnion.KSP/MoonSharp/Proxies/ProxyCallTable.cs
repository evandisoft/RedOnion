using System;
using System.Reflection;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace RedOnion.KSP.MoonSharp.Proxies
{
	/// <summary>
	/// Not needed anymore.
	/// </summary>
	public class ProxyCallTable : Table
	{
		public object ProxiedObject;
		global::MoonSharp.Interpreter.Script script;
		public string MemberName;

		public ProxyCallTable(global::MoonSharp.Interpreter.Script script,object proxiedObject,string memberName) : base(script)
		{
			this.script = script;
			MemberName = memberName;
			ProxiedObject = proxiedObject;

			DynValue createInvoker = script.DoString(
			@"
				return function(callFunc)
					return function(table,...)
						local t=callFunc({...})
						local r=t[1]
						t[1]=1
						if(#t>1) then
							return r,unpack(t,2)
						else
							return r
						end
					end
				end
			");
			DynValue invoker = script.Call(createInvoker, new Func<Table, Table>(CallFunc));
			
			var metatable = new Table(script);
			metatable["__call"] = invoker;
			MetaTable = metatable;
		}



		Table CallFunc(Table argTable)
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
				if (mi == null)
				{
					throw new Exception("MethodInfo null in ProxyCallTable for type " + ProxiedObject.GetType() + " method " + MemberName);
				}
			}
			//UnityEngine.Debug.Log("args");
			int i = 0;
			object[] args = new object[mi.GetParameters().Length];
			foreach(var value in argTable.Values)
			{
				if (value.Type == DataType.Nil)
				{
					break;
				}
				args[i++]=value.ToObject();
			}

			var pis = mi.GetParameters();
			List<int> OutParamIndices = new List<int>();
			for(i = 0; i < pis.Length; i++)
			{
				if (pis[i].IsOut)
				{
					OutParamIndices.Add(i);
				}
			}

			object retValue=mi.Invoke(ProxiedObject, args);
			Table returnVals = new Table(script);
			returnVals[1] = retValue;
			for(i = 0; i < OutParamIndices.Count; i++)
			{
				returnVals[i + 2] = args[OutParamIndices[i]];
			}
			return returnVals;
		}
	}
}
