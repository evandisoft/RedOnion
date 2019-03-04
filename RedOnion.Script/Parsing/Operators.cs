using System;
using System.Diagnostics;

namespace RedOnion.Script.Parsing
{
	partial class Parser
	{
		private OpCode[] _operators = new OpCode[64];
		/// <summary>
		/// Operator stack (to be processed according to priority/precedence)
		/// </summary>
		protected OpCode[] Operators => _operators;
		/// <summary>
		/// Write position (top) for operator stack
		/// </summary>
		protected int OperatorAt { get; private set; }

		/// <summary>
		/// Push right-associative operator (assign or special)
		/// </summary>
		protected void PushOperator(OpCode op)
		{
			Debug.Assert(op == OpCode.Comma || op.Unary() && !op.Postfix()
				|| op.Priority() <= OpCode.Assign.Priority());
			if (OperatorAt == Operators.Length)
				Array.Resize(ref _operators, Operators.Length << 1);
			Operators[OperatorAt++] = op;
		}

		/// <summary>
		/// Push left-associative operator but first pop and prepare operators with higher or same priority
		/// </summary>
		protected Parser PushOperator(OpCode op, int bottom)
		{
			Debug.Assert(op.Priority() > OpCode.Assign.Priority());
			while (OperatorAt > bottom && TopOperator().Priority() >= op.Priority())
				PrepareOperator(PopOperator());
			if (OperatorAt == Operators.Length)
				Array.Resize(ref _operators, Operators.Length << 1);
			Operators[OperatorAt++] = op;
			return this;
		}

		/// <summary>
		/// Get/peek top operator (without taking/poping it)
		/// </summary>
		protected OpCode TopOperator()
		{
			Debug.Assert(OperatorAt > 0);
			return Operators[OperatorAt - 1];
		}

		/// <summary>
		/// Get and remove (pop) top operator
		/// </summary>
		protected OpCode PopOperator()
		{
			Debug.Assert(OperatorAt > 0);
			return Operators[--OperatorAt];
		}

		/// <summary>
		/// Prepare top operator (prepare postfix record or expression tree node)
		/// </summary>
		protected void PrepareOperator(OpCode op)
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
			Debug.Assert(op == OpCode.CallN || op == OpCode.IndexN
				|| op == OpCode.Array || op == OpCode.Generic);
			var mstart = TopInt();
			var n = 1;
			if (op == OpCode.CallN || op == OpCode.IndexN)
			{
				n++;
				mstart = TopInt(mstart);
			}
			while (OperatorAt > 0 && TopOperator() == OpCode.Comma)
			{
				PopOperator();
				n++;
				mstart = TopInt(mstart);
			}
			if (n > 127)
				throw new ParseError(lexer, "Too many arguments");
			if (n == 3 && op == OpCode.CallN)
			{
				ValuesReserve(5);
				ValuesPush(OpCode.Call2.Code());
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
