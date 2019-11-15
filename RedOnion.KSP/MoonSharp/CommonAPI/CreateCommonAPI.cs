using System;
using System.Reflection;
using MoonSharp.Interpreter;
using RedOnion.KSP.API;

namespace RedOnion.KSP.MoonSharp.CommonAPI
{
	public class CreateCommonAPI
	{
		Script script;
		public CreateCommonAPI(Script script)
		{
			this.script=script;

			//Delegate.CreateDelegate()
		}

		//public Table Create()
		//{
		//	return CreateTableFromStaticType(typeof(Globals));
		//}

		//public Table Create(Type type)
		//{

		//}

		//Table CreateTableFromStaticType(Type type)
		//{
		//	var table=new CommonAPITable(script);
		//	var metatable=new PropertyMetatable(script);
		//	table.MetaTable=metatable;
		//}

		void FillTableWithField(FieldInfo[] fieldInfos,Table table)
		{
			foreach(var fieldinfo in fieldInfos)
			{
				table[fieldinfo.Name]=fieldinfo.GetValue(null);
			}

		}

		void FillTableWithProperties(PropertyInfo[] propertyInfos, PropertyMetatable metatable)
		{
			foreach (var propertyInfo in propertyInfos)
			{
				metatable.SetProperty(propertyInfo);
			}
		}

		//void FillTableWithMethods(MethodInfo[] methodInfos, Table table)
		//{
		//	foreach (var methodInfo in methodInfos)
		//	{
		//		table[methodInfo.Name]=Globals.Equals;
		//	}
		//}
	}
}
