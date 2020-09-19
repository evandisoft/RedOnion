using System;
using System.Diagnostics;
using RedOnion.Collections;

namespace RedOnion.ROS.Parsing
{
	partial class Parser
	{
		protected ListCore<ExCode> operators = new ListCore<ExCode>(64);

		/// <summary>
		/// Push right-associative operator (assign or special)
		/// </summary>
		protected void PushOperator(ExCode op)
		{
			Debug.Assert(op == ExCode.Comma || op.Unary() && !op.Postfix()
				|| op.Priority() <= ExCode.Assign.Priority());
			operators.Push(op);
		}

		/// <summary>
		/// Push left-associative operator but first pop and prepare operators with higher or same priority
		/// </summary>
		protected void PushOperator(ExCode op, int bottom)
		{
			Debug.Assert(op.Priority() > ExCode.Assign.Priority());
			while (operators.size > bottom && TopOperator().Priority() >= op.Priority())
				PrepareOperator(PopOperator(), bottom);
			operators.Push(op);
		}

		/// <summary>
		/// Get/peek top operator (without taking/poping it)
		/// </summary>
		protected ExCode TopOperator()
		{
			return operators.size > 0 ? operators[operators.size - 1] : ExCode.Void;
		}

		/// <summary>
		/// Get and remove (pop) top operator
		/// </summary>
		protected ExCode PopOperator()
		{
			Debug.Assert(operators.size > 0);
			return operators.Pop();
		}

		/// <summary>
		/// Prepare top operator (prepare postfix record or expression tree node)
		/// </summary>
		protected void PrepareOperator(ExCode op, int bottom)
		{
			Debug.Assert(op.Kind() <= OpKind.PreOrPost && op.Kind() >= OpKind.Special);

			if (op.Binary())
				goto binary;
			if (op.Ternary())
				goto ternary;
			if (op.Multi())
				goto multi;

			Debug.Assert(op.Unary());
			//unary:	[arg][start]
			//  --->	[arg][op][start]	(start position is not needed for argument of unary operation)
			var start = PopInt();
			ValuesReserve(5);
			ValuesPush(unchecked((byte)op));
			ValuesPush(start);
			return;

		binary:
			//binary:	[larg][lstart][rarg][rstart]
			//	--->	[larg][lstart][rarg][rstart][op][lstart]
			var rstart = TopInt();
			var lstart = TopInt(rstart);
			ValuesReserve(5);
			ValuesPush(op.Code());
			ValuesPush(lstart);
			return;
		ternary:
			//ternary:	[cond][cstart][true][tstart][false][fstart]
			//	--->	[cond][cstart][true][tstart][false][fstart][op][cstart]
			var fstart = TopInt();
			var tstart = TopInt(fstart);
			var cstart = TopInt(tstart);
			ValuesReserve(5);
			ValuesPush(op.Code());
			ValuesPush(cstart);
			return;
		multi:
			//multi:	[fn][start][arg0][start0]...[argN][startN]
			//	+ops:							[comma]
			//	--->	[fn][start][arg0][start0]...[argN][startN][N+2][op][start]
			Debug.Assert(op == ExCode.CallN || op == ExCode.IndexN || op == ExCode.Array);
			var mstart = TopInt();
			var n = 1;
			if (op == ExCode.CallN || op == ExCode.IndexN)
			{
				n++;
				mstart = TopInt(mstart);
			}
			while (operators.size > bottom && TopOperator() == ExCode.Comma)
			{
				PopOperator();
				n++;
				mstart = TopInt(mstart);
			}
			if (n > 127)
				throw new ParseError(this, "Too many arguments");
			if (n == 3 && op == ExCode.CallN)
			{
				ValuesReserve(5);
				ValuesPush(ExCode.Call2.Code());
				ValuesPush(mstart);
				return;
			}
			ValuesReserve(6);
			ValuesPush((byte)n);
			ValuesPush(op.Code());
			ValuesPush(mstart);
			return;
		}
	}
}
