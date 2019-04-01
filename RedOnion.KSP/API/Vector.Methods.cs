using MoonSharp.Interpreter;
using RedOnion.Script;
using System;

namespace RedOnion.KSP.API
{
	public partial class Vector
	{
		public abstract class VectorMethodBase : MethodBase<Vector>
		{
			protected Vector GetVectorOrNumber(Arguments args, out double d)
			{
				if (args.Length != 1)
					throw new InvalidOperationException("Expected one argument");
				var result = args[0];
				if (result.IsNumber)
				{
					d = result.Double;
					return null;
				}
				d = double.NaN;
				var v = result.Object as Vector;
				if (v == null)
					throw new InvalidOperationException("Argument is neither number nor Vector");
				return v;
			}
		}
		public class ScaleMethod : VectorMethodBase
		{
			public static ScaleMethod Instance { get; } = new ScaleMethod();

			public override Value Call(Vector self, Arguments args)
			{
				var v = GetVectorOrNumber(args, out var f);
				self.Native = v != null
					? Vector3d.Scale(self.Native, v.Native)
					: new Vector3d(self.X*f, self.Y*f, self.Z*f);
				return Value.Void;
			}
		}
		public class ShrinkMethod : VectorMethodBase
		{
			public static ShrinkMethod Instance { get; } = new ShrinkMethod();

			public override Value Call(Vector self, Arguments args)
			{
				var v = GetVectorOrNumber(args, out var f);
				self.Native = v != null
					? new Vector3d(self.X/v.X, self.Y/v.Y, self.Z/v.Z)
					: new Vector3d(self.X/f, self.Y/f, self.Z/f);
				return Value.Void;
			}
		}
	}
}
