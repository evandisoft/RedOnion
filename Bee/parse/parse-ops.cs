using Bee.Run;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	public partial class Parser
	{
		/// <summary>
		/// reset operator stack
		/// </summary>
		public Parser Reset()
		{
			OpsAt = 0;
			return this;
		}
		
		private Opcode[] _ops = new Opcode[64];
		/// <summary>
		/// operator stack (to be processed according to priority/precedence)
		/// </summary>
		public Opcode[] Ops
		{
			get => _ops;
			private set => _ops = value;
		}
		
		private int _opsAt;
		/// <summary>
		/// write position (top) for operator stack
		/// </summary>
		public int OpsAt
		{
			get => _opsAt;
			private set => _opsAt = value;
		}
		
		/// <summary>
		/// push right-associative operator (assign or special)
		/// </summary>
		public Parser Op(Opcode op)
		{
			Debug.Assert((op == Opcode.Comma || op.Prior() <= Opcode.Assign.Prior()) || (op.Unary() && (!op.Postfix())));
			if (OpsAt == Ops.Length)
			{
				Array.Resize(ref _ops, Ops.Length << 1);
			}
			Ops[OpsAt++] = op;
			return this;
		}
		
		/// <summary>
		/// push left-associative operator but first pop and prepare operators with higher or same priority
		/// </summary>
		public Parser Op(Opcode op, int bottom)
		{
			Debug.Assert(op.Prior() > Opcode.Assign.Prior());
			while (OpsAt > bottom && Top().Prior() >= op.Prior())
			{
				Cgen.Prepare(Pop());
			}
			if (OpsAt == Ops.Length)
			{
				Array.Resize(ref _ops, Ops.Length << 1);
			}
			Ops[OpsAt++] = op;
			return this;
		}
		
		/// <summary>
		/// get/peek top operator (without taking/poping it)
		/// </summary>
		public Opcode Top()
		{
			Debug.Assert(OpsAt > 0);
			return Ops[OpsAt - 1];
		}
		
		/// <summary>
		/// get and remove (pop) top operator
		/// </summary>
		public Opcode Pop()
		{
			Debug.Assert(OpsAt > 0);
			return Ops[--OpsAt];
		}
	}
}
