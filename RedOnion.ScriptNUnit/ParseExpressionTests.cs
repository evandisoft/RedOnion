using System;
using System.Linq;
using NUnit.Framework;
using RedOnion.Script.Parsing;
using RedOnion.Script.Execution;

//	x + 1
//	1u + x * 3f
//	(1L + x) * 3.0
//	cond ? true : false
//	abs(-1)
//	abs -1
//	fn(x,y)
//	fn x,y
//	fn(null, this, base.field)
//	fn null, this, base.field
//	f g(x), h x
//	f (g x, y), z, h()
//	++x--
//	"string" + 'c'
//	var x
//	var x int
//	var a:byte[]
//	var a = new byte[n]
//	var a as list.[byte]

namespace RedOnion.ScriptTests
{
	public class ParseTestsBase : Parser
	{
		public int CodeInt(int at)
			=> BitConverter.ToInt32(Code, at);
		public float CodeFloat(int at)
			=> BitConverter.ToSingle(Code, at);
		public long CodeLong(int at)
			=> BitConverter.ToInt64(Code, at);
		public double CodeDouble(int at)
			=> BitConverter.ToDouble(Code, at);

		public int ValueInt(int at)
			=> BitConverter.ToInt32(Values, at);
		public float ValueFloat(int at)
			=> BitConverter.ToSingle(Values, at);
		public long ValueLong(int at)
			=> BitConverter.ToInt64(Values, at);
		public double ValueDouble(int at)
			=> BitConverter.ToDouble(Values, at);

		public void CodeCheck(int at)
			=> Assert.AreEqual(at, CodeAt, "CodeAt");
		public void CodeCheck(int at, OpCode value)
			=> CodeCheck(at, (byte)value);

		public void CodeCheck(int at, byte value)
		{
			Assert.IsTrue(at <= CodeAt-1, "Code: {0} > {1}", at, CodeAt-1);
			Assert.AreEqual(value, Code[at], "Code[{0}] = 0x{1:X2} ! 0x{2:X2}", at, Code[at], value);
		}
		public void CodeCheck(int at, int value)
		{
			Assert.IsTrue(at <= CodeAt-4, "Code: {0} > {1}", at, CodeAt-4);
			Assert.AreEqual(value, CodeInt(at), "CodeInt({0})", at);
		}
		public void CodeCheck(int at, float value)
		{
			Assert.IsTrue(at <= CodeAt-4, "Code: {0} > {1}", at, CodeAt-4);
			Assert.AreEqual(value, CodeFloat(at), "CodeFloat({0})", at);
		}
		public void CodeCheck(int at, long value)
		{
			Assert.IsTrue(at <= CodeAt-8, "Code: {0} > {1}", at, CodeAt-8);
			Assert.AreEqual(value, CodeLong(at), "CodeLong({0})", at);
		}
		public void CodeCheck(int at, double value)
		{
			Assert.IsTrue(at <= CodeAt-8, "Code: {0} > {1}", at, CodeAt-8);
			Assert.AreEqual(value, CodeDouble(at), "CodeDouble({0})", at);
		}
		public void CodeCheck(int at, int index, string value)
		{
			Assert.IsTrue(at <= CodeAt-4, "Code: {0} > {1}", at, CodeAt-4);
			Assert.AreEqual(index, CodeInt(at), "CodeInt({0})", at);
			if (index == -1)
				Assert.IsTrue(value.Length == 0, "Empty string");
			else
			{
				Assert.IsTrue(index < StringsAt, "Strings: {0} >= {1}", index, StringsAt);
				Assert.AreEqual(value, Strings[index], "Strings[{0}]", index);
			}
		}

