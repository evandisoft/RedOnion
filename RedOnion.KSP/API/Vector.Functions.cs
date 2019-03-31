using MoonSharp.Interpreter;
using RedOnion.Script;
using System;

namespace RedOnion.KSP.API
{
	public partial class VectorCreator
	{
		public class CrossFunction : FunctionBase
		{
			public static CrossFunction Instance { get; } = new CrossFunction();

			public override Value Call(Arguments args)
			{
				if (args.Length != 2)
					throw new InvalidOperationException("Expected two vectors");
				var lhs = args[0].Object as Vector;
				if (lhs == null)
					throw new InvalidOperationException("First argument is not a vector");
				var rhs = args[1].Object as Vector;
				if (rhs == null)
					throw new InvalidOperationException("Second argument is not a vector");
				return new Value(new Vector(Vector3d.Cross(lhs.Native, rhs.Native)));
			}
			/*
			public override DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			{
				if (args.Count != 3)
					throw new InvalidOperationException("Expected two vectors");
				var lhs = args[1].ToObject<Vector>();
				if (lhs == null)
					throw new InvalidOperationException("First argument is not a vector");
				var rhs = args[2].ToObject<Vector>();
				if (rhs == null)
					throw new InvalidOperationException("Second argument is not a vector");
				return DynValue.FromObject(null, new Vector(Vector3d.Cross(lhs.Native, rhs.Native)));
			}
			*/
		}
	}
}
