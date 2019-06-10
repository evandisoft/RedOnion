using MoonSharp.Interpreter;
using RedOnion.ROS;
using System;

namespace RedOnion.KSP.API
{
	public partial class Vector
	{
		public abstract class VectorMethodBase : FunctionBase
		{
			/// <summary>
			/// Get Vector3d or Double from single argument. Returns true if vector.
			/// </summary>
			protected bool GetVectorOrNumber(Arguments args, out Vector3d result, out double d)
			{
				if (args.Length != 1)
					throw new InvalidOperationException("Expected one argument");
				var it = args[0];
				if (it.IsNumerOrChar)
				{
					d = it.ToDouble();
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

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				var me = (Vector)self;
				me.Native = GetVectorOrNumber(args, out var v, out var f)
					? Vector3d.Scale(me.Native, v)
					: new Vector3d(me.X*f, me.Y*f, me.Z*f);
				return true;
			}
		}
		public class ShrinkMethod : VectorMethodBase
		{
			public static ShrinkMethod Instance { get; } = new ShrinkMethod();

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				var me = (Vector)self;
				me.Native = GetVectorOrNumber(args, out var v, out var f)
					? new Vector3d(me.X/v.x, me.Y/v.y, me.Z/v.z)
					: new Vector3d(me.X/f, me.Y/f, me.Z/f);
				return true;
			}
		}
	}
}
