using Bee.Run;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bee.Tests
{
	public class BeeCodeTestsBase: CodeGenerator
	{
		public int Cint( int @int )
		{
			return Bits.Int( Code, @int );
		}//Cint
		
		public float Cfloat( int @int )
		{
			return Bits.Float( Code, @int );
		}//Cfloat
		
		public long Clong( int @int )
		{
			return Bits.Long( Code, @int );
		}//Clong
		
		public double Cdouble( int @int )
		{
			return Bits.Double( Code, @int );
		}//Cdouble
		
		public void Check( int at, byte @byte )
		{
			Assert.IsTrue( at < CodeAt, "code: {0} >= {1}", at, CodeAt );
			Assert.AreEqual( @byte, Code[at], "code[{0}] = 0x{1:X2} ! 0x{2:X2}", at, Code[at], @byte );
		}//Check
		
		public void Check( int at, int @int )
		{
			Assert.AreEqual( @int, Cint( at ), "cint({0})", at );
		}//Check
		
		public void Check( int at, float @float )
		{
			Assert.AreEqual( @float, Cfloat( at ), "cfloat({0})", at );
		}//Check
		
		public void Check( int at, long @long )
		{
			Assert.AreEqual( @long, Clong( at ), "clong({0})", at );
		}//Check
		
		public void Check( int at, double @double )
		{
			Assert.AreEqual( @double, Cdouble( at ), "cdouble({0})", at );
		}//Check
		
		public void Check( int at )
		{
			Assert.AreEqual( at, CodeAt, "codeAt" );
		}//Check
		
		public void Vcheck( int at, byte @byte )
		{
			Assert.IsTrue( at < ValsAt, "vals: {0} >= {1}", at, ValsAt );
			Assert.AreEqual( @byte, Vals[at], "vals[{0}] = 0x{1:X2} ! 0x{2:X2}", at, Vals[at], @byte );
		}//Vcheck
		
		public void Vcheck( int at, int @int )
		{
			Assert.IsTrue( (at + 4) <= ValsAt, "vals: {0} > {1}-4", at, ValsAt );
			Assert.AreEqual( @int, Bits.Int( Vals, at ), "vint({0})", at );
		}//Vcheck
		
		public void Vcheck( int at, float @float )
		{
			Assert.IsTrue( (at + 4) <= ValsAt, "vals: {0} > {1}-4", at, ValsAt );
			Assert.AreEqual( @float, Bits.Float( Vals, at ), "vfloat({0})", at );
		}//Vcheck
		
		public void Vcheck( int at, long @long )
		{
			Assert.IsTrue( (at + 4) <= ValsAt, "vals: {0} > {1}-4", at, ValsAt );
			Assert.AreEqual( @long, Bits.Long( Vals, at ), "vlong({0})", at );
		}//Vcheck
		
		public void Vcheck( int at, double @double )
		{
			Assert.IsTrue( (at + 4) <= ValsAt, "vals: {0} > {1}-4", at, ValsAt );
			Assert.AreEqual( @double, Bits.Double( Vals, at ), "vdouble({0})", at );
		}//Vcheck
		
		public void Vtopmk( int at, int @int )
		{
			Assert.IsTrue( at < ValsAt, "vals: {0} >= {1}", at, ValsAt );
			Assert.AreEqual( @int, TopInt( at ), "vtop {0}", at );
		}//Vtopmk
		
		public void Vtopmk( int @int )
		{
			Assert.AreEqual( @int, TopInt(), "vtop" );
		}//Vtopmk
		
		public void Vfinal( int @int )
		{
			Assert.AreEqual( @int, ValsAt, "valsAt" );
			Vtopmk( 0 );
		}//Vfinal
	}//BeeCodeTestsBase
	
	[TestClass]
	public class BeeExpressionCodeTests: BeeCodeTestsBase
	{
		public void Test( string @string )
		{
			try
			{
				Parser.Line = @string;
				Parser.Expression();
			}
			catch( Exception e )
			{
				throw new Exception( String.Format( "{0}: {1}; IN: <{2}>", e.GetType().ToString(), e.Message, @string ), e );
			}
		}//Test
		
		[TestMethod]
		public void CX01_add(  )
		{
			Test( "x + 1" );
			Vcheck( 0, unchecked((byte)'x') );
			Vcheck( 1, unchecked((byte)Opcode.Ident) );
			Vtopmk( 6, 0 );
			Vcheck( 6, 1 );
			Vcheck( 10, unchecked((byte)Opcode.Int) );
			Vtopmk( 15, 6 );
			Vcheck( 15, unchecked((byte)Opcode.Add) );
			Vfinal( 20 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Add) );
			Check( 1, unchecked((byte)Opcode.Ident) );
			Check( 2, unchecked((byte)1) );
			Check( 3, unchecked((byte)'x') );
			Check( 4, unchecked((byte)Opcode.Int) );
			Check( 5, 1 );
			Check( 9 );
		}//CX01_add
		
		[TestMethod]
		public void CX02_addmul(  )
		{
			Test( "1u + x * 3f" );
			Vcheck( 0, 1 );
			Vcheck( 4, unchecked((byte)Opcode.Uint) );
			Vtopmk( 9, 0 );
			Vcheck( 9, unchecked((byte)'x') );
			Vcheck( 10, unchecked((byte)Opcode.Ident) );
			Vtopmk( 15, 9 );
			Vcheck( 15, 3f );
			Vcheck( 19, unchecked((byte)Opcode.Float) );
			Vtopmk( 24, 15 );
			Vcheck( 24, unchecked((byte)Opcode.Mul) );
			Vtopmk( 29, 9 );
			Vcheck( 29, unchecked((byte)Opcode.Add) );
			Vfinal( 34 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Add) );
			Check( 1, unchecked((byte)Opcode.Uint) );
			Check( 2, 1 );
			Check( 6, unchecked((byte)Opcode.Mul) );
			Check( 7, unchecked((byte)Opcode.Ident) );
			Check( 8, unchecked((byte)1) );
			Check( 9, unchecked((byte)'x') );
			Check( 10, unchecked((byte)Opcode.Float) );
			Check( 11, 3f );
			Check( 15 );
		}//CX02_addmul
		
		[TestMethod]
		public void CX03_paren(  )
		{
			Test( "(1L + x) * 3.0" );
			Vcheck( 0, 1L );
			Vcheck( 8, unchecked((byte)Opcode.Long) );
			Vtopmk( 13, 0 );
			Vcheck( 13, unchecked((byte)'x') );
			Vcheck( 14, unchecked((byte)Opcode.Ident) );
			Vtopmk( 19, 13 );
			Vcheck( 19, unchecked((byte)Opcode.Add) );
			Vtopmk( 24, 0 );
			Vcheck( 24, 3.0 );
			Vcheck( 32, unchecked((byte)Opcode.Double) );
			Vtopmk( 37, 24 );
			Vcheck( 37, unchecked((byte)Opcode.Mul) );
			Vfinal( 42 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Mul) );
			Check( 1, unchecked((byte)Opcode.Add) );
			Check( 2, unchecked((byte)Opcode.Long) );
			Check( 3, 1L );
			Check( 11, unchecked((byte)Opcode.Ident) );
			Check( 12, unchecked((byte)1) );
			Check( 13, unchecked((byte)'x') );
			Check( 14, unchecked((byte)Opcode.Double) );
			Check( 15, 3.0 );
			Check( 23 );
		}//CX03_paren
		
		[TestMethod]
		public void CX04_ternary(  )
		{
			Test( "cond ? true : false" );
			Vcheck( 0, unchecked((byte)'c') );
			Vcheck( 3, unchecked((byte)'d') );
			Vcheck( 4, unchecked((byte)Opcode.Ident) );
			Vtopmk( 9, 0 );
			Vcheck( 9, unchecked((byte)Opcode.True) );
			Vtopmk( 14, 9 );
			Vcheck( 14, unchecked((byte)Opcode.False) );
			Vtopmk( 19, 14 );
			Vcheck( 19, unchecked((byte)Opcode.Ternary) );
			Vfinal( 24 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Ternary) );
			Check( 1, unchecked((byte)Opcode.Ident) );
			Check( 2, unchecked((byte)4) );
			Check( 3, unchecked((byte)'c') );
			Check( 6, unchecked((byte)'d') );
			Check( 7, 1 );
			Check( 11, unchecked((byte)Opcode.True) );
			Check( 12, 1 );
			Check( 16, unchecked((byte)Opcode.False) );
			Check( 17 );
		}//CX04_ternary
		
		[TestMethod]
		public void CX05_uncall_v1(  )
		{
			Test( "abs(-1)" );
			CX05_uncall();
		}//CX05_uncall_v1
		
		[TestMethod]
		public void CX05_uncall_v2(  )
		{
			Test( "abs -1" );
			CX05_uncall();
		}//CX05_uncall_v2
		
		public void CX05_uncall(  )
		{
			Vcheck( 0, unchecked((byte)'a') );
			Vcheck( 3, unchecked((byte)Opcode.Ident) );
			Vtopmk( 8, 0 );
			Vcheck( 8, -1 );
			Vcheck( 12, unchecked((byte)Opcode.Int) );
			Vtopmk( 17, 8 );
			Vcheck( 17, unchecked((byte)Opcode.Call) );
			Vfinal( 22 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Call) );
			Check( 1, unchecked((byte)Opcode.Ident) );
			Check( 2, unchecked((byte)3) );
			Check( 3, unchecked((byte)'a') );
			Check( 6, unchecked((byte)Opcode.Int) );
			Check( 7, -1 );
			Check( 11 );
		}//CX05_uncall
		
		[TestMethod]
		public void CX06_call2_v1(  )
		{
			Test( "fn(x,y)" );
			CX06_call2();
		}//CX06_call2_v1
		
		[TestMethod]
		public void CX06_call2_v2(  )
		{
			Test( "fn x,y" );
			CX06_call2();
		}//CX06_call2_v2
		
		public void CX06_call2(  )
		{
			Vcheck( 0, unchecked((byte)'f') );
			Vcheck( 1, unchecked((byte)'n') );
			Vcheck( 2, unchecked((byte)Opcode.Ident) );
			Vtopmk( 7, 0 );
			Vcheck( 7, unchecked((byte)'x') );
			Vcheck( 8, unchecked((byte)Opcode.Ident) );
			Vtopmk( 13, 7 );
			Vcheck( 13, unchecked((byte)'y') );
			Vcheck( 14, unchecked((byte)Opcode.Ident) );
			Vtopmk( 19, 13 );
			Vcheck( 19, unchecked((byte)Opcode.Call2) );
			Vfinal( 24 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Call2) );
			Check( 1, unchecked((byte)Opcode.Ident) );
			Check( 2, unchecked((byte)2) );
			Check( 3, unchecked((byte)'f') );
			Check( 4, unchecked((byte)'n') );
			Check( 5, unchecked((byte)Opcode.Ident) );
			Check( 6, unchecked((byte)1) );
			Check( 7, unchecked((byte)'x') );
			Check( 8, unchecked((byte)Opcode.Ident) );
			Check( 9, unchecked((byte)1) );
			Check( 10, unchecked((byte)'y') );
			Check( 11 );
		}//CX06_call2
		
		[TestMethod]
		public void CX07_mcall_v1(  )
		{
			Test( "fn(null, this, base.field)" );
			CX07_mcall();
		}//CX07_mcall_v1
		
		[TestMethod]
		public void CX07_mcall_v2(  )
		{
			Test( "fn null, this, base.field" );
			CX07_mcall();
		}//CX07_mcall_v2
		
		public void CX07_mcall(  )
		{
			Vcheck( 0, unchecked((byte)'f') );
			Vcheck( 1, unchecked((byte)'n') );
			Vcheck( 2, unchecked((byte)Opcode.Ident) );
			Vtopmk( 7, 0 );
			Vcheck( 7, unchecked((byte)Opcode.Null) );
			Vtopmk( 12, 7 );
			Vcheck( 12, unchecked((byte)Opcode.This) );
			Vtopmk( 17, 12 );
			Vcheck( 17, unchecked((byte)Opcode.Base) );
			Vtopmk( 22, 17 );
			Vcheck( 22, unchecked((byte)'f') );
			Vcheck( 26, unchecked((byte)'d') );
			Vcheck( 27, unchecked((byte)Opcode.Ident) );
			Vtopmk( 32, 22 );
			Vcheck( 32, unchecked((byte)Opcode.Dot) );
			Vtopmk( 37, 17 );
			Vcheck( 37, unchecked((byte)4) );
			Vcheck( 38, unchecked((byte)Opcode.Mcall) );
			Vfinal( 43 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Mcall) );
			Check( 1, unchecked((byte)4) );
			Check( 2, unchecked((byte)Opcode.Ident) );
			Check( 3, unchecked((byte)2) );
			Check( 4, unchecked((byte)'f') );
			Check( 5, unchecked((byte)'n') );
			Check( 6, unchecked((byte)Opcode.Null) );
			Check( 7, unchecked((byte)Opcode.This) );
			Check( 8, unchecked((byte)Opcode.Dot) );
			Check( 9, unchecked((byte)Opcode.Base) );
			Check( 10, unchecked((byte)5) );
			Check( 11, unchecked((byte)'f') );
			Check( 15, unchecked((byte)'d') );
			Check( 16 );
		}//CX07_mcall
		
		[TestMethod]
		public void CX08_calls(  )
		{
			Test( "f g(x), h y" );
			Vcheck( 0, unchecked((byte)'f') );
			Vcheck( 1, unchecked((byte)Opcode.Ident) );
			Vtopmk( 6, 0 );
			Vcheck( 6, unchecked((byte)'g') );
			Vcheck( 7, unchecked((byte)Opcode.Ident) );
			Vtopmk( 12, 6 );
			Vcheck( 12, unchecked((byte)'x') );
			Vcheck( 13, unchecked((byte)Opcode.Ident) );
			Vtopmk( 18, 12 );
			Vcheck( 18, unchecked((byte)Opcode.Call) );
			Vtopmk( 23, 6 );
			Vcheck( 23, unchecked((byte)'h') );
			Vcheck( 24, unchecked((byte)Opcode.Ident) );
			Vtopmk( 29, 23 );
			Vcheck( 29, unchecked((byte)'y') );
			Vcheck( 30, unchecked((byte)Opcode.Ident) );
			Vtopmk( 35, 29 );
			Vcheck( 35, unchecked((byte)Opcode.Call) );
			Vtopmk( 40, 23 );
			Vcheck( 40, unchecked((byte)Opcode.Call2) );
			Vfinal( 45 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Call2) );
			Check( 1, unchecked((byte)Opcode.Ident) );
			Check( 2, unchecked((byte)1) );
			Check( 3, unchecked((byte)'f') );
			Check( 4, unchecked((byte)Opcode.Call) );
			Check( 5, unchecked((byte)Opcode.Ident) );
			Check( 6, unchecked((byte)1) );
			Check( 7, unchecked((byte)'g') );
			Check( 8, unchecked((byte)Opcode.Ident) );
			Check( 9, unchecked((byte)1) );
			Check( 10, unchecked((byte)'x') );
			Check( 11, unchecked((byte)Opcode.Call) );
			Check( 12, unchecked((byte)Opcode.Ident) );
			Check( 13, unchecked((byte)1) );
			Check( 14, unchecked((byte)'h') );
			Check( 15, unchecked((byte)Opcode.Ident) );
			Check( 16, unchecked((byte)1) );
			Check( 17, unchecked((byte)'y') );
			Check( 18 );
		}//CX08_calls
		
		[TestMethod]
		public void CX09_calls2(  )
		{
			Test( "f (g x, y), z, h()" );
			Vcheck( 0, unchecked((byte)'f') );
			Vcheck( 1, unchecked((byte)Opcode.Ident) );
			Vtopmk( 6, 0 );
			Vcheck( 6, unchecked((byte)'g') );
			Vcheck( 7, unchecked((byte)Opcode.Ident) );
			Vtopmk( 12, 6 );
			Vcheck( 12, unchecked((byte)'x') );
			Vcheck( 13, unchecked((byte)Opcode.Ident) );
			Vtopmk( 18, 12 );
			Vcheck( 18, unchecked((byte)'y') );
			Vcheck( 19, unchecked((byte)Opcode.Ident) );
			Vtopmk( 24, 18 );
			Vcheck( 24, unchecked((byte)Opcode.Call2) );
			Vtopmk( 29, 6 );
			Vcheck( 29, unchecked((byte)'z') );
			Vcheck( 30, unchecked((byte)Opcode.Ident) );
			Vtopmk( 35, 29 );
			Vcheck( 35, unchecked((byte)'h') );
			Vcheck( 36, unchecked((byte)Opcode.Ident) );
			Vcheck( 37, unchecked((byte)Opcode.Ecall) );
			Vtopmk( 42, 35 );
			Vcheck( 42, unchecked((byte)4) );
			Vcheck( 43, unchecked((byte)Opcode.Mcall) );
			Vfinal( 48 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Mcall) );
			Check( 1, unchecked((byte)4) );
			Check( 2, unchecked((byte)Opcode.Ident) );
			Check( 3, unchecked((byte)1) );
			Check( 4, unchecked((byte)'f') );
			Check( 5, unchecked((byte)Opcode.Call2) );
			Check( 6, unchecked((byte)Opcode.Ident) );
			Check( 7, unchecked((byte)1) );
			Check( 8, unchecked((byte)'g') );
			Check( 9, unchecked((byte)Opcode.Ident) );
			Check( 10, unchecked((byte)1) );
			Check( 11, unchecked((byte)'x') );
			Check( 12, unchecked((byte)Opcode.Ident) );
			Check( 13, unchecked((byte)1) );
			Check( 14, unchecked((byte)'y') );
			Check( 15, unchecked((byte)Opcode.Ident) );
			Check( 16, unchecked((byte)1) );
			Check( 17, unchecked((byte)'z') );
			Check( 18, unchecked((byte)Opcode.Ecall) );
			Check( 19, unchecked((byte)Opcode.Ident) );
			Check( 20, unchecked((byte)1) );
			Check( 21, unchecked((byte)'h') );
			Check( 22 );
		}//CX09_calls2
		
		[TestMethod]
		public void CX10_prepost(  )
		{
			Test( "++x--" );
			Vcheck( 0, unchecked((byte)'x') );
			Vcheck( 1, unchecked((byte)Opcode.Ident) );
			Vcheck( 2, unchecked((byte)Opcode.Inc) );
			Vcheck( 3, unchecked((byte)Opcode.PostDec) );
			Vfinal( 8 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.PostDec) );
			Check( 1, unchecked((byte)Opcode.Inc) );
			Check( 2, unchecked((byte)Opcode.Ident) );
			Check( 5 );
		}//CX10_prepost
		
		[TestMethod]
		public void CX11_strchar(  )
		{
			Test( "\"string\" + 'c'" );
			Vcheck( 0, unchecked((byte)'s') );
			Vcheck( 5, unchecked((byte)'g') );
			Vcheck( 6, unchecked((byte)Opcode.String) );
			Vtopmk( 11, 0 );
			Vcheck( 11, unchecked((byte)'c') );
			Vcheck( 12, unchecked((byte)Opcode.Char) );
			Vtopmk( 17, 11 );
			Vcheck( 17, unchecked((byte)Opcode.Add) );
			Vfinal( 22 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Add) );
			Check( 1, unchecked((byte)Opcode.String) );
			Check( 2, unchecked((byte)6) );
			Check( 9, unchecked((byte)Opcode.Char) );
			Check( 10, unchecked((byte)'c') );
			Check( 11 );
		}//CX11_strchar
		
		[TestMethod]
		public void CX12_unarg(  )
		{
			Test( "f -x" );
			Assert.AreEqual( unchecked((byte)'f'), Vals[0] );
			Assert.AreEqual( unchecked((byte)Opcode.Ident), Vals[1] );
			Assert.AreEqual( 0, TopInt( 6 ) );
			Assert.AreEqual( unchecked((byte)'x'), Vals[6] );
			Assert.AreEqual( unchecked((byte)Opcode.Ident), Vals[7] );
			Assert.AreEqual( unchecked((byte)Opcode.Neg), Vals[8] );
			Assert.AreEqual( 6, TopInt( 13 ) );
			Assert.AreEqual( 0, TopInt() );
			Assert.AreEqual( 18, ValsAt );
			Rewrite( ValsAt );
			Assert.AreEqual( unchecked((byte)Opcode.Call), Code[0] );
			Assert.AreEqual( unchecked((byte)Opcode.Ident), Code[1] );
			Assert.AreEqual( unchecked((byte)1), Code[2] );
			Assert.AreEqual( unchecked((byte)'f'), Code[3] );
			Assert.AreEqual( unchecked((byte)Opcode.Neg), Code[4] );
			Assert.AreEqual( unchecked((byte)Opcode.Ident), Code[5] );
			Assert.AreEqual( unchecked((byte)1), Code[6] );
			Assert.AreEqual( unchecked((byte)'x'), Code[7] );
			Assert.AreEqual( 8, CodeAt );
		}//CX12_unarg
		
		[TestMethod]
		public void CX13_var1(  )
		{
			Test( "var x" );
			Assert.AreEqual( unchecked((byte)'x'), Vals[0] );
			Assert.AreEqual( unchecked((byte)Opcode.Ident), Vals[1] );
			Assert.AreEqual( 0, TopInt( 6 ) );
			Assert.AreEqual( unchecked((byte)Opcode.Undef), Vals[6] );
			Assert.AreEqual( 6, TopInt( 11 ) );
			Assert.AreEqual( unchecked((byte)Opcode.Undef), Vals[11] );
			Assert.AreEqual( 11, TopInt( 16 ) );
			Assert.AreEqual( unchecked((byte)Opcode.Var), Vals[16] );
			Assert.AreEqual( 0, TopInt() );
			Rewrite( ValsAt );
			Assert.AreEqual( unchecked((byte)Opcode.Var), Code[0] );
			Assert.AreEqual( unchecked((byte)1), Code[1] );
			Assert.AreEqual( unchecked((byte)'x'), Code[2] );
			Assert.AreEqual( unchecked((byte)Opcode.Undef), Code[3] );
			Assert.AreEqual( unchecked((byte)Opcode.Undef), Code[4] );
			Assert.AreEqual( 5, CodeAt );
		}//CX13_var1
		
		[TestMethod]
		public void CX14_var2(  )
		{
			Test( "var x int" );
			Vcheck( 0, unchecked((byte)'x') );
			Vcheck( 1, unchecked((byte)Opcode.Ident) );
			Vtopmk( 6, 0 );
			Vcheck( 6, unchecked((byte)Opcode.Int) );
			Vtopmk( 11, 6 );
			Vcheck( 11, unchecked((byte)Opcode.Undef) );
			Vtopmk( 16, 11 );
			Vcheck( 16, unchecked((byte)Opcode.Var) );
			Vfinal( 21 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Var) );
			Check( 1, unchecked((byte)1) );
			Check( 2, unchecked((byte)'x') );
			Check( 3, unchecked((byte)Opcode.Int) );
			Check( 4, unchecked((byte)Opcode.Undef) );
			Check( 5 );
		}//CX14_var2
		
		[TestMethod]
		public void CX15_array(  )
		{
			Test( "var a byte[]" );
			Vcheck( 0, unchecked((byte)'a') );
			Vcheck( 1, unchecked((byte)Opcode.Ident) );
			Vtopmk( 6, 0 );
			Vcheck( 6, unchecked((byte)Opcode.Byte) );
			Vtopmk( 11, 6 );
			Vcheck( 11, unchecked((byte)1) );
			Vcheck( 12, unchecked((byte)Opcode.Array) );
			Vtopmk( 17, 6 );
			Vcheck( 17, unchecked((byte)Opcode.Undef) );
			Vtopmk( 22, 17 );
			Vcheck( 22, unchecked((byte)Opcode.Var) );
			Vfinal( 27 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Var) );
			Check( 1, unchecked((byte)1) );
			Check( 2, unchecked((byte)'a') );
			Check( 3, unchecked((byte)Opcode.Array) );
			Check( 4, unchecked((byte)1) );
			Check( 5, unchecked((byte)Opcode.Byte) );
			Check( 6, unchecked((byte)Opcode.Undef) );
			Check( 7 );
		}//CX15_array
		
		[TestMethod]
		public void CX16_array_create(  )
		{
			Test( "var a = new byte[n]" );
			Vcheck( 0, unchecked((byte)'a') );
			Vcheck( 1, unchecked((byte)Opcode.Ident) );
			Vtopmk( 6, 0 );
			Vcheck( 6, unchecked((byte)Opcode.Undef) );
			Vtopmk( 11, 6 );
			Vcheck( 11, unchecked((byte)Opcode.Byte) );
			Vtopmk( 16, 11 );
			Vcheck( 16, unchecked((byte)'n') );
			Vcheck( 17, unchecked((byte)Opcode.Ident) );
			Vtopmk( 22, 16 );
			Vcheck( 22, unchecked((byte)2) );
			Vcheck( 23, unchecked((byte)Opcode.Array) );
			Vcheck( 24, unchecked((byte)Opcode.Create) );
			Vtopmk( 29, 11 );
			Vcheck( 29, unchecked((byte)Opcode.Var) );
			Vfinal( 34 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Var) );
			Check( 1, unchecked((byte)1) );
			Check( 2, unchecked((byte)'a') );
			Check( 3, unchecked((byte)Opcode.Undef) );
			Check( 4, unchecked((byte)Opcode.Create) );
			Check( 5, unchecked((byte)Opcode.Array) );
			Check( 6, unchecked((byte)2) );
			Check( 7, unchecked((byte)Opcode.Byte) );
			Check( 8, unchecked((byte)Opcode.Ident) );
			Check( 9, unchecked((byte)1) );
			Check( 10, unchecked((byte)'n') );
			Check( 11 );
		}//CX16_array_create
		
		[TestMethod]
		public void CX17_generic(  )
		{
			Test( "var a list.[byte]" );
			Vcheck( 0, unchecked((byte)'a') );
			Vcheck( 1, unchecked((byte)Opcode.Ident) );
			Vtopmk( 6, 0 );
			Vcheck( 6, unchecked((byte)'l') );
			Vcheck( 10, unchecked((byte)Opcode.Ident) );
			Vtopmk( 15, 6 );
			Vcheck( 15, unchecked((byte)Opcode.Byte) );
			Vtopmk( 20, 15 );
			Vcheck( 20, unchecked((byte)2) );
			Vcheck( 21, unchecked((byte)Opcode.Generic) );
			Vtopmk( 26, 6 );
			Vcheck( 26, unchecked((byte)Opcode.Undef) );
			Vtopmk( 31, 26 );
			Vcheck( 31, unchecked((byte)Opcode.Var) );
			Vfinal( 36 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Var) );
			Check( 1, unchecked((byte)1) );
			Check( 2, unchecked((byte)'a') );
			Check( 3, unchecked((byte)Opcode.Generic) );
			Check( 4, unchecked((byte)2) );
			Check( 5, unchecked((byte)Opcode.Ident) );
			Check( 6, unchecked((byte)4) );
			Check( 7, unchecked((byte)'l') );
			Check( 11, unchecked((byte)Opcode.Byte) );
			Check( 12, unchecked((byte)Opcode.Undef) );
			Check( 13 );
		}//CX17_generic
		
		[TestMethod]
		public void CX18_generic_create(  )
		{
			Test( "var a = new list.[byte]" );
			Vcheck( 0, unchecked((byte)'a') );
			Vcheck( 1, unchecked((byte)Opcode.Ident) );
			Vtopmk( 6, 0 );
			Vcheck( 6, unchecked((byte)Opcode.Undef) );
			Vtopmk( 11, 6 );
			Vcheck( 11, unchecked((byte)'l') );
			Vcheck( 15, unchecked((byte)Opcode.Ident) );
			Vtopmk( 20, 11 );
			Vcheck( 20, unchecked((byte)Opcode.Byte) );
			Vtopmk( 25, 20 );
			Vcheck( 25, unchecked((byte)2) );
			Vcheck( 26, unchecked((byte)Opcode.Generic) );
			Vcheck( 27, unchecked((byte)Opcode.Create) );
			Vtopmk( 32, 11 );
			Vcheck( 32, unchecked((byte)Opcode.Var) );
			Vfinal( 37 );
			Rewrite( ValsAt );
			Check( 0, unchecked((byte)Opcode.Var) );
			Check( 1, unchecked((byte)1) );
			Check( 2, unchecked((byte)'a') );
			Check( 3, unchecked((byte)Opcode.Undef) );
			Check( 4, unchecked((byte)Opcode.Create) );
			Check( 5, unchecked((byte)Opcode.Generic) );
			Check( 6, unchecked((byte)2) );
			Check( 7, unchecked((byte)Opcode.Ident) );
			Check( 8, unchecked((byte)4) );
			Check( 9, unchecked((byte)'l') );
			Check( 13, unchecked((byte)Opcode.Byte) );
			Check( 14 );
		}//CX18_generic_create
	}//BeeExpressionCodeTests
}//Bee.Tests
