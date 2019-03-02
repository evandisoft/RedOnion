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
		public static Value operator !( Value value )
		{
			return !value.Bool;
		}//!
		
		public static Value operator +( Value value )
		{
		again:
			if( value.Isnum )
			{
				return value;
			}
			if( value.Vt == Vtype.Ident )
			{
				value = ((IProps)value.Ptr).Get( value.Str );
				goto again;
			}
			return new Value();
		}//+
		
		public static Value operator -( Value value )
		{
		again:
			switch( value.Vt )
			{
			default:
				return new Value();
			case Vtype.Ident:
				value = ((IProps)value.Ptr).Get( value.Str );
				goto again;
			case Vtype.Char:
				return new Value() - value.Dta.Char;
			case Vtype.Bool:
				return new Value( value.Dta.Bool ? -1 : 0 );
			case Vtype.Byte:
				return new Value() - value.Dta.Byte;
			case Vtype.Ushort:
				return new Value() - value.Dta.Ushort;
			case Vtype.Uint:
				return new Value() - value.Dta.Uint;
			case Vtype.Ulong:
				return new Value() - value.Dta.Ulong;
			case Vtype.Sbyte:
				return new Value() - value.Dta.Sbyte;
			case Vtype.Short:
				return new Value() - value.Dta.Short;
			case Vtype.Int:
				return new Value() - value.Dta.Int;
			case Vtype.Long:
				return new Value() - value.Dta.Long;
			case Vtype.Float:
				return new Value() - value.Dta.Float;
			case Vtype.Double:
				return new Value() - value.Dta.Double;
			}
		}//-
		
		public static Value operator ~( Value value )
		{
		again:
			switch( value.Vt )
			{
			default:
				return new Value();
			case Vtype.Ident:
				value = ((IProps)value.Ptr).Get( value.Str );
				goto again;
			case Vtype.Char:
				return new Value( ~value.Dta.Char );
			case Vtype.Bool:
				return new Value( !value.Dta.Bool );
			case Vtype.Byte:
				return new Value( ~value.Dta.Byte );
			case Vtype.Ushort:
				return new Value( ~value.Dta.Ushort );
			case Vtype.Uint:
				return new Value( ~value.Dta.Uint );
			case Vtype.Ulong:
				return new Value( ~value.Dta.Ulong );
			case Vtype.Sbyte:
				return new Value( ~value.Dta.Sbyte );
			case Vtype.Short:
				return new Value( ~value.Dta.Short );
			case Vtype.Int:
				return new Value( ~value.Dta.Int );
			case Vtype.Long:
				return new Value( ~value.Dta.Long );
			case Vtype.Float:
				return new Value( ~((long)value.Dta.Float) );
			case Vtype.Double:
				return new Value( ~((long)value.Dta.Double) );
			}
		}//~
		
		public static Value operator ++( Value value )
		{
		again:
			switch( value.Vt )
			{
			default:
				return new Value();
			case Vtype.Ident:
				value = ((IProps)value.Ptr).Get( value.Str );
				goto again;
			case Vtype.Char:
				return new Value( ((char)(value.Dta.Char + 1)) );
			case Vtype.Bool:
				return new Value( true );
			case Vtype.Byte:
				return new Value( unchecked((byte)(value.Dta.Byte + 1u)) );
			case Vtype.Ushort:
				return new Value( ((ushort)(value.Dta.Ushort + 1u)) );
			case Vtype.Uint:
				return new Value( value.Dta.Uint + 1u );
			case Vtype.Ulong:
				return new Value( value.Dta.Ulong + 1u );
			case Vtype.Sbyte:
				return new Value( ((sbyte)(value.Dta.Sbyte + 1)) );
			case Vtype.Short:
				return new Value( ((short)(value.Dta.Short + 1)) );
			case Vtype.Int:
				return new Value( value.Dta.Int + 1 );
			case Vtype.Long:
				return new Value( value.Dta.Long + 1 );
			case Vtype.Float:
				return new Value( value.Dta.Float + 1f );
			case Vtype.Double:
				return new Value( value.Dta.Double + 1.0 );
			}
		}//++
		
		public static Value operator --( Value value )
		{
		again:
			switch( value.Vt )
			{
			default:
				return new Value();
			case Vtype.Ident:
				value = ((IProps)value.Ptr).Get( value.Str );
				goto again;
			case Vtype.Char:
				return new Value( ((char)(value.Dta.Char - 1)) );
			case Vtype.Bool:
				return new Value( false );
			case Vtype.Byte:
				return new Value( unchecked((byte)(value.Dta.Byte - 1u)) );
			case Vtype.Ushort:
				return new Value( ((ushort)(value.Dta.Ushort - 1u)) );
			case Vtype.Uint:
				return new Value( value.Dta.Uint - 1u );
			case Vtype.Ulong:
				return new Value( value.Dta.Ulong - 1u );
			case Vtype.Sbyte:
				return new Value( ((sbyte)(value.Dta.Sbyte - 1)) );
			case Vtype.Short:
				return new Value( ((short)(value.Dta.Short - 1)) );
			case Vtype.Int:
				return new Value( value.Dta.Int - 1 );
			case Vtype.Long:
				return new Value( value.Dta.Long - 1 );
			case Vtype.Float:
				return new Value( value.Dta.Float - 1f );
			case Vtype.Double:
				return new Value( value.Dta.Double - 1.0 );
			}
		}//--
	}//Value
}//Bee.Run
