using System;
using System.Linq.Expressions;
using System.Reflection;
using MoonSharp.Interpreter;
using RedOnion.KSP.API;

namespace RedOnion.KSP.MoonSharp.CommonAPI
{
	/// <summary>
	/// Given a reference to a script, Create() will create a new api structure
	/// in tables and return it.
	/// </summary>
	public class CommonAPICreator
	{
		Script script;
		public CommonAPICreator(Script script)
		{
			this.script=script;
		}

		public Table Create()
		{
			return CreateTableFromStaticType(typeof(Globals));
		}

		Table CreateTableFromStaticType(Type type)
		{
			var table=new CommonAPITable(script);
			var metatable=new CommonAPIMetatable(script);
			table.MetaTable=metatable;
			var publicStatic=BindingFlags.Public | BindingFlags.Static;
			var fields=type.GetFields(publicStatic);
			FillTableWithFields(fields, table);
			var properties=type.GetProperties(publicStatic);
			FillTableWithProperties(properties, metatable);
			var methods=type.GetMethods(publicStatic);
			FillTableWithMethods(methods, table);

			return table;
		}

		void FillTableWithFields(FieldInfo[] fieldInfos,Table table)
		{
			foreach(var fieldinfo in fieldInfos)
			{
				var fieldValue=fieldinfo.GetValue(null);
				if (fieldValue.GetType().Name=="RuntimeType")
				{
					Type type=fieldValue as Type;
					// Apparently this is how to check if a type is a static class
					// https://stackoverflow.com/questions/1175888/determine-if-a-type-is-static
					if (type.IsAbstract && type.IsSealed)
					{
						table[fieldinfo.Name]=CreateTableFromStaticType(fieldValue as Type);
					}
					else
					{
						table[fieldinfo.Name]=UserData.CreateStatic(type);
					}
				}
				else
				{
					table[fieldinfo.Name]=fieldValue;
				}
			}
		}

		void FillTableWithProperties(PropertyInfo[] propertyInfos, CommonAPIMetatable metatable)
		{
			foreach (var propertyInfo in propertyInfos)
			{
				metatable.SetProperty(propertyInfo);
			}
		}

		void FillTableWithMethods(MethodInfo[] methodInfos, Table table)
		{
			foreach (var methodInfo in methodInfos)
			{
				if (!methodInfo.IsSpecialName)
				{
					table[methodInfo.Name]=GetDelegateFromMethodInfo(methodInfo);
				}
			}
		}

		Delegate GetDelegateFromMethodInfo(MethodInfo methodInfo)
		{
			var parameterInfos = methodInfo.GetParameters();
			Type[] argTypes = new Type[parameterInfos.Length+1];
			for (int i = 0; i < parameterInfos.Length; i++)
			{
				argTypes[i] = parameterInfos[i].ParameterType;
			}
			argTypes[parameterInfos.Length] = methodInfo.ReturnType;
			Type delegateType = Expression.GetDelegateType(argTypes);

			return Delegate.CreateDelegate(delegateType, methodInfo);
		}
	}
}
