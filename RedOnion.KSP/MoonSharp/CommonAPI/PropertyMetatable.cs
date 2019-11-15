using System;
using System.Reflection;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.MoonSharp.CommonAPI
{
	public class PropertyMetatable : Table
	{
		public PropertyMetatable(Script owner) : base(owner)
		{
			this["__index"]=new Func<Table, DynValue, object>(GetProperty);
		}

		public void SetProperty(PropertyInfo propertyInfo)
		{
			this[propertyInfo.Name]=new Func<object>(() => propertyInfo.GetValue(null));
		}

		private object GetProperty(Table table, DynValue key)
		{
			if (key.Type != DataType.String)
			{
				throw new Exception("Index for" + GetType() + " must be string");
			}

			var func=this[key] as Func<object>;
			return func();
		}
	}
}
