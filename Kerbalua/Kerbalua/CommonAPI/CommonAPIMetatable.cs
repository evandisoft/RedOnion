using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.Common.Completion;

namespace Kerbalua.CommonAPI
{
	/// <summary>
	/// Property metatable.
	/// </summary>
	public class CommonAPIMetatable : Table, ICompletable
	{
		public CommonAPIMetatable(Script owner) : base(owner)
		{
			this["__index"]=new Func<Table, DynValue, DynValue>(GetProperty);
		}

		public IList<string> PossibleCompletions => properties.Keys.ToList();
		Dictionary<string,Func<DynValue>> properties=new Dictionary<string,Func<DynValue>>();
		Dictionary<string,PropertyInfo> propertyInfos=new Dictionary<string,PropertyInfo>();

		public void SetProperty(PropertyInfo propertyInfo)
		{
			properties[propertyInfo.Name]=new Func<DynValue>(() =>
			{
				var value=propertyInfo.GetValue(null);

				return DynValue.FromObject(OwnerScript, value);
			});
			propertyInfos[propertyInfo.Name]=propertyInfo;
		}

		public bool TryGetCompletion(string completionName, out object completion)
		{
			var dynValue=GetProperty(this, DynValue.NewString(completionName));

			if (dynValue.IsNil())
			{
				PropertyInfo propertyInfo=propertyInfos[completionName];
				if (propertyInfo==null)
				{
					completion=null;
					return false;
				}
				Type propertyType=propertyInfo.PropertyType;
				completion=new InstanceStatic(propertyType);
				return true;
			}

			completion=dynValue.ToObject();
			return true;
		}

		private DynValue GetProperty(Table table, DynValue key)
		{
			if (key.Type != DataType.String)
			{
				throw new Exception("Index for" + GetType() + " must be string");
			}

			var str=key.String;
			if (!properties.ContainsKey(str))
			{
				return DynValue.Nil;
			}
			var func=properties[str];
			var value=func();

			//func.GetMethodInfo().ReturnType
			return value;
		}
	}
}
