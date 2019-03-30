using MoonSharp.Interpreter;
using RedOnion.Script;
using System;

namespace RedOnion.KSP.API
{
	public partial class Vector
	{
		public class ScaleMethod : MethodBase<Vector>
		{
			public static ScaleMethod Instance { get; } = new ScaleMethod();

			public override Value Call(Vector self, Arguments args)
			{
				if (args.Length != 1)
					throw new InvalidOperationException("Expected one argument");
				var rhs = args[0];
				if (rhs.IsNumber)
				{
					var f = rhs.Double;
					self.Native = new Vector3d(self.X*f, self.Y*f, self.Z*f);
					return Value.Void;
				}
				var v = rhs.Object as Vector;
				if (v == null)
					throw new InvalidOperationException("Argument is neither number nor Vector");
				self.Native = Vector3d.Scale(self.Native, v.Native);
				return Value.Void;
			}
			public override DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			{
				if (args.Count != 2)
					throw new InvalidOperationException("Expected one argument");
				var self = args[0].ToObject<Vector>();
				var rhs = args[1];
				if (rhs.Type == DataType.Number)
				{
					var f = rhs.Number;
					self.Native = new Vector3d(self.X*f, self.Y*f, self.Z*f);
					return DynValue.Void;
				}
				var v = rhs.ToObject<Vector>();
				if (v == null)
					throw new InvalidOperationException("Argument is neither number nor Vector");
				self.Native = Vector3d.Scale(self.Native, v.Native);
				return DynValue.Void;
			}
		}
	}
}
