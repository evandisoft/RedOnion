using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.Lua.Proxies
{
	public class ProxyException : Exception
	{
		public ProxyException(string message) : base(message)
		{
		}
	}

	public class ProxyTable : Table
	{
		public object ProxiedObject;
		MoonSharp.Interpreter.Script script;
		public ProxyTable(MoonSharp.Interpreter.Script script, object proxied) : base(script)
		{
			this.script = script;
			ProxiedObject = proxied;
			Init();
			var metatable = new Table(script);
			metatable["__index"] = new Func<Table, DynValue, object>(IndexFunc);
			metatable["__tostring"] = new Func<Table, DynValue>(ToString);
			MetaTable = metatable;
		}

		DynValue Index(Table table,DynValue key)
		{
			return key;
		}

		void Init()
		{

		}

		DynValue ToString(Table table)
		{
			return DynValue.NewString(ProxiedObject.ToString());
		}

		object IndexFunc(Table table, DynValue key)
		{
			if (key.Type != DataType.String)
			{
				throw new ProxyException("Index for proxied object " + ProxiedObject.GetType() + " must be string");
			}
			string memberName = key.String;

			Type t = ProxiedObject.GetType();
			FieldInfo f=t.GetField(memberName);
			if (f != null)
			{
				return f.GetValue(ProxiedObject);
			}

			PropertyInfo p = t.GetProperty(memberName);
			if (p != null)
			{
				return p.GetValue(ProxiedObject, null);
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
				return new ProxyCallTable(script, ProxiedObject, memberName);
			}
			throw new NotImplementedException("Member "+memberName+" was not in proxied object "+ProxiedObject.GetType());
		}
	}
}
