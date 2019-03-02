using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedOnion.Script.Parsing;
using RedOnion.Script.Execution;

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

	[TestClass]
	public class ParseExpressionTests : ParseTestsBase
	{
		public void Test(string line)
		{
			try
			{
				lexer.Line = line;
				ParseExpression();
			}
			catch (Exception e)
			{
				throw new Exception(String.Format("{0}: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, line), e);
			}
		}

		[TestMethod]
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
	}
}
