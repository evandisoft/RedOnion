using MoonSharp.Interpreter;
using RedOnion.Script;
using System;

namespace RedOnion.KSP.API
{
	public partial class Vector
	{
		public abstract class VectorMethodBase : MethodBase<Vector>
		{
			/// <summary>
			/// Get Vector3d or Double from single argument. Returns true if vector.
			/// </summary>
			protected bool GetVectorOrNumber(Arguments args, out Vector3d result, out double d)
			{
				if (args.Length != 1)
					throw new InvalidOperationException("Expected one argument");
				var it = args[0];
				if (it.IsNumber)
				{
					d = it.Double;
					result = new Vector3d(d, d, d);
					return false;
				}
				d = double.NaN;
				if (!VectorCreator.ToVector3d(it, out result))
					throw new InvalidOperationException("Argument is neither number nor Vector");
				return true;
			}
		}
		public class ScaleMethod : VectorMethodBase
		{
			public static ScaleMethod Instance { get; } = new ScaleMethod();

			public override Value Call(Vector self, Arguments args)
			{
				self.Native = GetVectorOrNumber(args, out var v, out var f)
					? Vector3d.Scale(self.Native, v)
					: new Vector3d(self.X*f, self.Y*f, self.Z*f);
				return Value.Void;
			}
		}
		public class ShrinkMethod : VectorMethodBase
		{
			public static ShrinkMethod Instance { get; } = new ShrinkMethod();

			public override Value Call(Vector self, Arguments args)
			{
				self.Native = GetVectorOrNumber(args, out var v, out var f)
					? new Vector3d(self.X/v.x, self.Y/v.y, self.Z/v.z)
					: new Vector3d(self.X/f, self.Y/f, self.Z/f);
				return Value.Void;
			}
		}
	}
}
