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
		/// push simple value (this, null, false, ...)
		/// </summary>
		void Parser.IGenerator.Push(Opcode opcode)
		{
			var start = ValsAt;
			Vneed(5);
			Vpush(unchecked((byte)opcode));
			Vpush(start);
		}
		
		/// <summary>
		/// push string value (string, identifier)
		/// </summary>
		void Parser.IGenerator.Push(Opcode opcode, string @string)
		{
			var start = ValsAt;
			Vneed(5 + Encoding.UTF8.GetByteCount(@string));
			Vpush(@string);
			Vpush(unchecked((byte)opcode));
			Vpush(start);
		}
		
		private byte[] _vals = new byte[256];
		/// <summary>
		/// value buffer/stack (variables, expression trees in postfix notation)
		/// </summary>
		protected byte[] Vals
		{
			get => _vals;
			private set => _vals = value;
		}
		
		private int _valsAt;
		/// <summary>
		/// write position (top) for value buffer/stack
		/// </summary>
		protected int ValsAt
		{
			get => _valsAt;
			set => _valsAt = value;
		}
		
		/// <summary>
		/// peek (read but not pop) top integer from value buffer/stack
		/// </summary>
		protected int TopInt()
		{
			return TopInt(ValsAt);
		}
		
		/// <summary>
		/// peek (read but not pop) top integer from value buffer/stack with end at @top
		/// @top index after the integer
		/// </summary>
		protected int TopInt(int top)
		{
			Debug.Assert(ValsAt >= top && top >= 4);
			int value = Vals[--top];
			value = (value << 8) | Vals[--top];
			value = (value << 8) | Vals[--top];
			return (value << 8) | Vals[--top];
		}
		
		/// <summary>
		/// ensure value buffer/stack capacity
		/// </summary>
		protected void Vneed(int size)
		{
			var need = ValsAt + size;
			var cap = Vals.Length;
			if (need < cap)
			{
				return;
			}
			do
			{
				cap <<= 1;
			}
			while (need > cap);
			Array.Resize(ref _vals, cap);
		}
		
		/// <summary>
		/// pop top byte from value buffer/stack
		/// </summary>
		protected byte PopByte()
		{
			Debug.Assert(ValsAt > 0);
			return Vals[--ValsAt];
		}
		
		/// <summary>
		/// pop top integer from value buffer/stack
		/// </summary>
		protected int PopInt()
		{
			Debug.Assert(ValsAt >= 4);
			var v = (int)Vals[--ValsAt];
			v = (v << 8) | Vals[--ValsAt];
			v = (v << 8) | Vals[--ValsAt];
			return (v << 8) | Vals[--ValsAt];
		}
		
		/// <summary>
		/// push single byte to value buffer/stack
		/// </summary>
		protected void Push(byte @byte)
		{
			Vneed(1);
			Vpush(@byte);
		}
		
		/// <summary>
		/// push single integer to value buffer/stack
		/// </summary>
		protected void Push(int @int)
		{
			Vneed(4);
			Vpush(@int);
		}
		
		/// <summary>
		/// push string (sequence of characters) in UTF8 encoding to value buffer/stack
		/// </summary>
		protected void Push(string @string)
		{
			Push(Encoding.UTF8.GetBytes(@string));
		}
		
		/// <summary>
		/// push sequence of bytes
		/// </summary>
		protected void Push(byte[] bytes)
		{
			Vneed(bytes.Length);
			Vpush(bytes);
		}
		
		/// <summary>
		/// push single unsigned integer to value buffer/stack
		/// </summary>
		protected void Push(uint @uint)
		{
			Vneed(4);
			Vpush(@uint);
		}
		
		/// <summary>
		/// push single long integer to value buffer/stack
		/// </summary>
		protected void Push(long @long)
		{
			Vneed(8);
			Vpush(@long);
		}
		
		/// <summary>
		/// push single long unsigned integer to value buffer/stack
		/// </summary>
		protected void Push(ulong @ulong)
		{
			Vneed(8);
			Vpush(@ulong);
		}
		
		/// <summary>
		/// push single short integer to value buffer/stack
		/// </summary>
		protected void Push(short @short)
		{
			Vneed(2);
			Vpush(@short);
		}
		
		/// <summary>
		/// push single short unsigned integer to value buffer/stack
		/// </summary>
		protected void Push(ushort @ushort)
		{
			Vneed(2);
			Vpush(@ushort);
		}
		
		/// <summary>
		/// push single/float number to value buffer/stack
		/// </summary>
		protected void Push(float @float)
		{
			Vneed(4);
			Vpush(@float);
		}
		
		/// <summary>
		/// push double (fp) number to value buffer/stack
		/// </summary>
		protected void Push(double @double)
		{
			Vneed(8);
			Vpush(@double);
		}
		
		internal void Vpush(byte @byte)
		{
			Vals[ValsAt++] = @byte;
		}
		
		internal void Vpush(int @int)
		{
			Vals[ValsAt++] = unchecked((byte)@int);
			Vals[ValsAt++] = unchecked((byte)(@int >> 8));
			Vals[ValsAt++] = unchecked((byte)(@int >> 16));
			Vals[ValsAt++] = unchecked((byte)(@int >> 24));
		}
		
		internal void Vpush(string @string)
		{
			ValsAt += Encoding.UTF8.GetBytes(@string, 0, @string.Length, Vals, ValsAt);
		}
		
		internal void Vpush(byte[] bytes)
		{
			Array.Copy(bytes, 0, Vals, ValsAt, bytes.Length);
			ValsAt += bytes.Length;
		}
		
		internal void Vpush(uint @uint)
		{
			Vals[ValsAt++] = unchecked((byte)@uint);
			Vals[ValsAt++] = unchecked((byte)(@uint >> 8));
			Vals[ValsAt++] = unchecked((byte)(@uint >> 16));
			Vals[ValsAt++] = unchecked((byte)(@uint >> 24));
		}
		
		internal void Vpush(ulong @ulong)
		{
			Vpush((uint)@ulong);
			Vpush((uint)(@ulong >> 32));
		}
		
		internal void Vpush(long @long)
		{
			Vpush((ulong)@long);
		}
		
		internal void Vpush(short @short)
		{
			Vals[ValsAt++] = unchecked((byte)@short);
			Vals[ValsAt++] = unchecked((byte)(@short >> 8));
		}
		
		internal void Vpush(ushort @ushort)
		{
			Vals[ValsAt++] = unchecked((byte)@ushort);
			Vals[ValsAt++] = unchecked((byte)(@ushort >> 8));
		}
		
		internal void Vpush(float @float)
		{
			Vpush(Bits.Get(@float));
		}
		
		internal void Vpush(double @double)
		{
			Vpush(Bits.Get(@double));
		}
		
		private string _debuggerDisplay_vals
		{
			get
			{
				if (ValsAt == 0)
				{
					return System.String.Empty;
				}
				var sb = new StringBuilder();
				var i = ValsAt;
				for (var j = 0; i > 0 && j < 33; j++)
				{
					sb.AppendFormat("{0:X2}", Vals[--i]);
				}
				if (i > 0)
				{
					sb.Append("...");
				}
				return sb.ToString();
			}
		}
	}
}
