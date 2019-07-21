using System;
using System.Collections.Generic;
using RedOnion.ROS;
using MoonSharp.Interpreter;
using RedOnion.KSP.ReflectionUtil;
using RedOnion.KSP.Completion;
using KSP.UI.Screens;
using System.ComponentModel;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	[Description(@"Reflects/imports native types provided as namespace-qualified string
(e.g. ""System.Collections.Hashtable"") or assembly-qualified name
(""UnityEngine.Vector2,UnityEngine""). It will first try `Type.GetType`,
then search in `Assembly-CSharp`, then in `UnityEngine` and then in all assemblies
returned by `AppDomain.CurrentDomain.GetAssemblies()`.")]
	public class Reflect : Descriptor, IUserDataType, ICompletable
	{
		public static Reflect Instance { get; } = new Reflect();
		protected Reflect() { }

		[Description("Construct new object given type or object and arguments."
			+ " Example: `reflect.new(\"System.Collections.ArrayList\")`.")]
		public static Constructor @new => Constructor.Instance;
		[Description("Alias to new().")]
		public static Constructor create => Constructor.Instance;
		[Description("Alias to new().")]
		public static Constructor construct => Constructor.Instance;

		[Browsable(false)]
		public class Constructor : ICallable
		{
			static internal readonly string[] names = new string[]
			{
				"new", "create", "construct"
			};
			[Browsable(false), MoonSharpHidden]
			public static Constructor Instance { get; } = new Constructor();

			bool ICallable.Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Count == 0)
					throw new InvalidOperationException("Expected at least one argument");
				var it = new Value(ResolveType(args[0]));
				return it.desc.Call(ref result, null, new Arguments(args, args.Length-1), true);
			}
			[MoonSharpUserDataMetamethod("__call"), Browsable(false)]
			public DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			{
				if (args.Count <= 1)
					throw new InvalidOperationException("Expected at least one argument");
				return DynValue.FromObject(ctx.OwnerScript, LuaNew(args[1], args.GetArray(2)));
			}
		}

		public override bool Call(ref Value result, object self, Arguments args, bool create)
		{
			if (args.Length != 1)
				throw new InvalidOperationException(args.Length == 0
					? "Expected assembly-qualified name"
					: "Too many arguments");
			result = new Value(ResolveType(args[0].ToStr()));
			return true;
		}

		[MoonSharpUserDataMetamethod("__call"), Browsable(false)]
		public DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 1)
				throw new InvalidOperationException(args.Count == 0
					? "Expected assembly-qualified name"
					: "Too many arguments");
			return UserData.CreateStatic(ResolveType(args[0].String));
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

		static NamespaceInstance _map;
		static NamespaceInstance Map => _map ?? (_map = NamespaceMappings.DefaultAssemblies.GetNamespace(""));

		DynValue IUserDataType.MetaIndex(Script script, string metaname) => null;
		bool IUserDataType.SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
			=> throw InvalidOperation("Cannot modify fields of this const dictionary.");
		DynValue IUserDataType.Index(Script script, DynValue index, bool isDirectIndexing)
		{
			if (index.Type != DataType.String)
				throw InvalidOperation("Can only index with strings");
			var name = index.String;
			foreach (var alias in Constructor.names)
				if (name.Equals(alias, StringComparison.OrdinalIgnoreCase))
					return DynValue.FromObject(script, Constructor.Instance);
			return Map.Index(script, index, isDirectIndexing);
		}

		public override int Find(object self, string name, bool add)
		{
			foreach (var alias in Constructor.names)
				if (name.Equals(alias, StringComparison.OrdinalIgnoreCase))
					return 0;
			var at = Map.RosFind(name);
			return at < 0 ? at : at + 1;
		}
		public override string NameOf(object self, int at)
			=> at == 0 ? Constructor.names[0] : Map.RosNameOf(at - 1);
		public override bool Get(ref Value self, int at)
		{
			if (at == 0)
			{
				self = new Value(Constructor.Instance);
				return true;
			}
			return Map.RosGet(ref self, at - 1);
		}
		public override IEnumerable<string> EnumerateProperties(object self)
		{
			foreach (var name in Constructor.names)
				yield return name;
			foreach (var name in Map.PossibleCompletions)
				yield return name;
		}

		IList<string> ICompletable.PossibleCompletions
		{
			get
			{
				var map = Map.PossibleCompletions;
				var it = new string[Constructor.names.Length + map.Count];
				Constructor.names.CopyTo(it, 0);
				int i = Constructor.names.Length, j = 0;
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
			completion = null;
			return false;
		}
	}
}
