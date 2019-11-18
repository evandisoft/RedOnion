using System;
using System.Linq.Expressions;
using System.Reflection;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.MoonSharp.CommonAPI
{
	public class CommonAPITable : Table
	{
		public CommonAPITable(Script owner) : base(owner)
		{
			var metatable=new CommonAPIMetatable(owner);
			MetaTable=metatable;
		}

		bool IsStaticClass(Type type)
		{
			return type.IsAbstract && type.IsSealed;
		}

		public Table AddAll(Type type)
		{
			var publicStatic=BindingFlags.Public | BindingFlags.Static;
			var fields=type.GetFields(publicStatic);
			FillTableWithFields(fields);
			var properties=type.GetProperties(publicStatic);
			FillTableWithProperties(properties);
			var methods=type.GetMethods(publicStatic);
			FillTableWithMethods(methods);

			return this;
		}

		void FillTableWithFields(FieldInfo[] fieldInfos)
		{
			foreach (var fieldinfo in fieldInfos)
			{
				var fieldValue=fieldinfo.GetValue(null);
				if (fieldValue.GetType().Name=="RuntimeType")
				{
					Type type=fieldValue as Type;
					// Apparently this is how to check if a type is a static class
					// https://stackoverflow.com/questions/1175888/determine-if-a-type-is-static
					if (IsStaticClass(type))
					{
						CommonAPITable newTable=null;
						if (this[fieldinfo.Name]==null || !(this[fieldinfo.Name] is CommonAPITable))
						{
							newTable=new CommonAPITable(OwnerScript);
							this[fieldinfo.Name]=newTable;
						}
						else
						{
							newTable=this[fieldinfo.Name] as CommonAPITable;
						}
						newTable.AddAll(type);
					}
					else
					{
						this[fieldinfo.Name]=UserData.CreateStatic(type);
					}
				}
				else
				{
					this[fieldinfo.Name]=fieldValue;
				}
			}
		}

		void FillTableWithProperties(PropertyInfo[] propertyInfos)
		{
			foreach (var propertyInfo in propertyInfos)
			{
				(MetaTable as CommonAPIMetatable).SetProperty(propertyInfo);
			}
		}

		void FillTableWithMethods(MethodInfo[] methodInfos)
		{
			foreach (var methodInfo in methodInfos)
			{
				if (!methodInfo.IsSpecialName)
				{
					this[methodInfo.Name]=GetDelegateFromMethodInfo(methodInfo);
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
