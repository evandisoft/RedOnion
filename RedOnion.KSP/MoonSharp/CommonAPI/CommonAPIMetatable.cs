using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Completion;

namespace RedOnion.KSP.MoonSharp.CommonAPI
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
			completion=GetProperty(this, DynValue.NewString(completionName));

			return completion!=null;
		}

		private DynValue GetProperty(Table table, DynValue key)
		{
			if (key.Type != DataType.String)
			{
				throw new Exception("Index for" + GetType() + " must be string");
			}

			var str=key.String;
			var func=properties[str];
			var value=func();

			if (value.IsNil())
			{
				Type propertyType=propertyInfos[str].PropertyType;
				return DynValue.FromObject(OwnerScript, new InstanceStatic(propertyType));
			}
			//func.GetMethodInfo().ReturnType
			return value;
		}
	}
}
