using MoonSharp.Interpreter;
using RedOnion.Script;
using System;

namespace RedOnion.KSP.API
{
	public partial class VectorCreator
	{
		public abstract class VectorFunctionBase : FunctionBase
		{
			protected Vector3d GetOne(Arguments args)
			{
				if (args.Length != 1)
					throw new InvalidOperationException("Expected a vector");
				if (ToVector3d(args[0], out var it))
					return it;
				throw new InvalidOperationException("Argument is not a vector");
			}
			protected void GetTwo(Arguments args, out Vector3d lhs, out Vector3d rhs)
			{
				if (args.Length != 2)
					throw new InvalidOperationException("Expected two vectors");
				if (!ToVector3d(args[0], out lhs))
					throw new InvalidOperationException("First argument is not a vector");
				if (!ToVector3d(args[1], out rhs))
					throw new InvalidOperationException("Second argument is not a vector");
			}
		}
		public class CrossFunction : VectorFunctionBase
		{
			public static CrossFunction Instance { get; } = new CrossFunction();

			public override Value Call(Arguments args)
			{
				GetTwo(args, out var lhs, out var rhs);
				return new Value(new Vector(Vector3d.Cross(lhs, rhs)));
			}
		}
		public class DotFunction : VectorFunctionBase
		{
			public static DotFunction Instance { get; } = new DotFunction();

			public override Value Call(Arguments args)
			{
				GetTwo(args, out var lhs, out var rhs);
				return new Value(new Vector(Vector3d.Dot(lhs, rhs)));
			}
		}
		public class AbsFunction : VectorFunctionBase
		{
			public static AbsFunction Instance { get; } = new AbsFunction();

			public override Value Call(Arguments args)
			{
				var v = GetOne(args);
				return new Value(new Vector(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z)));
			}
		}
		public class AngleFunction : VectorFunctionBase
		{
			public static AngleFunction Instance { get; } = new AngleFunction();

			public override Value Call(Arguments args)
			{
				GetTwo(args, out var lhs, out var rhs);
				return new Value(new Vector(Vector3d.Angle(lhs, rhs)));
			}
		}
	}
}
