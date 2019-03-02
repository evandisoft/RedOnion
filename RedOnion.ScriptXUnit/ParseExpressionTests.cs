using System;
using System.Linq;
using Xunit;
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
			=> Assert.Equal(at, CodeAt);
		public void CodeCheck(int at, OpCode value)
			=> CodeCheck(at, (byte)value);

		public void CodeCheck(int at, byte value)
		{
			Assert.True(at <= CodeAt-1);
			Assert.Equal(value, Code[at]);
		}
		public void CodeCheck(int at, int value)
		{
			Assert.True(at <= CodeAt-4);
			Assert.Equal(value, CodeInt(at));
		}
		public void CodeCheck(int at, float value)
		{
			Assert.True(at <= CodeAt-4);
			Assert.Equal(value, CodeFloat(at));
		}
		public void CodeCheck(int at, long value)
		{
			Assert.True(at <= CodeAt-8);
			Assert.Equal(value, CodeLong(at));
		}
		public void CodeCheck(int at, double value)
		{
			Assert.True(at <= CodeAt-8);
			Assert.Equal(value, CodeDouble(at));
		}
		public void CodeCheck(int at, int index, string value)
		{
			Assert.True(at <= CodeAt-4);
			Assert.Equal(index, CodeInt(at));
			if (index == -1)
				Assert.True(value.Length == 0, "Empty string");
			else
			{
				Assert.True(index < StringsAt);
				Assert.Equal(value, Strings[index]);
			}
		}

		public void ValueCheck(int at, byte value)
		{
			Assert.True(at <= ValuesAt-1);
			Assert.Equal(value, Values[at]);
		}
		public void ValueCheck(int at, int value)
		{
			Assert.True(at <= ValuesAt-4);
			Assert.Equal(value, ValueInt(at));
		}
		public void ValueCheck(int at, float value)
		{
			Assert.True(at <= ValuesAt-4);
			Assert.Equal(value, ValueFloat(at));
		}
		public void ValueCheck(int at, long value)
		{
			Assert.True(at <= ValuesAt-8);
			Assert.Equal(value, ValueLong(at));
		}
		public void ValueCheck(int at, double value)
		{
			Assert.True(at <= ValuesAt-8);
			Assert.Equal(value, ValueDouble(at));
		}

		public void ValueTopMark(int at, int value)
		{
			Assert.True(at < ValuesAt);
			Assert.Equal(value, TopInt(at));
		}
		public void ValueTopMark(int value)
		{
			Assert.Equal(value, TopInt());
		}
		public void ValueFinal(int value)
		{
			Assert.Equal(value, ValuesAt);
			ValueTopMark(0);
		}

		public void ValueCheck(int at, int index, string value)
		{
			Assert.True(at <= ValuesAt-4, "Values: {0} > {1}");
			Assert.Equal(index, ValueInt(at));
			if (index == -1)
				Assert.True(value.Length == 0);
			else
			{
				Assert.True(index < StringValuesAt);
				Assert.Equal(value, StringValues[index]);
			}
		}
		public void ValueCheck(int at, OpCode value)
			=> ValueCheck(at, (byte)value);
	}

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

		[Fact]
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