		public void ValueCheck(int at, byte value)
		{
			Assert.IsTrue(at <= ValuesAt-1, "Values: {0} > {1}", at, ValuesAt-1);
			Assert.AreEqual(value, Values[at], "Values[{0}] = 0x{1:X2} ! 0x{2:X2}", at, Values[at], value);
		}
		public void ValueCheck(int at, int value)
		{
			Assert.IsTrue(at <= ValuesAt-4, "Values: {0} > {1}", at, ValuesAt-4);
			Assert.AreEqual(value, ValueInt(at), "ValueInt({0})", at);
		}
		public void ValueCheck(int at, float value)
		{
			Assert.IsTrue(at <= ValuesAt-4, "Values: {0} > {1}", at, ValuesAt-4);
			Assert.AreEqual(value, ValueFloat(at), "ValueFloat({0})", at);
		}
		public void ValueCheck(int at, long value)
		{
			Assert.IsTrue(at <= ValuesAt-8, "Values: {0} > {1}", at, ValuesAt-8);
			Assert.AreEqual(value, ValueLong(at), "ValueLong({0})", at);
		}
		public void ValueCheck(int at, double value)
		{
			Assert.IsTrue(at <= ValuesAt-8, "Values: {0} > {1}", at, ValuesAt-8);
			Assert.AreEqual(value, ValueDouble(at), "ValueDouble({0})", at);
		}

		public void ValueTopMark(int at, int value)
		{
			Assert.IsTrue(at < ValuesAt, "Values: {0} >= {1}", at, ValuesAt);
			Assert.AreEqual(value, TopInt(at), "ValueTopMark {0}", at);
		}
		public void ValueTopMark(int value)
		{
			Assert.AreEqual(value, TopInt(), "ValueTopMark");
		}
		public void ValueFinal(int value)
		{
			Assert.AreEqual(value, ValuesAt, "ValuesAt");
			ValueTopMark(0);
		}

		public void ValueCheck(int at, int index, string value)
		{
			Assert.IsTrue(at <= ValuesAt-4, "Values: {0} > {1}", at, ValuesAt-4);
			Assert.AreEqual(index, ValueInt(at), "ValueInt({0})", at);
			if (index == -1)
				Assert.IsTrue(value.Length == 0, "Empty string");
			else
			{
				Assert.IsTrue(index < StringValuesAt, "StringValues: {0} >= {1}", index, StringValuesAt);
				Assert.AreEqual(value, StringValues[index], "StringValues[{0}]", index);
			}
		}
		public void ValueCheck(int at, OpCode value)
			=> ValueCheck(at, (byte)value);
	}

	[TestFixture]
	public class ParseExpressionTests : ParseTestsBase
	{
		public void Test(string line)
		{
			try
			{
				Reset();
				lexer.Line = line;
				ParseExpression();
			}
			catch (Exception e)
			{
				throw new Exception(String.Format("{0}: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, line), e);
			}
		}

		[Test]
		public void ParseExpression_01_Add()
		{
			Test("x + 1");

			ValueCheck	( 0,  0, "x");              // ValueInt( 0) = 0; StringValues[0] = "x"
			ValueCheck	( 4,  OpCode.Identifier);   // Values  [ 4] = (byte)OpCode.Identifier
			ValueTopMark( 9,  0);					// ValueInt( 5) = 0 which is start of "identifier x"
			ValueCheck	( 9,  1);					// ValueInt( 9) = 1 which is the constant in the expression
			ValueCheck	(13,  OpCode.Int);          // Values  [13] = (byte)OpCode.Int = 32bit signed integer
			ValueTopMark(18,  9);					// ValueInt(14) = 9 which is start of "integer 1"
			ValueCheck	(18,  OpCode.Add);			// Values  [18] = (byte)OpCode.Add = the operator
			ValueFinal	(23);						// ValueInt(19) = 0 which is start of it all, ValuesAt = 23

			Rewrite(ValuesAt);						// rewrites backwards with some reduction/compression

			CodeCheck( 0, OpCode.Add);
			CodeCheck( 1, OpCode.Identifier);
			CodeCheck( 2, 0, "x");
			CodeCheck( 6, OpCode.Int);
			CodeCheck( 7, 1);
			CodeCheck(11);
		}

		[Test]
		public void ParseExpression_02_AddMul()
		{
			Test("1u + x * 3f");

			ValueCheck	( 0, 1);
			ValueCheck	( 4, OpCode.UInt);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, 0, "x");
			ValueCheck	(13, OpCode.Identifier);
			ValueTopMark(18, 9);
			ValueCheck	(18, 3f);
			ValueCheck	(22, OpCode.Float);
			ValueTopMark(27, 18);
			ValueCheck	(27, OpCode.Mul);
			ValueTopMark(32, 9);
			ValueCheck	(32, OpCode.Add);
			ValueFinal	(37);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Add);
			CodeCheck( 1, OpCode.UInt);
			CodeCheck( 2, 1);
			CodeCheck( 6, OpCode.Mul);
			CodeCheck( 7, OpCode.Identifier);
			CodeCheck( 8, 0, "x");
			CodeCheck(12, OpCode.Float);
			CodeCheck(13, 3f);
			CodeCheck(17);
		}

