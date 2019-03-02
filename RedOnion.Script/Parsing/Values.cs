using System;
using System.Diagnostics;
using System.Text;
using RedOnion.Script.Execution;

namespace RedOnion.Script.Parsing
{
	partial class Parser
	{
		private byte[] _values = new byte[256];
		/// <summary>
		/// Value buffer/stack (variables, expression trees in postfix notation)
		/// </summary>
		protected byte[] Values => _values;
		/// <summary>
		/// Write position (top) for value buffer/stack
		/// </summary>
		protected int ValuesAt { get; set; }

		/// <summary>
		/// Push simple value (this, null, false, ...)
		/// </summary>
		protected void Push(OpCode OpCode)
		{
			var start = ValuesAt;
			ValuesReserve(5);
			ValuesPush((byte)OpCode);
			ValuesPush(start);
		}

		private string[] _stringValues = new string[64];
		/// <summary>
		/// Strings associated with value buffer/stack
		/// </summary>
		protected string[] StringValues => _stringValues;
		/// <summary>
		/// Write position (top) for strings associated with value buffer/stack
		/// </summary>
		protected int StringValuesAt { get; set; }

		/// <summary>
		/// push string value (string, identifier)
		/// </summary>
		void Push(OpCode OpCode, string value)
		{
			var start = ValuesAt;
			ValuesReserve(9);
			ValuesPush(value);
			ValuesPush((byte)OpCode);
			ValuesPush(start);
		}

		/// <summary>
		/// peek (read but not pop) top integer from value buffer/stack
		/// </summary>
		protected int TopInt()
		{
			return TopInt(ValuesAt);
		}

		/// <summary>
		/// peek (read but not pop) top integer from value buffer/stack with end at @top
		/// @top index after the integer
		/// </summary>
		protected int TopInt(int top)
		{
			Debug.Assert(ValuesAt >= top && top >= 4);
			int value = Values[--top];
			value = (value << 8) | Values[--top];
			value = (value << 8) | Values[--top];
			return (value << 8) | Values[--top];
		}

		/// <summary>
		/// Ensure value buffer/stack capacity
		/// </summary>
		protected void ValuesReserve(int size)
		{
			var need = ValuesAt + size;
			var cap = Values.Length;
			if (need < cap)
				return;
			do cap <<= 1; while (need > cap);
			Array.Resize(ref _values, cap);
		}

		/// <summary>
		/// Pop top byte from value buffer/stack
		/// </summary>
		protected byte PopByte()
		{
			Debug.Assert(ValuesAt > 0);
			return Values[--ValuesAt];
		}

		/// <summary>
		/// Pop top integer from value buffer/stack
		/// </summary>
		protected int PopInt()
		{
			Debug.Assert(ValuesAt >= 4);
			var v = (int)Values[--ValuesAt];
			v = (v << 8) | Values[--ValuesAt];
			v = (v << 8) | Values[--ValuesAt];
			return (v << 8) | Values[--ValuesAt];
		}

		/// <summary>
		/// Push single byte to value buffer/stack
		/// </summary>
		protected void Push(byte value)
		{
			ValuesReserve(1);
			ValuesPush(value);
		}

		/// <summary>
		/// Push single integer to value buffer/stack
		/// </summary>
		protected void Push(int value)
		{
			ValuesReserve(4);
			ValuesPush(value);
		}

		/// <summary>
		/// Push string (sequence of characters) to value buffer/stack
		/// </summary>
		protected void Push(string value)
		{
			ValuesReserve(4);
			ValuesPush(value);
		}

		/// <summary>
		/// Push sequence of bytes
		/// </summary>
		protected void Push(byte[] bytes)
		{
			ValuesReserve(bytes.Length);
			ValuesPush(bytes);
		}

		/// <summary>
		/// Push single unsigned integer to value buffer/stack
		/// </summary>
		protected void Push(uint value)
		{
			ValuesReserve(4);
			ValuesPush(value);
		}

		/// <summary>
		/// Push single long integer to value buffer/stack
		/// </summary>
		protected void Push(long value)
		{
			ValuesReserve(8);
			ValuesPush(value);
		}

		/// <summary>
		/// Push single long unsigned integer to value buffer/stack
		/// </summary>
		protected void Push(ulong value)
		{
			ValuesReserve(8);
			ValuesPush(value);
		}

		/// <summary>
		/// Push single short integer to value buffer/stack
		/// </summary>
		protected void Push(short value)
		{
			ValuesReserve(2);
			ValuesPush(value);
		}

		/// <summary>
		/// Push single short unsigned integer to value buffer/stack
		/// </summary>
		protected void Push(ushort value)
		{
			ValuesReserve(2);
			ValuesPush(value);
		}

		/// <summary>
		/// Push single/float number to value buffer/stack
		/// </summary>
		protected void Push(float value)
		{
			ValuesReserve(4);
			ValuesPush(value);
		}

		/// <summary>
		/// Push double (fp) number to value buffer/stack
		/// </summary>
		protected void Push(double value)
		{
			ValuesReserve(8);
			ValuesPush(value);
		}

		internal void ValuesPush(byte value)
		{
			Values[ValuesAt++] = value;
		}

		internal void ValuesPush(int value)
		{
			Values[ValuesAt++] = unchecked((byte)value);
			Values[ValuesAt++] = unchecked((byte)(value >> 8));
			Values[ValuesAt++] = unchecked((byte)(value >> 16));
			Values[ValuesAt++] = unchecked((byte)(value >> 24));
		}

		internal void ValuesPush(string value)
		{
			ValuesPush(StringValuesAt);
			if (StringValuesAt == StringValues.Length)
				Array.Resize(ref _stringValues, StringValues.Length << 1);
			StringValues[StringValuesAt++] = value;
		}

		internal void ValuesPush(byte[] bytes)
		{
			Array.Copy(bytes, 0, Values, ValuesAt, bytes.Length);
			ValuesAt += bytes.Length;
		}

		internal void ValuesPush(uint value)
		{
			Values[ValuesAt++] = unchecked((byte)value);
			Values[ValuesAt++] = unchecked((byte)(value >> 8));
			Values[ValuesAt++] = unchecked((byte)(value >> 16));
			Values[ValuesAt++] = unchecked((byte)(value >> 24));
		}

		internal void ValuesPush(ulong value)
		{
			ValuesPush((uint)value);
			ValuesPush((uint)(value >> 32));
		}

		internal void ValuesPush(long value)
		{
			ValuesPush((ulong)value);
		}

		internal void ValuesPush(short value)
		{
			Values[ValuesAt++] = unchecked((byte)value);
			Values[ValuesAt++] = unchecked((byte)(value >> 8));
		}

		internal void ValuesPush(ushort value)
		{
			Values[ValuesAt++] = unchecked((byte)value);
			Values[ValuesAt++] = unchecked((byte)(value >> 8));
		}

		internal unsafe void ValuesPush(float value)
		{
			ValuesPush(*(uint*)&value);
		}

		internal unsafe void ValuesPush(double value)
		{
			ValuesPush(*(ulong*)&value);
		}
	}
}
