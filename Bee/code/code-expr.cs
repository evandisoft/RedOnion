using Bee.Run;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	public partial class PseudoGenerator: Parser.IGenerator
	{
		/// <summary>
		/// prepare top operator (prepare postfix record or expression tree node)
		/// </summary>
		void Parser.IGenerator.Prepare(Opcode op)
		{
			Debug.Assert(op.Kind() <= Opkind.Prepost && op.Kind() >= Opkind.Special);
			if (op.Binary())
			{
				goto binary;
			}
			if (op.Ternary())
			{
				goto ternary;
			}
			if (op.Multi())
			{
				goto multi;
			}
			Debug.Assert(op.Unary());
			var start = PopInt();
			Vneed(1);
			Vpush(unchecked((byte)op));
			Vpush(start);
			return;
		binary:
			var rstart = TopInt();
			var lstart = TopInt(rstart);
			Vneed(5);
			Vpush(op.Code());
			Vpush(lstart);
			return;
		ternary:
			var fstart = TopInt();
			var tstart = TopInt(fstart);
			var cstart = TopInt(tstart);
			Vneed(5);
			Vpush(op.Code());
			Vpush(cstart);
			return;
		multi:
			Debug.Assert(((op == Opcode.Mcall || op == Opcode.Mindex) || op == Opcode.Array) || op == Opcode.Generic);
			var mstart = TopInt();
			var n = 1;
			if (op == Opcode.Mcall || op == Opcode.Mindex)
			{
				n++;
				mstart = TopInt(mstart);
			}
			while (Parser.OpsAt > 0 && Parser.Top() == Opcode.Comma)
			{
				Parser.Pop();
				n++;
				mstart = TopInt(mstart);
			}
			if (n > 127)
			{
				throw new ParseError(Parser, "Too many arguments");
			}
			if (n == 3 && op == Opcode.Mcall)
			{
				Vneed(5);
				Vpush(unchecked((byte)Opcode.Call2));
				Vpush(mstart);
				return;
			}
			Vneed(6);
			Vpush(unchecked((byte)n));
			Vpush(op.Code());
			Vpush(mstart);
			return;
		}
	}
}
