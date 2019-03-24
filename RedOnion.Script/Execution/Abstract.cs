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
		/// Engine options
		/// </summary>
		public EngineOption Options { get; set; }
			= DefaultOptions;
		public static EngineOption DefaultOptions
			= EngineOption.Strict
			| EngineOption.BlockScope
			| EngineOption.SelfScope
			| EngineOption.Autocall;
		public bool HasOption(EngineOption option)
			=> (Options & option) != 0;

		/// <summary>
		/// Exit code (of last statement, code block or whole program)
		/// </summary>
		public OpCode Exit { get; protected set; }
		/// <summary>
		/// Result of last expression (lvalue)
		/// </summary>
		protected Value Value;
		/// <summary>
		/// Result of last expression (rvalue)
		/// </summary>
		public Value Result => Value.RValue;

		/// <summary>
		/// Currently executed code
		/// </summary>
		public CompiledCode Compiled
		{
			get => _compiled;
			set
			{
				_compiled = value;
				Strings = value?.Strings;
				Code = value?.Code;
			}
		}
		private CompiledCode _compiled;

		/// <summary>
		/// String table associated with code
		/// </summary>
		public string[] Strings { get; protected set; }
		/// <summary>
		/// Active code (compilation unit)
		/// </summary>
		public byte[] Code { get; protected set; }

		public virtual void Log(string msg) { }
		public void Log(string msg, params object[] args)
			=> Log(string.Format(Value.Culture, msg, args));
		[Conditional("DEBUG")]
		public void DebugLog(string msg) => Log(msg);
		[Conditional("DEBUG")]
		public void DebugLog(string msg, params object[] args)
			=> Log(string.Format(Value.Culture, msg, args));

		public event Action<string> Printing;
		public void Print(string msg)
		{
			var it = Printing;
			if (it != null) it(msg);
			else Log(msg);
		}
		public void Print(string msg, params object[] args)
			=> Print(string.Format(Value.Culture, msg, args));

		/// <summary>
		/// Run script
		/// </summary>
		public virtual void Execute(CompiledCode code, int at, int size)
		{
			Exit = 0;

			var prev = Compiled;
			Compiled = code;
			try
			{

				Current = 0;
				Inside = 0;
				var end = at + size;
				if (HasOption(EngineOption.Repl) && at < end
					&& ((OpCode)Code[at]).Extend() == OpCode.Autocall)
				{
					at++;
					Process(ref at);
					Autocall(at == end);
				}
				while (at < end)
				{
					Process(ref at);
					if (Exit != 0)
						break;
				}
			}
			catch (RuntimeError)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new RuntimeError(code, at, ex);
			}
			finally
			{
				Compiled = prev;
			}
		}
		/// <summary>
		/// Run compiled script
		/// </summary>
		public void Execute(CompiledCode code)
			=> Execute(code, 0, code.Code.Length);

		/// <summary>
		/// Evaluate single expression
		/// </summary>
		public virtual Value Evaluate(CompiledCode code, int at)
		{
			Exit = 0;

			var prev = Compiled;
			Compiled = code;
			try
			{

				Current = 0;
				Inside = 0;
				Expression(ref at);
			}
			finally
			{
				Compiled = prev;
			}
			return Result;
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

		protected abstract void Autocall(bool weak = false);

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
				if (kind < OpKind.Statement)
				{
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

		protected ushort CodeUShort(ref int at)
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
