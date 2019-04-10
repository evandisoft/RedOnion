using System;
using RedOnion.Script;
using KSP.UI.Screens;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using RedOnion.Script.ReflectedObjects;
using RedOnion.KSP.ReflectionUtil;
using RedOnion.KSP.Completion;

namespace RedOnion.KSP.API
{
	// RedOnion.Builder again had some problems
	[ProxyDocs(typeof(Reflect))]
	public static class ReflectMembers
	{
		public static MemberList MemberList { get; } = new MemberList(
		ObjectFeatures.Function,

@"Reflects/imports native types provided as namespace-qualified string
(e.g. ""System.Collections.Hashtable"") or assembly-qualified name
(""UnityEngine.Vector2,UnityEngine""). It will first try `Type.GetType`,
then search in `Assembly-CSharp`, then in `UnityEngine` and then in all assemblies
returned by `AppDomain.CurrentDomain.GetAssemblies()`.",

		new IMember[]
		{
			new Function("new", "object",
@"Construct new object given type or object and arguments.
Example: `reflect.new(""System.Collections.ArrayList"")`.",
				() => Reflect.Constructor.Instance),
			new Function("create", "object", "Alias to new().",
				() => Reflect.Constructor.Instance),
			new Function("construct", "object", "Alias to new().",
				() => Reflect.Constructor.Instance),
		});
	}

	[IgnoreForDocs]
	public class Reflect : InteropObject, ICompletable
	{
		public static Reflect Instance { get; } = new Reflect();

		public Reflect() : base(ReflectMembers.MemberList) { }

		public override Value Call(IObject self, Arguments args)
		{
			if (args.Length != 1)
				throw new InvalidOperationException(args.Length == 0
					? "Expected assembly-qualified name"
					: "Too many arguments");
			return new ReflectedType(args.Engine, ResolveType(args[0].String));
		}
		public override DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 1)
				throw new InvalidOperationException(args.Count == 0
					? "Expected assembly-qualified name"
					: "Too many arguments");
			return UserData.CreateStatic(ResolveType(args[0].String));
		}

		public static Type ResolveType(string s)
		{
			var t = Type.GetType(s, false);
			if (t != null) return t;
			t = typeof(Vector3d).Assembly.GetType(s, false);
			if (t != null) return t;
			t = typeof(UnityEngine.Vector3).Assembly.GetType(s, false);
			if (t != null) return t;
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				t = assembly.GetType(s, false);
				if (t != null) return t;
			}
			throw new InvalidOperationException("Could not resolve " + s + " to type");
		}
		public static Type ResolveType(object o)
		{
			if (o is Type t)
				return t;
			if (o is string s)
				return ResolveType(s);
			if (o is IObject obj)
			{
				t = obj.Type;
				if (t != null)
					return t;
			}
			if (o is Value v)
			{
				if (v.IsString)
					return ResolveType(v.String);
				return v.Native?.GetType();
			}
			if (o is DynValue dyn)
			{
				if (dyn.Type == DataType.UserData)
					return dyn.UserData.Descriptor.Type;
				if (dyn.Type == DataType.String)
					return ResolveType(dyn.String);
				return dyn.ToObject()?.GetType();
			}
			return o?.GetType();
		}

		public static object LuaNew(object obj, params DynValue[] dynArgs)
			=> LuaNew(ResolveType(obj), dynArgs);
		public static object LuaNew(Type t, params DynValue[] dynArgs)
		{
			var constructors = t.GetConstructors();
			foreach (var constructor in constructors)
			{
				var parinfos = constructor.GetParameters();
				if (parinfos.Length >= dynArgs.Length)
				{
					object[] args = new object[parinfos.Length];

					for (int i = 0; i < args.Length; i++)
					{
						var parinfo = parinfos[i];
						if (i>= dynArgs.Length)
						{
							if (!parinfo.IsOptional)
							{
								goto nextConstructor;
							}
							args[i] = parinfo.DefaultValue;
						}
						else
						{
							if (parinfo.ParameterType.IsValueType)
							{
								try
								{
									args[i] = System.Convert.ChangeType(dynArgs[i].ToObject(), parinfo.ParameterType);
								}
								catch (Exception)
								{
									goto nextConstructor;
								}
							}
							else
							{
								args[i] = dynArgs[i].ToObject();
							}
						}

					}

					return constructor.Invoke(args);
				}
			nextConstructor:;
			}

			if (dynArgs.Length == 0)
			{
				return Activator.CreateInstance(t);
			}
			throw new Exception("Could not find constructor accepting given args for type " + t);
		}


		public class Constructor : FunctionBase
		{
			public static Constructor Instance { get; } = new Constructor();
			public override Value Call(Arguments args)
			{
				if (args.Count == 0)
					throw new InvalidOperationException("Expected at least one argument");
				return new Value(new ReflectedType(args.Engine, ResolveType(args[0]))
					.Create(new Arguments(args, args.Length-1)));
			}
			public override DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			{
				if (args.Count <= 1)
					throw new InvalidOperationException("Expected at least one argument");
				return DynValue.FromObject(ctx.OwnerScript, LuaNew(args[1], args.GetArray(2)));


			}
		}

		NamespaceInstance map = NamespaceMappings.ForAllAssemblies.GetNamespace("");
		public override bool Has(string name) => base.Has(name) || map.Has(name);
		public override bool Get(string name, out Value value)
			=> base.Get(name, out value) || map.Get(name, out value);

		public IList<string> PossibleCompletions
		{
			get
			{
				var one = Members;
				var two = map.PossibleCompletions;
				var it = new string[one.Count+two.Count];
				int i = 0;
				while (i < one.Count)
				{
					it[i] = one[i].Name;
					i++;
				}
				int j = 0;
				while (i < it.Length)
					it[i++] = two[j++];
				return it;
			}
		}
		public bool TryGetCompletion(string completionName, out object completion)
			=> map.TryGetCompletion(completionName, out completion);
	}
}
