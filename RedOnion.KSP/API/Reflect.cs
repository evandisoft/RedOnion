using System;
using RedOnion.Script;
using KSP.UI.Screens;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using RedOnion.Script.ReflectedObjects;

namespace RedOnion.KSP.API
{
	public class Reflect : InteropObject
	{
		public static MemberList MemberList { get; } = new MemberList(
		ObjectFeatures.Function,

@"Reflects types provided as assembly-qualified name (""Namespace.Type,Assembly"").",

		new IMember[]
		{
			new Function("new", "object", "Construct new object given type or object and arguments.",
				() => Constructor.Instance),
			new Function("create", "object", "Alias to new().",
				() => Constructor.Instance),
			new Function("construct", "object", "Alias to new().",
				() => Constructor.Instance),
		});

		public static Reflect Instance { get; } = new Reflect();
		public Reflect() : base(MemberList) { }

		public override Value Call(IObject self, Arguments args)
		{
			if (args.Length != 1)
				throw new InvalidOperationException(args.Length == 0
					? "Expected assembly-qualified name"
					: "Too many arguments");
			return new ReflectedType(args.Engine, Type.GetType(args[0].String, true));
		}
		public override DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (args.Count != 1)
				throw new InvalidOperationException(args.Count == 0
					? "Expected assembly-qualified name"
					: "Too many arguments");
			return UserData.CreateStatic(Type.GetType(args[0].String, true));
		}

		public class Constructor : FunctionBase
		{
			public static Constructor Instance { get; } = new Constructor();
			public override Value Call(Arguments args)
			{
				if (args.Count == 0)
					throw new InvalidOperationException("Expected at least one argument");
				var obj = args[0].Object;
				Type t = (obj as Type) ?? obj.GetType();
				return new Value(new ReflectedType(args.Engine, t).Create(new Arguments(args, args.Length-1)));
			}
			public override DynValue Call(ScriptExecutionContext ctx, CallbackArguments _args)
			{
				_args = _args.SkipMethodCall();
				if (_args.Count == 0)
					throw new InvalidOperationException("Expected at least one argument");
				var obj = _args[1].ToObject();
				Type t = (obj as Type) ?? obj.GetType();

				var constructors = t.GetConstructors();
				var dynArgs = _args.GetArray(2);
				foreach (var constructor in constructors)
				{
					var parinfos = constructor.GetParameters();
					if (parinfos.Length >= dynArgs.Length)
					{
						object[] args = new object[parinfos.Length];

						for (int i = 0; i < args.Length; i++)
						{
							var parinfo = parinfos[i];
							if (i >= dynArgs.Length)
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

						return DynValue.FromObject(ctx.OwnerScript,constructor.Invoke(args));
					}
				nextConstructor:;
				}

				if (dynArgs.Length == 0)
					return DynValue.FromObject(ctx.OwnerScript, Activator.CreateInstance(t));
				throw new Exception("Could not find constructor accepting given args for type " + t);
			}
		}
	}
}
