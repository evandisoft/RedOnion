using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using MoonSharp.Interpreter;
using RedOnion.KSP.API;
using RedOnion.ROS;

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

		const BindingFlags publicStatic=BindingFlags.Public | BindingFlags.Static;
		public Table AddAPI(Type type)
		{
			var fields=type.GetFields(publicStatic);
			FillTableWithFields(fields);
			var properties=type.GetProperties(publicStatic);
			FillTableWithProperties(properties);
			var methods=type.GetMethods(publicStatic);
			FillTableWithMethods(methods);

			var callable=type.GetCustomAttribute<CallableAttribute>();
			if (callable!=null)
			{
				var methodInfo=type.GetMethod(callable.Name,publicStatic);

				if (methodInfo==null)
				{
					methodInfo=type.GetMethod("get_"+callable.Name);
				}
				Delegate dele=GetDelegateFromMethodInfo(methodInfo);

				// createInvoker is a script that takes a function, f, and returns
				// a new function, g, that, when called, will call f with
				// its arguments after the first argument.
				DynValue createInvoker = OwnerScript.DoString(
				@"
					return function(callFunc)
						return function(table,...)
							return callFunc(...)
						end
					end
				");
				DynValue invoker = OwnerScript.Call(createInvoker,dele);
				MetaTable["__call"]=invoker;
			}

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
						newTable.AddAPI(type);
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
