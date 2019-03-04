using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	/// <summary>
	/// Base class for both runtime engine and source generators
	/// </summary>
	[DebuggerDisplay("{Current} inside {Inside}")]
	public abstract class AbstractEngine
	{
		/// <summary>
		/// Exit code (of last statement, code block or whole program)
		/// </summary>
		public OpCode Exit { get; protected set; }

		/// <summary>
		/// String table associated with code
		/// </summary>
		public string[] Strings { get; protected set; }

		/// <summary>
		/// Active code (compilation unit)
		/// </summary>
		public byte[] Code { get; protected set; }

		/// <summary>
		/// Run script
		/// </summary>
		public AbstractEngine Execute(string[] strings, byte[] code)
			=> Execute(strings, code, 0, code.Length);

		/// <summary>
		/// Run script
		/// </summary>
		public AbstractEngine Execute(string[] strings, byte[] code, int at, int size)
		{
			ExecCode(strings, code, at, size);
			return this;
		}

		/// <summary>
		/// Run script
		/// </summary>
		protected virtual void ExecCode(string[] strings, byte[] code, int at, int size)
		{
			Exit = 0;

			var prevStrings = Strings;
			var prevCode = Code;
			Strings = strings;
			Code = code;

			Current = 0;
			Inside = 0;
			var end = at + size;
			while (at < end)
			{
				Process(ref at);
				if (Exit != 0)
					break;
			}

			Strings = prevStrings;
			Code = prevCode;
		}

		/// <summary>
		/// Evaluate expression
		/// </summary>
		public AbstractEngine Expression(string[] strings, byte[] code)
			=> Expression(strings, code, 0);

		/// <summary>
		/// Evaluate expression
		/// </summary>
		public AbstractEngine Expression(string[] strings, byte[] code, int at)
		{
			EvalExpression(strings, code, at);
			return this;
		}

		/// <summary>
		/// Evaluate expression
		/// </summary>
		protected virtual void EvalExpression(string[] strings, byte[] code, int at)
		{
			Exit = 0;

			var prevStrings = Strings;
			var prevCode = Code;
			Strings = strings;
			Code = code;

			Current = 0;
			Inside = 0;
			Expression(ref at);

			Strings = prevStrings;
			Code = prevCode;
		}

		/// <summary>
		/// Code of current operation
		/// </summary>
		protected OpCode Current;
		/// <summary>
		/// Hint for source generator, unused in real engine
		/// </summary>
		protected OpCode Inside;

		protected abstract void Literal(OpCode op, ref int at);

		protected abstract void Binary(OpCode op, ref int at);

		protected abstract void Unary(OpCode op, ref int at);

		protected abstract void Special(OpCode op, ref int at);

		protected abstract void Statement(OpCode op, ref int at);

		protected abstract void Other(OpCode op, ref int at);

		protected virtual void Block(ref int at)
		{
			Exit = 0;
			var size = CodeInt(ref at);
			var end = at + size;
			while (at < end)
			{
				Process(ref at);
				if (Exit != 0)
				{
					break;
				}
			}
			at = end;
		}

		protected virtual void Process(ref int at)
		{
			var op = Code[at];
			if (op < (byte)OpKind.Statement)
			{
				Expression(ref at);
				return;
			}
			at++;
			var prev = Inside;
			Inside = Current;
			Current = (OpCode)op;
			if (op < (byte)OpKind.Access)
				Statement(Current, ref at);
			else
				Other(Current, ref at);
			Current = Inside;
			Inside = prev;
		}

		protected virtual void Expression(ref int at)
		{
			var prev = Inside;
			Inside = Current;
			var op = ((OpCode)Code[at++]).Extend();
			Current = op;
			var kind = op.Kind();
			switch (kind)
			{
			case OpKind.Literal:
			case OpKind.Number:
				Literal(op, ref at);
				break;
			case OpKind.Special:
				Special(op, ref at);
				break;
			default:
				if (kind >= OpKind.Statement)
					throw new InvalidOperationException();
				if (op.Binary())
				{
					Binary(op, ref at);
					break;
				}
				if (op.Unary())
				{
					Unary(op, ref at);
					break;
				}
				Special(op, ref at);
				break;
			}
			Current = Inside;
			Inside = prev;
		}

		protected int CodeInt(ref int at)
		{
			var v = BitConverter.ToInt32(Code, at);
			at += 4;
			return v;
		}

		protected long CodeLong(ref int at)
		{
			var v = BitConverter.ToInt64(Code, at);
			at += 8;
			return v;
		}

		protected short CodeShort(ref int at)
		{
			var v = BitConverter.ToInt16(Code, at);
			at += 2;
			return v;
		}

		protected uint CodeUInt(ref int at)
		{
			var v = BitConverter.ToUInt32(Code, at);
			at += 4;
			return v;
		}

		protected ulong CodeULong(ref int at)
		{
			var v = BitConverter.ToUInt64(Code, at);
			at += 8;
			return v;
		}

		protected ushort Cushort(ref int at)
		{
			var v = BitConverter.ToUInt16(Code, at);
			at += 2;
			return v;
		}

		protected unsafe float CodeFloat(ref int at)
		{
			var v = BitConverter.ToUInt32(Code, at);
			at += 4;
			return *(float*)&v;
		}

		protected double CodeDouble(ref int at)
		{
			var v = BitConverter.ToDouble(Code, at);
			at += 8;
			return v;
		}
	}
}