		[Test]
		public void ParseExpression_03_Parenthesis()
		{
			Test("(1L + x) * 3.0");

			ValueCheck	( 0, 1L);
			ValueCheck	( 8, OpCode.Long);
			ValueTopMark(13, 0);
			ValueCheck	(13, 0, "x");
			ValueCheck	(17, OpCode.Identifier);
			ValueTopMark(22, 13);
			ValueCheck	(22, OpCode.Add);
			ValueTopMark(27, 0);
			ValueCheck	(27, 3.0);
			ValueCheck	(35, OpCode.Double);
			ValueTopMark(40, 27);
			ValueCheck	(40, OpCode.Mul);
			ValueFinal	(45);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Mul);
			CodeCheck( 1, OpCode.Add);
			CodeCheck( 2, OpCode.Long);
			CodeCheck( 3, 1L);
			CodeCheck(11, OpCode.Identifier);
			CodeCheck(12, 0, "x");
			CodeCheck(16, OpCode.Double);
			CodeCheck(17, 3.0);
			CodeCheck(25);
		}

		[Test]
		public void ParseExpression_04_Ternary()
		{
			Test("cond ? true : false");

			ValueCheck	( 0, 0, "cond");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, OpCode.True);
			ValueTopMark(14, 9);
			ValueCheck	(14, OpCode.False);
			ValueTopMark(19, 14);
			ValueCheck	(19, OpCode.Ternary);
			ValueFinal	(24);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Ternary);
			CodeCheck( 1, OpCode.Identifier);
			CodeCheck( 2, 0, "cond");
			CodeCheck( 6, 1); // size of true-expression for quick skip
			CodeCheck(10, OpCode.True);
			CodeCheck(11, 1); // size of false-expression for quick skip
			CodeCheck(15, OpCode.False);
			CodeCheck(16);
		}

		[Test]
		public void ParseExpression_05_CallWithUnary_v1()
		{
			Test("abs(-1)");
			ParseExpression_05_CallWithUnary();
		}

		[Test]
		public void ParseExpression_05_CallWithUnary_v2()
		{
			Test("abs -1");
			ParseExpression_05_CallWithUnary();
		}

		public void ParseExpression_05_CallWithUnary()
		{
			ValueCheck	( 0, 0, "abs");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, -1);
			ValueCheck	(13, OpCode.Int);
			ValueTopMark(18, 9);
			ValueCheck	(18, OpCode.Call1);
			ValueFinal	(23);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Call1);
			CodeCheck( 1, OpCode.Identifier);
			CodeCheck( 2, 0, "abs");
			CodeCheck( 6, OpCode.Int);
			CodeCheck( 7, -1);
			CodeCheck(11);
		}

		[Test]
		public void ParseExpression_06_CallTwoArgs_v1()
		{
			Test("fn(x,y)");
			ParseExpression_06_CallTwoArgs();
		}

		[Test]
		public void ParseExpression_06_CallTwoArgs_v2()
		{
			Test("fn x,y");
			ParseExpression_06_CallTwoArgs();
		}

		public void ParseExpression_06_CallTwoArgs()
		{
			ValueCheck	( 0, 0, "fn");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, 1, "x");
			ValueCheck	(13, OpCode.Identifier);
			ValueTopMark(18, 9);
			ValueCheck	(18, 2, "y");
			ValueCheck	(22, OpCode.Identifier);
			ValueTopMark(27, 18);
			ValueCheck	(27, OpCode.Call2);
			ValueFinal	(32);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Call2);
			CodeCheck( 1, OpCode.Identifier);
			CodeCheck( 2, 0, "fn");
			CodeCheck( 6, OpCode.Identifier);
			CodeCheck( 7, 1, "x");
			CodeCheck(11, OpCode.Identifier);
			CodeCheck(12, 2, "y");
			CodeCheck(16);
		}

		[Test]
		public void ParseExpression_07_CallManyArgs_v1()
		{
			Test("fn(null, this, base.field)");
			ParseExpression_07_CallManyArgs();
		}

		[Test]
		public void ParseExpression_07_CallManyArgs_v2()
		{
			Test("fn null, this, base.field");
			ParseExpression_07_CallManyArgs();
		}

		public void ParseExpression_07_CallManyArgs()
		{
			ValueCheck	( 0, 0, "fn");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, OpCode.Null);
			ValueTopMark(14, 9);
			ValueCheck	(14, OpCode.This);
			ValueTopMark(19, 14);
			ValueCheck	(19, OpCode.Base);
			ValueTopMark(24, 19);
			ValueCheck	(24, 1, "field");
			ValueCheck	(28, OpCode.Identifier);
			ValueTopMark(33, 24);
			ValueCheck	(33, OpCode.Dot);
			ValueTopMark(38, 19);		// base.field
			ValueCheck	(38, (byte)4);	// number of arguments
			ValueCheck	(39, OpCode.CallN);
			ValueFinal	(44);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.CallN);
			CodeCheck( 1, (byte)4);
			CodeCheck( 2, OpCode.Identifier);
			CodeCheck( 3, 0, "fn");
			CodeCheck( 7, OpCode.Null);
			CodeCheck( 8, OpCode.This);
			CodeCheck( 9, OpCode.Dot);
			CodeCheck(10, OpCode.Base);
			CodeCheck(11, 1, "field");
			CodeCheck(15);
		}

		[Test]
		public void ParseExpression_08_NestedCalls()
		{
			Test("f g(x), h x");

			ValueCheck	( 0, 0, "f");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, 1, "g");
			ValueCheck	(13, OpCode.Identifier);
			ValueTopMark(18, 9);
			ValueCheck	(18, 2, "x");
			ValueCheck	(22, OpCode.Identifier);
			ValueTopMark(27, 18);
			ValueCheck	(27, OpCode.Call1);
			ValueTopMark(32, 9);		// g(x)
			ValueCheck	(32, 3, "h");
			ValueCheck	(36, OpCode.Identifier);
			ValueTopMark(41, 32);
			ValueCheck	(41, 4, "x");	// value-strings have duplicits
			ValueCheck	(45, OpCode.Identifier);
			ValueTopMark(50, 41);
			ValueCheck	(50, OpCode.Call1);
			ValueTopMark(55, 32);		// h x
			ValueCheck	(55, OpCode.Call2);
			ValueFinal	(60);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Call2);
			CodeCheck( 1, OpCode.Identifier);
			CodeCheck( 2, 0, "f");
			CodeCheck( 6, OpCode.Call1);
			CodeCheck( 7, OpCode.Identifier);
			CodeCheck( 8, 1, "g");
			CodeCheck(12, OpCode.Identifier);
			CodeCheck(13, 2, "x");
			CodeCheck(17, OpCode.Call1);
			CodeCheck(18, OpCode.Identifier);
			CodeCheck(19, 3, "h");
			CodeCheck(23, OpCode.Identifier);
			CodeCheck(24, 2, "x"); // final string table must reuse strings
			CodeCheck(28);
		}

		[Test]
		public void ParseExpression_09_MoreCalls()
		{
			Test("f (g x, y), z, h()");

			ValueCheck	( 0, 0, "f");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, 1, "g");
			ValueCheck	(13, OpCode.Identifier);
			ValueTopMark(18, 9);
			ValueCheck	(18, 2, "x");
			ValueCheck	(22, OpCode.Identifier);
			ValueTopMark(27, 18);
			ValueCheck	(27, 3, "y");
			ValueCheck	(31, OpCode.Identifier);
			ValueTopMark(36, 27);
			ValueCheck	(36, OpCode.Call2);
			ValueTopMark(41, 9); // g x, y
			ValueCheck	(41, 4, "z");
			ValueCheck	(45, OpCode.Identifier);
			ValueTopMark(50, 41);
			ValueCheck	(50, 5, "h");
			ValueCheck	(54, OpCode.Identifier);
			ValueCheck	(55, OpCode.Call0);
			ValueTopMark(60, 50);
			ValueCheck	(60, (byte)4);
			ValueCheck	(61, OpCode.CallN);
			ValueFinal	(66);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.CallN);
			CodeCheck( 1, (byte)4);
			CodeCheck( 2, OpCode.Identifier);
			CodeCheck( 3, 0, "f");
			CodeCheck( 7, OpCode.Call2);
			CodeCheck( 8, OpCode.Identifier);
			CodeCheck( 9, 1, "g");
			CodeCheck(13, OpCode.Identifier);
			CodeCheck(14, 2, "x");
			CodeCheck(18, OpCode.Identifier);
			CodeCheck(19, 3, "y");
			CodeCheck(23, OpCode.Identifier);
			CodeCheck(24, 4, "z");
			CodeCheck(28, OpCode.Call0);
			CodeCheck(29, OpCode.Identifier);
			CodeCheck(30, 5, "h");
			CodeCheck(34);
		}

		[Test]
		public void ParseExpression_10_PreAndPost()
		{
			Test("++x--");

			ValueCheck( 0, 0, "x");
			ValueCheck( 4, OpCode.Identifier);
			ValueCheck( 5, OpCode.Inc);
			ValueCheck( 6, OpCode.PostDec);
			ValueFinal(11);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.PostDec);
			CodeCheck( 1, OpCode.Inc);
			CodeCheck( 2, OpCode.Identifier);
			CodeCheck( 3, 0, "x");
			CodeCheck( 7);
		}

		[Test]
		public void ParseExpression_11_StringAndChar()
		{
			Test("\"string\" + 'c'");

			ValueCheck	( 0, 0, "string");
			ValueCheck	( 4, OpCode.String);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, (byte)'c');
			ValueCheck	(10, OpCode.Char);
			ValueTopMark(15, 9);
			ValueCheck	(15, OpCode.Add);
			ValueFinal	(20);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Add);
			CodeCheck( 1, OpCode.String);
			CodeCheck( 2, 0, "string");
			CodeCheck( 6, OpCode.Char);
			CodeCheck( 7, (byte)'c');
			CodeCheck( 8);
		}

		[Test]
		public void ParseExpression_12_Variable()
		{
			Test("var x");

			ValueCheck	( 0, 0, "x");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, OpCode.Undefined);
			ValueTopMark(14, 9);
			ValueCheck	(14, OpCode.Undefined);
			ValueTopMark(19, 14);
			ValueCheck	(19, OpCode.Var);
			ValueFinal	(24);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Var);
			CodeCheck( 1, 0, "x");
			CodeCheck( 5, OpCode.Undefined);	// type
			CodeCheck( 6, OpCode.Undefined);	// no initializer
			CodeCheck( 7);
		}

		[Test]
		public void ParseExpression_13_TypedVariable()
		{
			Test("var x int"); // pure style

			ValueCheck	( 0, 0, "x");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, OpCode.Int);
			ValueTopMark(14, 9);
			ValueCheck	(14, OpCode.Undefined);
			ValueTopMark(19, 14);
			ValueCheck	(19, OpCode.Var);
			ValueFinal	(24);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Var);
			CodeCheck( 1, 0, "x");
			CodeCheck( 5, OpCode.Int);			// type
			CodeCheck( 6, OpCode.Undefined);	// no initializer
			CodeCheck( 7);
		}

		[Test]
		public void ParseExpression_14_ArrayVariable()
		{
			Test("var a:byte[]"); // ActionScript

			ValueCheck	( 0, 0, "a");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, OpCode.Byte);
			ValueTopMark(14, 9);
			ValueCheck	(14, (byte)1);			// dimensions
			ValueCheck	(15, OpCode.Array);
			ValueTopMark(20, 9);				// byte[]
			ValueCheck	(20, OpCode.Undefined);	// no initializer
			ValueTopMark(25, 20);
			ValueCheck	(25, OpCode.Var);
			ValueFinal	(30);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Var);
			CodeCheck( 1, 0, "a");
			CodeCheck( 5, OpCode.Array);
			CodeCheck( 6, (byte)1);				// dimensions
			CodeCheck( 7, OpCode.Byte);			// byte[]
			CodeCheck( 8, OpCode.Undefined);	// no initializer
			CodeCheck( 9);
		}

		[Test]
		public void ParseExpression_15_CreateArray()
		{
			Test("var a = new byte[n]");

			ValueCheck	( 0, 0, "a");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, OpCode.Undefined);
			ValueTopMark(14, 9);
			ValueCheck	(14, OpCode.Byte);
			ValueTopMark(19, 14);
			ValueCheck	(19, 1, "n");
			ValueCheck	(23, OpCode.Identifier);
			ValueTopMark(28, 19);
			ValueCheck	(28, (byte)2);			// two arguments to new[] - type and size
			ValueCheck	(29, OpCode.Array);
			ValueCheck	(30, OpCode.Create);
			ValueTopMark(35, 14);				// new byte[n]
			ValueCheck	(35, OpCode.Var);
			ValueFinal	(40);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Var);
			CodeCheck( 1, 0, "a");
			CodeCheck( 5, OpCode.Undefined);	// auto-type
			CodeCheck( 6, OpCode.Create);
			CodeCheck( 7, OpCode.Array);
			CodeCheck( 8, (byte)2);				// two arguments to new[] - type and size
			CodeCheck( 9, OpCode.Byte);
			CodeCheck(10, OpCode.Identifier);
			CodeCheck(11, 1, "n");
			CodeCheck(15);
		}

		[Test]
		public void ParseExpression_16_GenericType()
		{
			Test("var a as list.[byte]"); // Boo/Python type-spec (var x as type)

			ValueCheck	( 0, 0, "a");
			ValueCheck	( 4, OpCode.Identifier);
			ValueTopMark( 9, 0);
			ValueCheck	( 9, 1, "list");
			ValueCheck	(13, OpCode.Identifier);
			ValueTopMark(18, 9);
			ValueCheck	(18, OpCode.Byte);
			ValueTopMark(23, 18);
			ValueCheck	(23, (byte)2);			// two arguments/types to generic
			ValueCheck	(24, OpCode.Generic);
			ValueTopMark(29, 9);
			ValueCheck	(29, OpCode.Undefined);
			ValueTopMark(34, 29);
			ValueCheck	(34, OpCode.Var);
			ValueFinal	(39);

			Rewrite(ValuesAt);

			CodeCheck( 0, OpCode.Var);
			CodeCheck( 1, 0, "a");
			CodeCheck( 5, OpCode.Generic);
			CodeCheck( 6, (byte)2);				// two arguments/types to generic
			CodeCheck( 7, OpCode.Identifier);
			CodeCheck( 8, 1, "list");
			CodeCheck(12, OpCode.Byte);
			CodeCheck(13, OpCode.Undefined);
			CodeCheck(14);
		}
	}
}
