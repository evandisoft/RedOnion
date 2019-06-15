using System;
using System.Collections.Generic;
using RedOnion.ROS;
using MoonSharp.Interpreter;
using RedOnion.KSP.ReflectionUtil;
using RedOnion.KSP.Completion;
using KSP.UI.Screens;

namespace RedOnion.KSP.API
{
	// RedOnion.Builder again had some problems
	[ProxyDocs(typeof(Reflect))]
	public static class ReflectMembers
	{
		public static MemberList MemberList { get; } = new MemberList(

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
	public class Reflect : InteropObject
	{
		public static Reflect Instance { get; } = new Reflect();

		public Reflect() : base(ReflectMembers.MemberList) { }

		public override bool Call(ref Value result, object self, Arguments args, bool create)
		{
			if (args.Length != 1)
				throw new InvalidOperationException(args.Length == 0
					? "Expected assembly-qualified name"
					: "Too many arguments");
			result = new Value(ResolveType(args[0].ToStr()));
			return true;
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
			if (o is Value v)
			{
				if (v.IsString)
					return ResolveType(v.ToStr());
				return v.obj?.GetType();
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
			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Count == 0)
					throw new InvalidOperationException("Expected at least one argument");
				var it = new Value(ResolveType(args[0]));
				return it.desc.Call(ref result, null, new Arguments(args, args.Length-1), true);
			}
			public override DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			{
				if (args.Count <= 1)
					throw new InvalidOperationException("Expected at least one argument");

				return DynValue.FromObject(ctx.OwnerScript, LuaNew(args[1], args.GetArray(2)));
			}
		}

		NamespaceInstance map = NamespaceMappings.DefaultAssemblies.GetNamespace("");
		public override int Find(object self, string name, bool add)
		{
			var at = base.Find(self, name, false);
			if (at >= 0) return at;
			at = map.RosFind(name);
			return at < 0 ? at : at + Members.Count;
		}
		public override string NameOf(object self, int at)
			=> at < Members.Count ? base.NameOf(self, at) : map.RosNameOf(at - Members.Count);
		public override bool Get(ref Value self, int at)
		{
			if (at < Members.Count)
				return base.Get(ref self, at);
			return map.RosGet(ref self, at - Members.Count);
		}
		public override IEnumerable<string> EnumerateProperties(object self)
		{
			foreach (var member in Members)
				yield return member.Name;
			foreach (var name in map.PossibleCompletions)
				yield return name;
		}
		public override IList<string> PossibleCompletions
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
		public override bool TryGetCompletion(string completionName, out object completion)
			=> Members.TryGetCompletion(completionName, out completion)
			|| map.TryGetCompletion(completionName, out completion);
	}
}
