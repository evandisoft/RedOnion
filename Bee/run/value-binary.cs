using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bee.Run
{
	public partial struct Value
	{
		public static Value operator +( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Vt == Vtype.String )
			{
				return new Value( lhs.Str + rhs.String );
			}
			if( rhs.Vt == Vtype.String )
			{
				return new Value( lhs.String + rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp )
				{
					if( (lhs.Vt == Vtype.Float) && (!rhs.Is64) )
					{
						return new Value( lhs.Dta.Float + rhs.Float );
					}
					return new Value( lhs.Dta.Double + rhs.Double );
				}
				if( rhs.Isfp )
				{
					if( (rhs.Vt == Vtype.Float) && (!lhs.Is64) )
					{
						return new Value( lhs.Float + rhs.Dta.Float );
					}
					return new Value( lhs.Double + rhs.Dta.Double );
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return new Value( lhs.Is64 ? lhs.Vt : rhs.Vt, lhs.Long + rhs.Long );
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return new Value( lhs.Int + rhs.Int );
					}
					return new Value( lhs.Long + rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint + rhs.Uint );
				}
				return new Value( lhs.Ulong + rhs.Ulong );
			}
			return new Value();
		}//+
		
		public static Value operator -( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp )
				{
					if( (lhs.Vt == Vtype.Float) && (!rhs.Is64) )
					{
						return new Value( lhs.Dta.Float - rhs.Float );
					}
					return new Value( lhs.Dta.Double - rhs.Double );
				}
				if( rhs.Isfp )
				{
					if( (rhs.Vt == Vtype.Float) && (!lhs.Is64) )
					{
						return new Value( lhs.Float - rhs.Dta.Float );
					}
					return new Value( lhs.Double - rhs.Dta.Double );
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return new Value( lhs.Is64 ? lhs.Vt : rhs.Vt, lhs.Long - rhs.Long );
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return new Value( lhs.Int - rhs.Int );
					}
					return new Value( lhs.Long - rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint - rhs.Uint );
				}
				return new Value( lhs.Ulong - rhs.Ulong );
			}
			return new Value();
		}//-
		
		public static Value operator *( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp )
				{
					if( (lhs.Vt == Vtype.Float) && (!rhs.Is64) )
					{
						return new Value( lhs.Dta.Float * rhs.Float );
					}
					return new Value( lhs.Dta.Double * rhs.Double );
				}
				if( rhs.Isfp )
				{
					if( (rhs.Vt == Vtype.Float) && (!lhs.Is64) )
					{
						return new Value( lhs.Float * rhs.Dta.Float );
					}
					return new Value( lhs.Double * rhs.Dta.Double );
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return new Value( lhs.Is64 ? lhs.Vt : rhs.Vt, lhs.Long * rhs.Long );
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return new Value( lhs.Int * rhs.Int );
					}
					return new Value( lhs.Long * rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint * rhs.Uint );
				}
				return new Value( lhs.Ulong * rhs.Ulong );
			}
			return new Value();
		}//*
		
		public static Value operator /( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp )
				{
					if( (lhs.Vt == Vtype.Float) && (!rhs.Is64) )
					{
						return new Value( lhs.Dta.Float / rhs.Float );
					}
					return new Value( lhs.Dta.Double / rhs.Double );
				}
				if( rhs.Isfp )
				{
					if( (rhs.Vt == Vtype.Float) && (!lhs.Is64) )
					{
						return new Value( lhs.Float / rhs.Dta.Float );
					}
					return new Value( lhs.Double / rhs.Dta.Double );
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					var @long = rhs.Long;
					if( @long == 0 )
					{
						return new Value();
					}
					return new Value( lhs.Is64 ? lhs.Vt : rhs.Vt, lhs.Long / @long );
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						var @int = rhs.Int;
						if( @int == 0 )
						{
							return new Value();
						}
						return new Value( lhs.Int / @int );
					}
					var @long = rhs.Long;
					if( @long == 0 )
					{
						return new Value();
					}
					return new Value( lhs.Long / @long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					var @uint = rhs.Uint;
					if( @uint == 0 )
					{
						return new Value();
					}
					return new Value( lhs.Uint / @uint );
				}
				var @ulong = rhs.Ulong;
				if( @ulong == 0 )
				{
					return new Value();
				}
				return new Value( lhs.Ulong / @ulong );
			}
			return new Value();
		}///
		
		public static Value operator %( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp )
				{
					if( (lhs.Vt == Vtype.Float) && (!rhs.Is64) )
					{
						return new Value( lhs.Dta.Float % rhs.Float );
					}
					return new Value( lhs.Dta.Double % rhs.Double );
				}
				if( rhs.Isfp )
				{
					if( (rhs.Vt == Vtype.Float) && (!lhs.Is64) )
					{
						return new Value( lhs.Float % rhs.Dta.Float );
					}
					return new Value( lhs.Double % rhs.Dta.Double );
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					var @long = rhs.Long;
					if( @long == 0 )
					{
						return new Value();
					}
					return new Value( lhs.Is64 ? lhs.Vt : rhs.Vt, lhs.Long % @long );
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						var @int = rhs.Int;
						if( @int == 0 )
						{
							return new Value();
						}
						return new Value( lhs.Int % @int );
					}
					var @long = rhs.Long;
					if( @long == 0 )
					{
						return new Value();
					}
					return new Value( lhs.Long % @long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					var @uint = rhs.Uint;
					if( @uint == 0 )
					{
						return new Value();
					}
					return new Value( lhs.Uint % @uint );
				}
				var @ulong = rhs.Ulong;
				if( @ulong == 0 )
				{
					return new Value();
				}
				return new Value( lhs.Ulong % @ulong );
			}
			return new Value();
		}//%
		
		public static Value operator &( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Vt == Vtype.String )
			{
				return new Value( lhs.Str + rhs.String );
			}
			if( rhs.Vt == Vtype.String )
			{
				return new Value( lhs.String + rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp || rhs.Isfp )
				{
					return new Value( lhs.Long & rhs.Long );
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return new Value( lhs.Is64 ? lhs.Vt : rhs.Vt, lhs.Long & rhs.Long );
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return new Value( lhs.Int & rhs.Int );
					}
					return new Value( lhs.Long & rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint & rhs.Uint );
				}
				return new Value( lhs.Ulong & rhs.Ulong );
			}
			return new Value();
		}//&
		
		public static Value operator |( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Vt == Vtype.String )
			{
				return new Value( lhs.Str + rhs.String );
			}
			if( rhs.Vt == Vtype.String )
			{
				return new Value( lhs.String + rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp || rhs.Isfp )
				{
					return new Value( lhs.Long | rhs.Long );
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return new Value( lhs.Is64 ? lhs.Vt : rhs.Vt, lhs.Long | rhs.Long );
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return new Value( lhs.Int | rhs.Int );
					}
					return new Value( lhs.Long | rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint | rhs.Uint );
				}
				return new Value( lhs.Ulong | rhs.Ulong );
			}
			return new Value();
		}//|
		
		public static Value operator ^( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp )
				{
					if( (lhs.Vt == Vtype.Float) && (!rhs.Is64) )
					{
						return new Value( Math.Pow( lhs.Dta.Float, rhs.Float ) );
					}
					return new Value( Math.Pow( lhs.Dta.Double, rhs.Double ) );
				}
				if( rhs.Isfp )
				{
					if( (rhs.Vt == Vtype.Float) && (!lhs.Is64) )
					{
						return new Value( Math.Pow( lhs.Float, rhs.Dta.Float ) );
					}
					return new Value( Math.Pow( lhs.Double, rhs.Dta.Double ) );
				}
				if( lhs.Isfp || rhs.Isfp )
				{
					return new Value( lhs.Long ^ rhs.Long );
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return new Value( lhs.Is64 ? lhs.Vt : rhs.Vt, lhs.Long ^ rhs.Long );
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return new Value( lhs.Int ^ rhs.Int );
					}
					return new Value( lhs.Long ^ rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint ^ rhs.Uint );
				}
				return new Value( lhs.Ulong ^ rhs.Ulong );
			}
			return new Value();
		}//^
		
		public Value ShiftLeft( Value by )
		{
			return ShiftLeft( this, by );
		}//ShiftLeft
		
		public static Value ShiftLeft( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Vt == Vtype.String )
			{
				return new Value( lhs.Str + rhs.String );
			}
			if( rhs.Vt == Vtype.String )
			{
				return new Value( lhs.String + rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp || rhs.Isfp )
				{
					return new Value( lhs.Long << rhs.Int );
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return new Value( lhs.Is64 ? lhs.Vt : rhs.Vt, lhs.Long << rhs.Int );
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return new Value( lhs.Int << rhs.Int );
					}
					return new Value( lhs.Long << rhs.Int );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint << rhs.Int );
				}
				return new Value( lhs.Ulong << rhs.Int );
			}
			return new Value();
		}//ShiftLeft
		
		public Value ShiftRight( Value by )
		{
			return ShiftLeft( this, by );
		}//ShiftRight
		
		public static Value ShiftRight( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp || rhs.Isfp )
				{
					return new Value( lhs.Long >> rhs.Int );
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return new Value( lhs.Is64 ? lhs.Vt : rhs.Vt, lhs.Long >> rhs.Int );
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return new Value( lhs.Int >> rhs.Int );
					}
					return new Value( lhs.Long >> rhs.Int );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint >> rhs.Int );
				}
				return new Value( lhs.Ulong >> rhs.Int );
			}
			return new Value();
		}//ShiftRight
		
		public static bool operator ==( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp || rhs.Isfp )
				{
					return lhs.Double == rhs.Double;
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return lhs.Long == rhs.Long;
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return lhs.Int == rhs.Int;
					}
					return new Value( lhs.Long == rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return lhs.Uint == rhs.Uint;
				}
				return lhs.Ulong == rhs.Ulong;
			}
			switch( lhs.Vt )
			{
			case Vtype.Undef:
				return (rhs.Vt == Vtype.Undef) || ((rhs.Vt == Vtype.Object) && (rhs.Ptr == null));
			case Vtype.Object:
				return ((rhs.Vt == Vtype.Object) || (rhs.Vt == Vtype.Undef)) && (lhs.Ptr == rhs.Ptr);
			}
			switch( rhs.Vt )
			{
			case Vtype.Undef:
			case Vtype.Object:
				return false;
			}
			return lhs.String == rhs.String;
		}//==
		
		public static bool operator !=( Value lhs, Value rhs )
		{
			return !(lhs == rhs);
		}//!=
		
		public override bool Equals( Object @object )
		{
			if( @object is Value )
			{
				return this == ((Value)@object);
			}
			switch( Vt )
			{
			default:
				return false;
			case Vtype.Object:
				return Ptr == @object;
			case Vtype.Ident:
				return ((IProps)Ptr).Get( Str ).Equals( @object );
			case Vtype.String:
				return (@object is System.String) && (Str == ((string)@object));
			case Vtype.Char:
				return (@object is System.Char) && (Dta.Char == ((char)@object));
			case Vtype.Bool:
				return (@object is System.Boolean) && (Dta.Bool == ((bool)@object));
			case Vtype.Byte:
				return (@object is System.Byte) && (Dta.Byte == unchecked((byte)@object));
			case Vtype.Ushort:
				return (@object is System.UInt16) && (Dta.Ushort == ((ushort)@object));
			case Vtype.Uint:
				return (@object is System.UInt32) && (Dta.Uint == ((uint)@object));
			case Vtype.Ulong:
				return (@object is System.UInt64) && (Dta.Ulong == ((ulong)@object));
			case Vtype.Sbyte:
				return (@object is System.SByte) && (Dta.Sbyte == ((sbyte)@object));
			case Vtype.Short:
				return (@object is System.Int16) && (Dta.Short == ((short)@object));
			case Vtype.Int:
				return (@object is System.Int32) && (Dta.Int == ((int)@object));
			case Vtype.Long:
				return (@object is System.Int64) && (Dta.Long == ((long)@object));
			case Vtype.Float:
				return (@object is System.Single) && (Dta.Float == ((float)@object));
			case Vtype.Double:
				return (@object is System.Double) && (Dta.Double == ((double)@object));
			}
		}//Equals
		
		public override int GetHashCode(  )
		{
			switch( Vt )
			{
			default:
				return ~0;
			case Vtype.Object:
				return Ptr == null ? 0 : Ptr.GetHashCode();
			case Vtype.Ident:
				return ((IProps)Ptr).Get( Str ).GetHashCode();
			case Vtype.String:
				return Str.GetHashCode();
			case Vtype.Char:
			case Vtype.Bool:
			case Vtype.Byte:
			case Vtype.Ushort:
			case Vtype.Uint:
			case Vtype.Ulong:
			case Vtype.Sbyte:
			case Vtype.Short:
			case Vtype.Int:
			case Vtype.Long:
				return Dta.Long.GetHashCode();
			case Vtype.Float:
			case Vtype.Double:
				return Dta.Double.GetHashCode();
			}
		}//GetHashCode
		
		public static bool operator <( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Vt == Vtype.String )
			{
				return System.String.Compare( lhs.Str, rhs.String, Culture, CompareOptions.None ) < 0;
			}
			if( rhs.Vt == Vtype.String )
			{
				return System.String.Compare( lhs.String, rhs.Str, Culture, CompareOptions.None ) < 0;
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp )
				{
					if( (lhs.Vt == Vtype.Float) && (!rhs.Is64) )
					{
						return lhs.Dta.Float < rhs.Float;
					}
					return lhs.Dta.Double < rhs.Double;
				}
				if( rhs.Isfp )
				{
					if( (rhs.Vt == Vtype.Float) && (!lhs.Is64) )
					{
						return lhs.Float < rhs.Dta.Float;
					}
					return lhs.Double < rhs.Dta.Double;
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return lhs.Vt == Vtype.Ulong ? (lhs.Dta.Ulong < rhs.Dta.Ulong) && ((rhs.Vt == Vtype.Ulong) || (rhs.Dta.Long >= 0)) : (lhs.Dta.Long < rhs.Dta.Long) || ((rhs.Vt == Vtype.Ulong) && (lhs.Dta.Long < 0));
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return new Value( lhs.Int < rhs.Int );
					}
					return new Value( lhs.Long < rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint < rhs.Uint );
				}
				return new Value( lhs.Ulong < rhs.Ulong );
			}
			return false;
		}//<
		
		public static bool operator <=( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Vt == Vtype.String )
			{
				return System.String.Compare( lhs.Str, rhs.String, Culture, CompareOptions.None ) <= 0;
			}
			if( rhs.Vt == Vtype.String )
			{
				return System.String.Compare( lhs.String, rhs.Str, Culture, CompareOptions.None ) <= 0;
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp )
				{
					if( (lhs.Vt == Vtype.Float) && (!rhs.Is64) )
					{
						return lhs.Dta.Float <= rhs.Float;
					}
					return lhs.Dta.Double <= rhs.Double;
				}
				if( rhs.Isfp )
				{
					if( (rhs.Vt == Vtype.Float) && (!lhs.Is64) )
					{
						return lhs.Float <= rhs.Dta.Float;
					}
					return lhs.Double <= rhs.Dta.Double;
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return lhs.Vt == Vtype.Ulong ? (lhs.Dta.Ulong <= rhs.Dta.Ulong) && ((rhs.Vt == Vtype.Ulong) || (rhs.Dta.Long >= 0)) : (lhs.Dta.Long <= rhs.Dta.Long) || ((rhs.Vt == Vtype.Ulong) && (lhs.Dta.Long < 0));
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz < 4) )
					{
						return new Value( lhs.Int <= rhs.Int );
					}
					return new Value( lhs.Long <= rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint <= rhs.Uint );
				}
				return new Value( lhs.Ulong <= rhs.Ulong );
			}
			return false;
		}//<=
		
		public static bool operator >( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Vt == Vtype.String )
			{
				return System.String.Compare( lhs.Str, rhs.String, Culture, CompareOptions.None ) > 0;
			}
			if( rhs.Vt == Vtype.String )
			{
				return System.String.Compare( lhs.String, rhs.Str, Culture, CompareOptions.None ) > 0;
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp )
				{
					if( (lhs.Vt == Vtype.Float) && (!rhs.Is64) )
					{
						return lhs.Dta.Float > rhs.Float;
					}
					return lhs.Dta.Double > rhs.Double;
				}
				if( rhs.Isfp )
				{
					if( (rhs.Vt == Vtype.Float) && (!lhs.Is64) )
					{
						return lhs.Float > rhs.Dta.Float;
					}
					return lhs.Double > rhs.Dta.Double;
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return lhs.Vt == Vtype.Ulong ? (lhs.Dta.Ulong > rhs.Dta.Ulong) && ((rhs.Vt == Vtype.Ulong) || (rhs.Dta.Long < 0)) : (lhs.Dta.Long > rhs.Dta.Long) || ((rhs.Vt == Vtype.Ulong) && (lhs.Dta.Long >= 0));
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz > 4) )
					{
						return new Value( lhs.Int > rhs.Int );
					}
					return new Value( lhs.Long > rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint > rhs.Uint );
				}
				return new Value( lhs.Ulong > rhs.Ulong );
			}
			return false;
		}//>
		
		public static bool operator >=( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			if( lhs.Vt == Vtype.String )
			{
				return System.String.Compare( lhs.Str, rhs.String, Culture, CompareOptions.None ) >= 0;
			}
			if( rhs.Vt == Vtype.String )
			{
				return System.String.Compare( lhs.String, rhs.Str, Culture, CompareOptions.None ) >= 0;
			}
			if( lhs.Isnum && rhs.Isnum )
			{
				if( lhs.Isfp )
				{
					if( (lhs.Vt == Vtype.Float) && (!rhs.Is64) )
					{
						return lhs.Dta.Float >= rhs.Float;
					}
					return lhs.Dta.Double >= rhs.Double;
				}
				if( rhs.Isfp )
				{
					if( (rhs.Vt == Vtype.Float) && (!lhs.Is64) )
					{
						return lhs.Float >= rhs.Dta.Float;
					}
					return lhs.Double >= rhs.Dta.Double;
				}
				if( lhs.Is64 || rhs.Is64 )
				{
					return lhs.Vt == Vtype.Ulong ? (lhs.Dta.Ulong >= rhs.Dta.Ulong) && ((rhs.Vt == Vtype.Ulong) || (rhs.Dta.Long < 0)) : (lhs.Dta.Long >= rhs.Dta.Long) || ((rhs.Vt == Vtype.Ulong) && (lhs.Dta.Long >= 0));
				}
				if( lhs.Signed )
				{
					if( rhs.Signed || (rhs.Numsz > 4) )
					{
						return new Value( lhs.Int >= rhs.Int );
					}
					return new Value( lhs.Long >= rhs.Long );
				}
				if( (!rhs.Signed) || (rhs.Numsz < 4) )
				{
					return new Value( lhs.Uint >= rhs.Uint );
				}
				return new Value( lhs.Ulong >= rhs.Ulong );
			}
			return false;
		}//>=
		
		public bool Identical( Value rhs )
		{
			return Identical( this, rhs );
		}//Identical
		
		public static bool Identical( Value lhs, Value rhs )
		{
			if( lhs.Vt == Vtype.Ident )
			{
				lhs = ((IProps)lhs.Ptr).Get( lhs.Str );
			}
			if( rhs.Vt == Vtype.Ident )
			{
				rhs = ((IProps)rhs.Ptr).Get( rhs.Str );
			}
			return (((lhs.Vt == rhs.Vt) && (lhs.Ptr == rhs.Ptr)) && (lhs.Str == rhs.Str)) && (lhs.Dta.Long == rhs.Dta.Long);
		}//Identical
	}//Value
}//Bee.Run
