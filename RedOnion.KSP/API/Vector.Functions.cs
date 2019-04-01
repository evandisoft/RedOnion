using MoonSharp.Interpreter;
using RedOnion.Script;
using System;

namespace RedOnion.KSP.API
{
	public partial class VectorCreator
	{
		public abstract class VectorFunctionBase : FunctionBase
		{
			protected Vector GetOne(Arguments args)
			{
				if (args.Length != 1)
					throw new InvalidOperationException("Expected a vector");
				var result = args[0].Object as Vector;
				if (result == null)
					throw new InvalidOperationException("Argument is not a vector");
				return result;
			}
			protected void GetTwo(Arguments args, out Vector lhs, out Vector rhs)
			{
				if (args.Length != 2)
					throw new InvalidOperationException("Expected two vectors");
				lhs = args[0].Object as Vector;
				if (lhs == null)
					throw new InvalidOperationException("First argument is not a vector");
				rhs = args[1].Object as Vector;
				if (rhs == null)
					throw new InvalidOperationException("Second argument is not a vector");
			}
		}
		public class CrossFunction : VectorFunctionBase
		{
			public static CrossFunction Instance { get; } = new CrossFunction();

			public override Value Call(Arguments args)
			{
				GetTwo(args, out var lhs, out var rhs);
				return new Value(new Vector(Vector3d.Cross(lhs.Native, rhs.Native)));
			}
		}
		public class DotFunction : VectorFunctionBase
		{
			public static DotFunction Instance { get; } = new DotFunction();

			public override Value Call(Arguments args)
			{
				GetTwo(args, out var lhs, out var rhs);
				return new Value(new Vector(Vector3d.Dot(lhs.Native, rhs.Native)));
			}
		}
		public class AbsFunction : VectorFunctionBase
		{
			public static AbsFunction Instance { get; } = new AbsFunction();

			public override Value Call(Arguments args)
			{
				var v = GetOne(args);
				return new Value(new Vector(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z)));
			}
		}
	}
}
