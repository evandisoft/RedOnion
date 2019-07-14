using System;
using System.Collections.Generic;
using RedOnion.ROS;
using MoonSharp.Interpreter;
using RedOnion.KSP.ReflectionUtil;
using RedOnion.KSP.Completion;
using KSP.UI.Screens;
using System.ComponentModel;

namespace RedOnion.KSP.API
{
	[Description(@"Reflects/imports native types provided as namespace-qualified string
(e.g. ""System.Collections.Hashtable"") or assembly-qualified name
(""UnityEngine.Vector2,UnityEngine""). It will first try `Type.GetType`,
then search in `Assembly-CSharp`, then in `UnityEngine` and then in all assemblies
returned by `AppDomain.CurrentDomain.GetAssemblies()`.")]
	public class Reflect : ISelfDescribing, IHasCompletionProxy
	{
		public static Reflect Instance { get; } = new Reflect();
		protected Reflect() { }

		[Description("Construct new object given type or object and arguments."
			+ " Example: `reflect.new(\"System.Collections.ArrayList\")`.")]
		public static Constructor New => Constructor.Instance;
		[Description("Alias to new().")]
		public static Constructor Create => Constructor.Instance;
		[Description("Alias to new().")]
		public static Constructor Construct => Constructor.Instance;

		[Browsable(false)]
		public class Constructor : ICallable
		{
			[Browsable(false), MoonSharpHidden]
			public static Constructor Instance { get; } = new Constructor();
			bool ICallable.Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Count == 0)
					throw new InvalidOperationException("Expected at least one argument");
				var it = new Value(ResolveType(args[0]));
				return it.desc.Call(ref result, null, new Arguments(args, args.Length-1), true);
			}
			[MoonSharpUserDataMetamethod("__call")]
			static DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			{
				if (args.Count <= 1)
					throw new InvalidOperationException("Expected at least one argument");
				return DynValue.FromObject(ctx.OwnerScript, LuaNew(args[1], args.GetArray(2)));
			}
		}

		[Browsable(false), MoonSharpHidden]
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
		[Browsable(false), MoonSharpHidden]
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

		[Browsable(false), MoonSharpHidden]
		public static object LuaNew(object obj, params DynValue[] dynArgs)
			=> LuaNew(ResolveType(obj), dynArgs);
		[Browsable(false), MoonSharpHidden]
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

		[MoonSharpUserDataMetamethod("__call")]
		static DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 1)
				throw new InvalidOperationException(args.Count == 0
					? "Expected assembly-qualified name"
					: "Too many arguments");
			return UserData.CreateStatic(ResolveType(args[0].String));
		}

		SelfDescriptor descriptor;
		Descriptor ISelfDescribing.Descriptor => descriptor ?? (descriptor = new SelfDescriptor());
		object IHasCompletionProxy.CompletionProxy => descriptor ?? (descriptor = new SelfDescriptor());

		class SelfDescriptor : Descriptor.Reflected, ICompletable
		{
			static NamespaceInstance _map;
			static NamespaceInstance Map => _map ?? (_map = NamespaceMappings.DefaultAssemblies.GetNamespace(""));
			public SelfDescriptor() : base(typeof(Reflect)) { }
			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Length != 1)
					throw new InvalidOperationException(args.Length == 0
						? "Expected assembly-qualified name"
						: "Too many arguments");
				result = new Value(ResolveType(args[0].ToStr()));
				return true;
			}
			public override int Find(object self, string name, bool add)
			{
				var at = base.Find(self, name, false);
				if (at >= 0) return at;
				at = Map.RosFind(name);
				return at < 0 ? at : at + prop.Count;
			}
			public override string NameOf(object self, int at)
				=> at < prop.Count ? base.NameOf(self, at) : Map.RosNameOf(at - prop.Count);
			public override bool Get(ref Value self, int at)
			{
				if (at < prop.Count)
					return base.Get(ref self, at);
				return Map.RosGet(ref self, at - prop.Count);
			}
			public override IEnumerable<string> EnumerateProperties(object self)
			{
				foreach (var p in prop)
					yield return p.name;
				foreach (var name in Map.PossibleCompletions)
					yield return name;
			}

			IList<string> ICompletable.PossibleCompletions
			{
				get
				{
					var map = Map.PossibleCompletions;
					var it = new string[prop.Count+map.Count];
					int i = 0;
					while (i < prop.Count)
					{
						it[i] = prop[i].name;
						i++;
					}
					int j = 0;
					while (i < it.Length)
						it[i++] = map[j++];
					return it;
				}
			}
			bool ICompletable.TryGetCompletion(string completionName, out object completion)
			{
				var at = Find(null, completionName, false);
				if (at >= 0)
				{
					var it = Value.Void;
					if (Get(ref it, at))
					{
						completion = it.Box();
						return true;
					}
				}
				return Map.TryGetCompletion(completionName, out completion);
			}
		}
	}
}
