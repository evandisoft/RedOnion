using System;
using System.Collections.Generic;
using System.Reflection;
using MunSharp.Interpreter;
using MunSharp.Interpreter.Interop;
using RedOnion.Common.Completion;

namespace RedOnion.KSP.Kerbalua.Proxies
{
	public class LuaProxy:IUserDataType,IHasCompletionProxy
	{
		public object ProxiedObject;

		public LuaProxy(object ProxiedObject)
		{
			this.ProxiedObject = ProxiedObject;
		}

		public DynValue Index(global::MunSharp.Interpreter.Script script, DynValue index, bool isDirectIndexing)
		{
			if (index.Type != DataType.String)
			{
				throw new ProxyException("Index for proxied object " + ProxiedObject.GetType() + " must be string");
			}
			string memberName = index.String;

			Type t = ProxiedObject.GetType();
			FieldInfo f = t.GetField(memberName);
			if (f != null)
			{
				return DynValue.FromObject(script,f.GetValue(ProxiedObject));
			}

			PropertyInfo p = t.GetProperty(memberName);
			if (p != null)
			{
				return DynValue.FromObject(script,p.GetValue(ProxiedObject, null));
			}

			MethodInfo mi = null;
			try
			{
				mi = t.GetMethod(memberName);
			}
			catch (AmbiguousMatchException)
			{
				MethodInfo[] mis = t.GetMethods();
				foreach (var methodInfo in mis)
				{
					if (methodInfo.Name == memberName)
					{
						mi = methodInfo;
						break;
					}
				}
			}

			if (mi != null)
			{
				return DynValue.NewTable(new ProxyCallTable(script, ProxiedObject, memberName));
			}
			throw new NotImplementedException("Member " + memberName + " was not in proxied object " + ProxiedObject.GetType());
		}

		public DynValue MetaIndex(global::MunSharp.Interpreter.Script script, string metaname)
		{
			throw new NotImplementedException();
		}

		public bool SetIndex(global::MunSharp.Interpreter.Script script, DynValue index, DynValue value, bool isDirectIndexing)
		{
			throw new NotImplementedException();
		}

		public object CompletionProxy => ProxiedObject.GetType();
	}
}
