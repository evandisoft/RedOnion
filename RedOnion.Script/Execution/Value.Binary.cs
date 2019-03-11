using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	public partial struct Value
	{
		public Value Binary(OpCode op, Value right)
		{
			switch(op)
			{
			default:
				return new Value();
			case OpCode.BitOr:
				return this | right;
			case OpCode.BitXor:
				return this ^ right;
			case OpCode.BitAnd:
				return this & right;
			case OpCode.ShiftLeft:
				return ShiftLeft(right);
			case OpCode.ShiftRight:
				return ShiftRight(right);
			case OpCode.Add:
				return this + right;
			case OpCode.Sub:
				return this - right;
			case OpCode.Mul:
				return this * right;
			case OpCode.Div:
				return this / right;
			case OpCode.Mod:
				return this % right;
			case OpCode.Equals:
				return new Value(this == right);
			case OpCode.Differ:
				return new Value(this != right);
			case OpCode.Less:
				return new Value(this < right);
			case OpCode.More:
				return new Value(this > right);
			case OpCode.LessEq:
				return new Value(this <= right);
			case OpCode.MoreEq:
				return new Value(this >= right);
			}
		}

		public static Value operator +(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.Type == ValueKind.String)
				return new Value(lhs.str + rhs.String);
			if (rhs.Type == ValueKind.String)
				return new Value(lhs.String + rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Type == ValueKind.Float && !rhs.Is64)
						return new Value(lhs.data.Float + rhs.Float);
					return new Value(lhs.data.Double + rhs.Double);
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Type == ValueKind.Float && !lhs.Is64)
						return new Value(lhs.Float + rhs.data.Float);
					return new Value(lhs.Double + rhs.data.Double);
				}
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Type : rhs.Type, lhs.Long + rhs.Long);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int + rhs.Int);
					return new Value(lhs.Long + rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt + rhs.UInt);
				return new Value(lhs.ULong + rhs.ULong);
			}
			return new Value();
		}

		public static Value operator -(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Type == ValueKind.Float && !rhs.Is64)
						return new Value(lhs.data.Float - rhs.Float);
					return new Value(lhs.data.Double - rhs.Double);
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Type == ValueKind.Float && !lhs.Is64)
						return new Value(lhs.Float - rhs.data.Float);
					return new Value(lhs.Double - rhs.data.Double);
				}
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Type : rhs.Type, lhs.Long - rhs.Long);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int - rhs.Int);
					return new Value(lhs.Long - rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt - rhs.UInt);
				return new Value(lhs.ULong - rhs.ULong);
			}
			return new Value();
		}

		public static Value operator *(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Type == ValueKind.Float && !rhs.Is64)
						return new Value(lhs.data.Float * rhs.Float);
					return new Value(lhs.data.Double * rhs.Double);
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Type == ValueKind.Float && !lhs.Is64)
						return new Value(lhs.Float * rhs.data.Float);
					return new Value(lhs.Double * rhs.data.Double);
				}
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Type : rhs.Type, lhs.Long * rhs.Long);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int * rhs.Int);
					return new Value(lhs.Long * rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt * rhs.UInt);
				return new Value(lhs.ULong * rhs.ULong);
			}
			return new Value();
		}

		public static Value operator /(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Type == ValueKind.Float && !rhs.Is64)
						return new Value(lhs.data.Float / rhs.Float);
					return new Value(lhs.data.Double / rhs.Double);
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Type == ValueKind.Float && !lhs.Is64)
						return new Value(lhs.Float / rhs.data.Float);
					return new Value(lhs.Double / rhs.data.Double);
				}
				if (lhs.Is64 || rhs.Is64)
				{
					var v = rhs.Long;
					if (v == 0)
						return new Value();
					return new Value(lhs.Is64 ? lhs.Type : rhs.Type, lhs.Long / v);
				}
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
					{
						var i = rhs.Int;
						if (i == 0)
							return new Value();
						return new Value(lhs.Int / i);
					}
					var v = rhs.Long;
					if (v == 0)
						return new Value();
					return new Value(lhs.Long / v);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
				{
					var i = rhs.UInt;
					if (i == 0)
						return new Value();
					return new Value(lhs.UInt / i);
				}
				var u = rhs.ULong;
				if (u == 0)
					return new Value();
				return new Value(lhs.ULong / u);
			}
			return new Value();
		}

		public static Value operator %(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Type == ValueKind.Float && !rhs.Is64)
						return new Value(lhs.data.Float % rhs.Float);
					return new Value(lhs.data.Double % rhs.Double);
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Type == ValueKind.Float && !lhs.Is64)
						return new Value(lhs.Float % rhs.data.Float);
					return new Value(lhs.Double % rhs.data.Double);
				}
				if (lhs.Is64 || rhs.Is64)
				{
					var v = rhs.Long;
					if (v == 0)
						return new Value();
					return new Value(lhs.Is64 ? lhs.Type : rhs.Type, lhs.Long % v);
				}
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
					{
						var i = rhs.Int;
						if (i == 0)
							return new Value();
						return new Value(lhs.Int % i);
					}
					var v = rhs.Long;
					if (v == 0)
						return new Value();
					return new Value(lhs.Long % v);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
				{
					var v = rhs.UInt;
					if (v == 0)
						return new Value();
					return new Value(lhs.UInt % v);
				}
				var u = rhs.ULong;
				if (u == 0)
					return new Value();
				return new Value(lhs.ULong % u);
			}
			return new Value();
		}

		public static Value operator &(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.Type == ValueKind.String)
				return new Value(lhs.str + rhs.String);
			if (rhs.Type == ValueKind.String)
				return new Value(lhs.String + rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return new Value(lhs.Long & rhs.Long);
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Type : rhs.Type, lhs.Long & rhs.Long);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int & rhs.Int);
					return new Value(lhs.Long & rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt & rhs.UInt);
				return new Value(lhs.ULong & rhs.ULong);
			}
			return new Value();
		}

		public static Value operator |(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.Type == ValueKind.String)
				return new Value(lhs.str + rhs.String);
			if (rhs.Type == ValueKind.String)
				return new Value(lhs.String + rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return new Value(lhs.Long | rhs.Long);
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Type : rhs.Type, lhs.Long | rhs.Long);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int | rhs.Int);
					return new Value(lhs.Long | rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt | rhs.UInt);
				return new Value(lhs.ULong | rhs.ULong);
			}
			return new Value();
		}

		public static Value operator ^(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Type == ValueKind.Float && !rhs.Is64)
						return new Value(Math.Pow(lhs.data.Float, rhs.Float));
					return new Value(Math.Pow(lhs.data.Double, rhs.Double));
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Type == ValueKind.Float && !lhs.Is64)
						return new Value(Math.Pow(lhs.Float, rhs.data.Float));
					return new Value(Math.Pow(lhs.Double, rhs.data.Double));
				}
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return new Value(lhs.Long ^ rhs.Long);
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Type : rhs.Type, lhs.Long ^ rhs.Long);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int ^ rhs.Int);
					return new Value(lhs.Long ^ rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt ^ rhs.UInt);
				return new Value(lhs.ULong ^ rhs.ULong);
			}
			return new Value();
		}

		public Value ShiftLeft(Value by)
			=> ShiftLeft(this, by);
		public static Value ShiftLeft(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.Type == ValueKind.String)
				return new Value(lhs.str + rhs.String);
			if (rhs.Type == ValueKind.String)
				return new Value(lhs.String + rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return new Value(lhs.Long << rhs.Int);
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Type : rhs.Type, lhs.Long << rhs.Int);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int << rhs.Int);
					return new Value(lhs.Long << rhs.Int);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt << rhs.Int);
				return new Value(lhs.ULong << rhs.Int);
			}
			return new Value();
		}

		public Value ShiftRight(Value by)
			=> ShiftLeft(this, by);
		public static Value ShiftRight(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return new Value(lhs.Long >> rhs.Int);
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Type : rhs.Type, lhs.Long >> rhs.Int);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int >> rhs.Int);
					return new Value(lhs.Long >> rhs.Int);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt >> rhs.Int);
				return new Value(lhs.ULong >> rhs.Int);
			}
			return new Value();
		}

		public static bool operator ==(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return lhs.Double == rhs.Double;
				if (lhs.Is64 || rhs.Is64)
					return lhs.Long == rhs.Long;
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return lhs.Int == rhs.Int;
					return new Value(lhs.Long == rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return lhs.UInt == rhs.UInt;
				return lhs.ULong == rhs.ULong;
			}
			switch (lhs.Type)
			{
			case ValueKind.Undefined:
				// undefined == null (equality) but undefined !== null (not identical)
				return rhs.Type == ValueKind.Undefined
					|| (rhs.Type == ValueKind.Object && rhs.ptr == null);
			case ValueKind.Object:
				// ptr is null for ValueType.Undefined
				return (rhs.Type == ValueKind.Object || rhs.Type == ValueKind.Undefined)
					&& lhs.ptr == rhs.ptr;
			}
			switch (rhs.Type)
			{
			case ValueKind.Undefined:
			case ValueKind.Object:
				return false;
			}

			// for now, may get changed for side-effects
			return lhs.String == rhs.String;
		}

		public static bool operator !=(Value lhs, Value rhs)
			=> !(lhs == rhs);

		public override bool Equals(object rhs)
		{
			if (rhs is Value)
				return this == (Value)rhs;
			switch (Type)
			{
			default:
				return false;
			case ValueKind.Object:
				return ptr == rhs;
			case ValueKind.Reference:
				return ((IProperties)ptr).Get(str).Equals(rhs);
			case ValueKind.String:
				return rhs is string	&& str			== (string)rhs;
			case ValueKind.Char:
				return rhs is char		&& data.Char	== (char)rhs;
			case ValueKind.Bool:
				return rhs is bool		&& data.Bool	== (bool)rhs;
			case ValueKind.Byte:
				return rhs is byte		&& data.Byte	== (byte)rhs;
			case ValueKind.UShort:
				return rhs is ushort	&& data.UShort	== (ushort)rhs;
			case ValueKind.UInt:
				return rhs is uint		&& data.UInt	== (uint)rhs;
			case ValueKind.ULong:
				return rhs is ulong		&& data.ULong	== (ulong)rhs;
			case ValueKind.SByte:
				return rhs is sbyte		&& data.SByte	== (sbyte)rhs;
			case ValueKind.Short:
				return rhs is short		&& data.Short	== (short)rhs;
			case ValueKind.Int:
				return rhs is int		&& data.Int		== (int)rhs;
			case ValueKind.Long:
				return rhs is long		&& data.Long	== (long)rhs;
			case ValueKind.Float:
				return rhs is float		&& data.Float	== (float)rhs;
			case ValueKind.Double:
				return rhs is double	&& data.Double	== (double)rhs;
			}
		}

		public override int GetHashCode()
		{
			switch (Type)
			{
			default:
				return ~0;
			case ValueKind.Object:
				return ptr == null ? 0 : ptr.GetHashCode();
			case ValueKind.Reference:
				return ((IProperties)ptr).Get(str).GetHashCode();
			case ValueKind.String:
				return str.GetHashCode();
			case ValueKind.Char:
			case ValueKind.Bool:
			case ValueKind.Byte:
			case ValueKind.UShort:
			case ValueKind.UInt:
			case ValueKind.ULong:
			case ValueKind.SByte:
			case ValueKind.Short:
			case ValueKind.Int:
			case ValueKind.Long:
				return data.Long.GetHashCode();
			case ValueKind.Float:
			case ValueKind.Double:
				return data.Double.GetHashCode();
			}
		}

		public static bool operator <(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.Type == ValueKind.String)
				return string.Compare(lhs.str, rhs.String, Culture, CompareOptions.None) < 0;
			if (rhs.Type == ValueKind.String)
				return string.Compare(lhs.String, rhs.str, Culture, CompareOptions.None) < 0;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Type == ValueKind.Float && !rhs.Is64)
						return lhs.data.Float < rhs.Float;
					return lhs.data.Double < rhs.Double;
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Type == ValueKind.Float && !lhs.Is64)
						return lhs.Float < rhs.data.Float;
					return lhs.Double < rhs.data.Double;
				}
				if (lhs.Is64 || rhs.Is64)
					return lhs.Type == ValueKind.ULong ?
						lhs.data.ULong < rhs.data.ULong && (rhs.Type == ValueKind.ULong || rhs.data.Long >= 0) :
						lhs.data.Long < rhs.data.Long || (rhs.Type == ValueKind.ULong && lhs.data.Long < 0);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int < rhs.Int);
					return new Value(lhs.Long < rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt < rhs.UInt);
				return new Value(lhs.ULong < rhs.ULong);
			}
			return false;
		}

		public static bool operator <=(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.Type == ValueKind.String)
				return string.Compare(lhs.str, rhs.String, Culture, CompareOptions.None) <= 0;
			if (rhs.Type == ValueKind.String)
				return string.Compare(lhs.String, rhs.str, Culture, CompareOptions.None) <= 0;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Type == ValueKind.Float && !rhs.Is64)
						return lhs.data.Float <= rhs.Float;
					return lhs.data.Double <= rhs.Double;
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Type == ValueKind.Float && !lhs.Is64)
						return lhs.Float <= rhs.data.Float;
					return lhs.Double <= rhs.data.Double;
				}
				if (lhs.Is64 || rhs.Is64)
					return lhs.Type == ValueKind.ULong ?
						lhs.data.ULong <= rhs.data.ULong && (rhs.Type == ValueKind.ULong || rhs.data.Long >= 0) :
						lhs.data.Long <= rhs.data.Long || (rhs.Type == ValueKind.ULong && lhs.data.Long < 0);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int <= rhs.Int);
					return new Value(lhs.Long <= rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt <= rhs.UInt);
				return new Value(lhs.ULong <= rhs.ULong);
			}
			return false;
		}

		public static bool operator >(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.Type == ValueKind.String)
				return string.Compare(lhs.str, rhs.String, Culture, CompareOptions.None) > 0;
			if (rhs.Type == ValueKind.String)
				return string.Compare(lhs.String, rhs.str, Culture, CompareOptions.None) > 0;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Type == ValueKind.Float && !rhs.Is64)
						return lhs.data.Float > rhs.Float;
					return lhs.data.Double > rhs.Double;
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Type == ValueKind.Float && !lhs.Is64)
						return lhs.Float > rhs.data.Float;
					return lhs.Double > rhs.data.Double;
				}
				if (lhs.Is64 || rhs.Is64)
					return lhs.Type == ValueKind.ULong ?
						lhs.data.ULong > rhs.data.ULong && (rhs.Type == ValueKind.ULong || rhs.data.Long < 0) :
						lhs.data.Long > rhs.data.Long || (rhs.Type == ValueKind.ULong && lhs.data.Long >= 0);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize > 4)
						return new Value(lhs.Int > rhs.Int);
					return new Value(lhs.Long > rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt > rhs.UInt);
				return new Value(lhs.ULong > rhs.ULong);
			}
			return false;
		}

		public static bool operator >=(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			if (lhs.Type == ValueKind.String)
				return string.Compare(lhs.str, rhs.String, Culture, CompareOptions.None) >= 0;
			if (rhs.Type == ValueKind.String)
				return string.Compare(lhs.String, rhs.str, Culture, CompareOptions.None) >= 0;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Type == ValueKind.Float && !rhs.Is64)
						return lhs.data.Float >= rhs.Float;
					return lhs.data.Double >= rhs.Double;
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Type == ValueKind.Float && !lhs.Is64)
						return lhs.Float >= rhs.data.Float;
					return lhs.Double >= rhs.data.Double;
				}
				if (lhs.Is64 || rhs.Is64)
					return lhs.Type == ValueKind.ULong ?
						lhs.data.ULong >= rhs.data.ULong && (rhs.Type == ValueKind.ULong || rhs.data.Long < 0) :
						lhs.data.Long >= rhs.data.Long || (rhs.Type == ValueKind.ULong && lhs.data.Long >= 0);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize > 4)
						return new Value(lhs.Int >= rhs.Int);
					return new Value(lhs.Long >= rhs.Long);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt >= rhs.UInt);
				return new Value(lhs.ULong >= rhs.ULong);
			}
			return false;
		}

		public bool Identical(Value rhs)
			=> Identical(this, rhs);
		public static bool Identical(Value lhs, Value rhs)
		{
			if (lhs.Type == ValueKind.Reference)
				lhs = ((IProperties)lhs.ptr).Get(lhs.str);
			if (rhs.Type == ValueKind.Reference)
				rhs = ((IProperties)rhs.ptr).Get(rhs.str);
			return lhs.Type == rhs.Type && lhs.ptr == rhs.ptr
				&& lhs.str == rhs.str && lhs.data.Long == rhs.data.Long;
		}
	}
}
