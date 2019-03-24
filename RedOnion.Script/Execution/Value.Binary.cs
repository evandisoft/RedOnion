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
			switch (op)
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
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.Kind == ValueKind.String)
				return new Value((string)lhs.ptr + rhs.String);
			if (rhs.Kind == ValueKind.String)
				return new Value(lhs.String + (string)rhs.ptr);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Kind == ValueKind.Float && !rhs.Is64)
						return new Value(lhs.data.Float + rhs.Float);
					return new Value(lhs.data.Double + rhs.Double);
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Kind == ValueKind.Float && !lhs.Is64)
						return new Value(lhs.Float + rhs.data.Float);
					return new Value(lhs.Double + rhs.data.Double);
				}
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Kind : rhs.Kind, lhs.Long + rhs.Long);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Add, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Add, lhs, true, out var result))
					return result;
			}
			return new Value();
		}

		public static Value operator -(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Kind == ValueKind.Float && !rhs.Is64)
						return new Value(lhs.data.Float - rhs.Float);
					return new Value(lhs.data.Double - rhs.Double);
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Kind == ValueKind.Float && !lhs.Is64)
						return new Value(lhs.Float - rhs.data.Float);
					return new Value(lhs.Double - rhs.data.Double);
				}
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Kind : rhs.Kind, lhs.Long - rhs.Long);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Sub, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Sub, lhs, true, out var result))
					return result;
			}
			return new Value();
		}

		public static Value operator *(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Kind == ValueKind.Float && !rhs.Is64)
						return new Value(lhs.data.Float * rhs.Float);
					return new Value(lhs.data.Double * rhs.Double);
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Kind == ValueKind.Float && !lhs.Is64)
						return new Value(lhs.Float * rhs.data.Float);
					return new Value(lhs.Double * rhs.data.Double);
				}
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Kind : rhs.Kind, lhs.Long * rhs.Long);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Mul, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Mul, lhs, true, out var result))
					return result;
			}
			return new Value();
		}

		public static Value operator /(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Kind == ValueKind.Float && !rhs.Is64)
						return new Value(lhs.data.Float / rhs.Float);
					return new Value(lhs.data.Double / rhs.Double);
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Kind == ValueKind.Float && !lhs.Is64)
						return new Value(lhs.Float / rhs.data.Float);
					return new Value(lhs.Double / rhs.data.Double);
				}
				if (lhs.Is64 || rhs.Is64)
				{
					var v = rhs.Long;
					if (v == 0)
						return new Value();
					return new Value(lhs.Is64 ? lhs.Kind : rhs.Kind, lhs.Long / v);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Div, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Div, lhs, true, out var result))
					return result;
			}
			return new Value();
		}

		public static Value operator %(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Kind == ValueKind.Float && !rhs.Is64)
						return new Value(lhs.data.Float % rhs.Float);
					return new Value(lhs.data.Double % rhs.Double);
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Kind == ValueKind.Float && !lhs.Is64)
						return new Value(lhs.Float % rhs.data.Float);
					return new Value(lhs.Double % rhs.data.Double);
				}
				if (lhs.Is64 || rhs.Is64)
				{
					var v = rhs.Long;
					if (v == 0)
						return new Value();
					return new Value(lhs.Is64 ? lhs.Kind : rhs.Kind, lhs.Long % v);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Mod, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Mod, lhs, true, out var result))
					return result;
			}
			return new Value();
		}

		public static Value operator &(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.Kind == ValueKind.String)
				return new Value((string)lhs.ptr + rhs.String);
			if (rhs.Kind == ValueKind.String)
				return new Value(lhs.String + (string)rhs.ptr);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return new Value(lhs.Long & rhs.Long);
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Kind : rhs.Kind, lhs.Long & rhs.Long).AdjustForEnum(lhs);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int & rhs.Int).AdjustForEnum(lhs);
					return new Value(lhs.Long & rhs.Long).AdjustForEnum(lhs);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt & rhs.UInt).AdjustForEnum(lhs);
				return new Value(lhs.ULong & rhs.ULong).AdjustForEnum(lhs);
			}
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.BitAnd, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.BitAnd, lhs, true, out var result))
					return result;
			}
			return new Value();
		}

		public static Value operator |(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.Kind == ValueKind.String)
				return new Value((string)lhs.ptr + rhs.String);
			if (rhs.Kind == ValueKind.String)
				return new Value(lhs.String + (string)rhs.ptr);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return new Value(lhs.Long | rhs.Long);
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Kind : rhs.Kind, lhs.Long | rhs.Long).AdjustForEnum(lhs);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int | rhs.Int).AdjustForEnum(lhs);
					return new Value(lhs.Long | rhs.Long).AdjustForEnum(lhs);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt | rhs.UInt).AdjustForEnum(lhs);
				return new Value(lhs.ULong | rhs.ULong).AdjustForEnum(lhs);
			}
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.BitOr, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.BitOr, lhs, true, out var result))
					return result;
			}
			return new Value();
		}

		public static Value operator ^(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Kind == ValueKind.Float && !rhs.Is64)
						return new Value(Math.Pow(lhs.data.Float, rhs.Float));
					return new Value(Math.Pow(lhs.data.Double, rhs.Double));
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Kind == ValueKind.Float && !lhs.Is64)
						return new Value(Math.Pow(lhs.Float, rhs.data.Float));
					return new Value(Math.Pow(lhs.Double, rhs.data.Double));
				}
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return new Value(lhs.Long ^ rhs.Long);
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Kind : rhs.Kind, lhs.Long ^ rhs.Long).AdjustForEnum(lhs);
				if (lhs.Signed)
				{
					if (rhs.Signed || rhs.NumberSize < 4)
						return new Value(lhs.Int ^ rhs.Int).AdjustForEnum(lhs);
					return new Value(lhs.Long ^ rhs.Long).AdjustForEnum(lhs);
				}
				if (!rhs.Signed || rhs.NumberSize < 4)
					return new Value(lhs.UInt ^ rhs.UInt).AdjustForEnum(lhs);
				return new Value(lhs.ULong ^ rhs.ULong).AdjustForEnum(lhs);
			}
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.BitXor, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.BitXor, lhs, true, out var result))
					return result;
			}
			return new Value();
		}

		public Value ShiftLeft(Value by)
			=> ShiftLeft(this, by);
		public static Value ShiftLeft(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.Kind == ValueKind.String)
				return new Value((string)lhs.ptr + rhs.String);
			if (rhs.Kind == ValueKind.String)
				return new Value(lhs.String + (string)rhs.ptr);
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return new Value(lhs.Long << rhs.Int);
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Kind : rhs.Kind, lhs.Long << rhs.Int);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.ShiftLeft, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.ShiftLeft, lhs, true, out var result))
					return result;
			}
			return new Value();
		}

		public Value ShiftRight(Value by)
			=> ShiftLeft(this, by);
		public static Value ShiftRight(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint || rhs.IsFloatigPoint)
					return new Value(lhs.Long >> rhs.Int);
				if (lhs.Is64 || rhs.Is64)
					return new Value(lhs.Is64 ? lhs.Kind : rhs.Kind, lhs.Long >> rhs.Int);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.ShiftRight, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.ShiftRight, lhs, true, out var result))
					return result;
			}
			return new Value();
		}

		public static bool operator ==(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
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
			switch (lhs.Kind)
			{
			case ValueKind.Undefined:
				// undefined == null (equality) but undefined !== null (not identical)
				return rhs.Kind == ValueKind.Undefined
					|| (rhs.Kind == ValueKind.Object && rhs.ptr == null);
			case ValueKind.Object:
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Equals, rhs, false, out var result))
					return result;
				// ptr is null for ValueType.Undefined
				return (rhs.Kind == ValueKind.Object || rhs.Kind == ValueKind.Undefined)
					&& lhs.ptr == rhs.ptr;
			}
			switch (rhs.Kind)
			{
			case ValueKind.Undefined:
				return false;
			case ValueKind.Object:
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Add, lhs, true, out var result))
					return result;
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
			switch (Kind)
			{
			default:
				return false;
			case ValueKind.Object:
				return ptr == rhs;
			case ValueKind.Reference:
				return ((IProperties)ptr).Get((string)idx).Equals(rhs);
			case ValueKind.IndexRef:
				return ((IObject)ptr).IndexGet((Value)idx).Equals(rhs);
			case ValueKind.String:
				return rhs is string	&& (string)ptr	== (string)rhs;
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
			if (IsReference)
				return RValue.GetHashCode();
			switch (Kind)
			{
			default:
				return ~0;
			case ValueKind.Object:
			case ValueKind.String:
				return ptr == null ? 0 : ptr.GetHashCode();
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
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.Kind == ValueKind.String)
				return string.Compare((string)lhs.ptr, rhs.String, Culture, CompareOptions.None) < 0;
			if (rhs.Kind == ValueKind.String)
				return string.Compare(lhs.String, (string)rhs.ptr, Culture, CompareOptions.None) < 0;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Kind == ValueKind.Float && !rhs.Is64)
						return lhs.data.Float < rhs.Float;
					return lhs.data.Double < rhs.Double;
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Kind == ValueKind.Float && !lhs.Is64)
						return lhs.Float < rhs.data.Float;
					return lhs.Double < rhs.data.Double;
				}
				if (lhs.Is64 || rhs.Is64)
					return lhs.Kind == ValueKind.ULong ?
						lhs.data.ULong < rhs.data.ULong && (rhs.Kind == ValueKind.ULong || rhs.data.Long >= 0) :
						lhs.data.Long < rhs.data.Long || (rhs.Kind == ValueKind.ULong && lhs.data.Long < 0);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Less, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.Less, lhs, true, out var result))
					return result;
			}
			return false;
		}

		public static bool operator <=(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.Kind == ValueKind.String)
				return string.Compare((string)lhs.ptr, rhs.String, Culture, CompareOptions.None) <= 0;
			if (rhs.Kind == ValueKind.String)
				return string.Compare(lhs.String, (string)rhs.ptr, Culture, CompareOptions.None) <= 0;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Kind == ValueKind.Float && !rhs.Is64)
						return lhs.data.Float <= rhs.Float;
					return lhs.data.Double <= rhs.Double;
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Kind == ValueKind.Float && !lhs.Is64)
						return lhs.Float <= rhs.data.Float;
					return lhs.Double <= rhs.data.Double;
				}
				if (lhs.Is64 || rhs.Is64)
					return lhs.Kind == ValueKind.ULong ?
						lhs.data.ULong <= rhs.data.ULong && (rhs.Kind == ValueKind.ULong || rhs.data.Long >= 0) :
						lhs.data.Long <= rhs.data.Long || (rhs.Kind == ValueKind.ULong && lhs.data.Long < 0);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.LessEq, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.LessEq, lhs, true, out var result))
					return result;
			}
			return false;
		}

		public static bool operator >(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.Kind == ValueKind.String)
				return string.Compare((string)lhs.ptr, rhs.String, Culture, CompareOptions.None) > 0;
			if (rhs.Kind == ValueKind.String)
				return string.Compare(lhs.String, (string)rhs.ptr, Culture, CompareOptions.None) > 0;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Kind == ValueKind.Float && !rhs.Is64)
						return lhs.data.Float > rhs.Float;
					return lhs.data.Double > rhs.Double;
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Kind == ValueKind.Float && !lhs.Is64)
						return lhs.Float > rhs.data.Float;
					return lhs.Double > rhs.data.Double;
				}
				if (lhs.Is64 || rhs.Is64)
					return lhs.Kind == ValueKind.ULong ?
						lhs.data.ULong > rhs.data.ULong && (rhs.Kind == ValueKind.ULong || rhs.data.Long < 0) :
						lhs.data.Long > rhs.data.Long || (rhs.Kind == ValueKind.ULong && lhs.data.Long >= 0);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.More, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.More, lhs, true, out var result))
					return result;
			}
			return false;
		}

		public static bool operator >=(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			if (lhs.Kind == ValueKind.String)
				return string.Compare((string)lhs.ptr, rhs.String, Culture, CompareOptions.None) >= 0;
			if (rhs.Kind == ValueKind.String)
				return string.Compare(lhs.String, (string)rhs.ptr, Culture, CompareOptions.None) >= 0;
			if (lhs.IsNumber && rhs.IsNumber)
			{
				if (lhs.IsFloatigPoint)
				{
					if (lhs.Kind == ValueKind.Float && !rhs.Is64)
						return lhs.data.Float >= rhs.Float;
					return lhs.data.Double >= rhs.Double;
				}
				if (rhs.IsFloatigPoint)
				{
					if (rhs.Kind == ValueKind.Float && !lhs.Is64)
						return lhs.Float >= rhs.data.Float;
					return lhs.Double >= rhs.data.Double;
				}
				if (lhs.Is64 || rhs.Is64)
					return lhs.Kind == ValueKind.ULong ?
						lhs.data.ULong >= rhs.data.ULong && (rhs.Kind == ValueKind.ULong || rhs.data.Long < 0) :
						lhs.data.Long >= rhs.data.Long || (rhs.Kind == ValueKind.ULong && lhs.data.Long >= 0);
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
			if (lhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)lhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.MoreEq, rhs, false, out var result))
					return result;
			}
			if (rhs.Kind == ValueKind.Object)
			{
				var obj = (IObject)rhs.ptr;
				if (obj?.HasFeature(ObjectFeatures.Operators) == true
					&& obj.Operator(OpCode.MoreEq, lhs, true, out var result))
					return result;
			}
			return false;
		}

		public bool Identical(Value rhs)
			=> Identical(this, rhs);
		public static bool Identical(Value lhs, Value rhs)
		{
			lhs = lhs.RValue;
			rhs = rhs.RValue;
			return lhs.Kind == rhs.Kind && lhs.ptr == rhs.ptr
				&& lhs.idx == rhs.idx && lhs.data.Long == rhs.data.Long;
		}
	}
}
