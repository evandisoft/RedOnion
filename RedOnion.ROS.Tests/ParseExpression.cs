using System;
using NUnit.Framework;
using RedOnion.ROS.Parsing;

//	x + 1
//	1u + x * 3f
//	(1L + x) * 3.0
//	cond ? true : false
//	++x--
//	"string" + 'c'
//	abs(-1)
//	abs -1
//	abs(-x)
//	abs -x
//	fn(x,y)
//	fn x,y
//	fn(null, this, base.field)
//	fn null, this, base.field
//	f g(x), h x
//	f (g x, y), z, h()
//	fn new pt 1, 2
//	var x
//	var x int
//	var a:byte[]
//	var a = new byte[n]

namespace RedOnion.ROS.Tests
{
	public class ParserTests : Parser
	{
		public int CodeInt(int at)
			=> BitConverter.ToInt32(code.items, at);
		public float CodeFloat(int at)
			=> BitConverter.ToSingle(code.items, at);
		public long CodeLong(int at)
			=> BitConverter.ToInt64(code.items, at);
		public double CodeDouble(int at)
			=> BitConverter.ToDouble(code.items, at);

		public int ValueInt(int at)
			=> BitConverter.ToInt32(values.items, at);
		public float ValueFloat(int at)
			=> BitConverter.ToSingle(values.items, at);
		public long ValueLong(int at)
			=> BitConverter.ToInt64(values.items, at);
		public double ValueDouble(int at)
			=> BitConverter.ToDouble(values.items, at);

		public void CodeCheck(int at)
			=> Assert.AreEqual(at, code.size, "code.size");
		public void CodeCheck(int at, OpCode value)
			=> CodeCheck(at, (byte)value);

		public void CodeCheck(int at, byte value)
		{
			Assert.IsTrue(at <= code.size-1, "code.size: {0} > {1}", at, code.size-1);
			Assert.AreEqual(value, code.items[at], "code[{0}] = 0x{1:X2} ! 0x{2:X2}", at, code.items[at], value);
		}
		public void CodeCheck(int at, int value)
		{
			Assert.IsTrue(at <= code.size-4, "code.size: {0} > {1}", at, code.size-4);
			Assert.AreEqual(value, CodeInt(at), "CodeInt({0})", at);
		}
		public void CodeCheck(int at, float value)
		{
			Assert.IsTrue(at <= code.size-4, "code.size: {0} > {1}", at, code.size-4);
			Assert.AreEqual(value, CodeFloat(at), "CodeFloat({0})", at);
		}
		public void CodeCheck(int at, long value)
		{
			Assert.IsTrue(at <= code.size-8, "code.size: {0} > {1}", at, code.size-8);
			Assert.AreEqual(value, CodeLong(at), "CodeLong({0})", at);
		}
		public void CodeCheck(int at, double value)
		{
			Assert.IsTrue(at <= code.size-8, "code.size: {0} > {1}", at, code.size-8);
			Assert.AreEqual(value, CodeDouble(at), "CodeDouble({0})", at);
		}
		public void CodeCheck(int at, int index, string value)
		{
			var limit = code.size - 4;
			Assert.IsTrue(at <= limit, "code.size: {0} > {1}", at, limit);
			Assert.AreEqual(index, CodeInt(at), "CodeInt({0})", at);
			if (index == -1)
				Assert.IsTrue(value.Length == 0, "Empty string");
			else
			{
				Assert.IsTrue(index < strings.size, "strings.size: {0} >= {1}", index, strings.size);
				Assert.AreEqual(value, strings.items[index], "strings[{0}]", index);
			}
		}

		public void ValueCheck(int at, byte value)
		{
			Assert.IsTrue(at <= values.size-1, "values.size: {0} > {1}", at, values.size-1);
			Assert.AreEqual(value, values.items[at], "Values[{0}] = 0x{1:X2} ! 0x{2:X2}", at, values.items[at], value);
		}
		public void ValueCheck(int at, int value)
		{
			Assert.IsTrue(at <= values.size-4, "values.size: {0} > {1}", at, values.size-4);
			Assert.AreEqual(value, ValueInt(at), "ValueInt({0})", at);
		}
		public void ValueCheck(int at, float value)
		{
			Assert.IsTrue(at <= values.size-4, "values.size: {0} > {1}", at, values.size-4);
			Assert.AreEqual(value, ValueFloat(at), "ValueFloat({0})", at);
		}
		public void ValueCheck(int at, long value)
		{
			Assert.IsTrue(at <= values.size-8, "values.size: {0} > {1}", at, values.size-8);
			Assert.AreEqual(value, ValueLong(at), "ValueLong({0})", at);
		}
		public void ValueCheck(int at, double value)
		{
			Assert.IsTrue(at <= values.size-8, "values.size: {0} > {1}", at, values.size-8);
			Assert.AreEqual(value, ValueDouble(at), "ValueDouble({0})", at);
		}

		public void ValueTopMark(int at, int value)
		{
			Assert.IsTrue(at < values.size, "values.size: {0} >= {1}", at, values.size);
			Assert.AreEqual(value, TopInt(at), "ValueTopMark {0}", at);
		}
		public void ValueTopMark(int value)
		{
			Assert.AreEqual(value, TopInt(), "ValueTopMark");
		}
		public void ValueFinal(int value)
		{
			Assert.AreEqual(value, values.size, "values.size");
			ValueTopMark(0);
		}

		public void ValueCheck(int at, int index, string value)
		{
			Assert.IsTrue(at <= values.size-4, "Values: {0} > {1}", at, values.size-4);
			Assert.AreEqual(index, ValueInt(at), "ValueInt({0})", at);
			if (index == -1)
				Assert.IsTrue(value.Length == 0, "Empty string");
			else
			{
				Assert.IsTrue(index < stringValues.size, "stringValues.size: {0} >= {1}", index, stringValues.size);
				Assert.AreEqual(value, stringValues.items[index], "stringValues[{0}]", index);
			}
		}
		public void ValueCheck(int at, OpCode value)
			=> ValueCheck(at, (byte)value);

		protected virtual void Parse()
			=> Unit();
		protected virtual void Rewrite()
		{ }
		public void Test(string source)
		{
			try
			{
				Reset();
				Source = source;
				Parse();
			}
			catch (Exception e)
			{
				throw new Exception(string.Format("{0}: {1}; IN: <{2}>, OPTS: {3}",
					e.GetType().ToString(), e.Message, source, Options), e);
			}
		}
		public void Lines(params string[] lines)
			=> Test(string.Join(Environment.NewLine, lines));

		public static Option[] TestedOptions = new Option[]
		{
			DefaultOptions | Option.Prefix,
			DefaultOptions &~Option.Prefix,
		};
		public void Test(
			string source, Action testValues,
			Action prefix, Action postfix)
		{
			foreach (var opt in TestedOptions)
			{
				Options = opt;
				var phase = "reset";
				try
				{
					Reset();
					phase = "Source";
					Source = source;
					phase = "Parse";
					Parse();
					phase = "Values";
					testValues?.Invoke();
					phase = "Rewrite";
					Rewrite();
					phase = HasOption(Option.Prefix)
						? "Prefix"
						: "Postfix";
					(HasOption(Option.Prefix)
						? prefix
						: postfix
					)?.Invoke();
				}
				catch (Exception e)
				{
					throw new Exception(string.Format("{0}: {1}; IN: <{2}>, OPTS: {3}, Phase: {4}",
						e.GetType().ToString(), e.Message, source, Options, phase), e);
				}
			}
		}
	}

	[TestFixture]
	public class ROS_ParseExpression : ParserTests
	{
		protected override void Parse() => ParseExpression();
		protected override void Rewrite() => Rewrite(values.size);

		[Test]
		public void ROS_PExpr01_Add()
		{
			Test(
				"x + 1",
				() =>
				{
					ValueCheck	( 0,  0, "x");              // ValueInt( 0) = 0; StringValues[0] = "x"
					ValueCheck	( 4,  OpCode.Identifier);   // Values  [ 4] = (byte)OpCode.Identifier
					ValueTopMark( 9,  0);					// ValueInt( 5) = 0 which is start of "identifier x"
					ValueCheck	( 9,  1);					// ValueInt( 9) = 1 which is the constant in the expression
					ValueCheck	(13,  OpCode.Int);          // Values  [13] = (byte)OpCode.Int = 32bit signed integer
					ValueTopMark(18,  9);					// ValueInt(14) = 9 which is start of "integer 1"
					ValueCheck	(18,  OpCode.Add);			// Values  [18] = (byte)OpCode.Add = the operator
					ValueFinal	(23);						// ValueInt(19) = 0 which is start of it all, values.size = 23
				},
				() =>
				{
					CodeCheck( 0, OpCode.Add);
					CodeCheck( 1, OpCode.Identifier);
					CodeCheck( 2, 0, "x");
					CodeCheck( 6, OpCode.Int);
					CodeCheck( 7, 1);
					CodeCheck(11);
				},
				() =>
				{
					CodeCheck( 0, OpCode.Identifier);
					CodeCheck( 1, 0, "x");
					CodeCheck( 5, OpCode.Int);
					CodeCheck( 6, 1);
					CodeCheck(10, OpCode.Add);
					CodeCheck(11);
				}
			);
		}

		[Test]
		public void ROS_PExpr02_AddMul()
		{
			Test(
				"1u + x * 3f",
				() =>
				{
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
				},
				() =>
				{
					CodeCheck( 0, OpCode.Add);
					CodeCheck( 1, OpCode.UInt);
					CodeCheck( 2, 1);
					CodeCheck( 6, OpCode.Mul);
					CodeCheck( 7, OpCode.Identifier);
					CodeCheck( 8, 0, "x");
					CodeCheck(12, OpCode.Float);
					CodeCheck(13, 3f);
					CodeCheck(17);
				},
				() =>
				{
					CodeCheck( 0, OpCode.UInt);
					CodeCheck( 1, 1);
					CodeCheck( 5, OpCode.Identifier);
					CodeCheck( 6, 0, "x");
					CodeCheck(10, OpCode.Float);
					CodeCheck(11, 3f);
					CodeCheck(15, OpCode.Mul);
					CodeCheck(16, OpCode.Add);
					CodeCheck(17);
				}
			);
		}

		[Test]
		public void ROS_PExpr03_Parenthesis()
		{
			foreach (var s in new string[]
			{
				"(1L + x) * 3.0",
				"(1L\n+ x) * 3.0",
				"(\n1L\n+\nx\n) * 3.0",
			})
			{
				Test(s,
					() =>
					{
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
					},
					() =>
					{
						CodeCheck( 0, OpCode.Mul);
						CodeCheck( 1, OpCode.Add);
						CodeCheck( 2, OpCode.Long);
						CodeCheck( 3, 1L);
						CodeCheck(11, OpCode.Identifier);
						CodeCheck(12, 0, "x");
						CodeCheck(16, OpCode.Double);
						CodeCheck(17, 3.0);
						CodeCheck(25);
					},
					() =>
					{
						CodeCheck( 0, OpCode.Long);
						CodeCheck( 1, 1L);
						CodeCheck( 9, OpCode.Identifier);
						CodeCheck(10, 0, "x");
						CodeCheck(14, OpCode.Add);
						CodeCheck(15, OpCode.Double);
						CodeCheck(16, 3.0);
						CodeCheck(24, OpCode.Mul);
						CodeCheck(25);
					}
				);
			}
		}

		[Test]
		public void ROS_PExpr04_Ternary()
		{
			foreach (var s in new string[]
			{
				"cond ? true : false",
				"(cond\n?\rtrue\r:\nfalse)",
			})
			{
				Test(s,
					() =>
					{
						ValueCheck	( 0, 0, "cond");
						ValueCheck	( 4, OpCode.Identifier);
						ValueTopMark( 9, 0);
						ValueCheck	( 9, OpCode.True);
						ValueTopMark(14, 9);
						ValueCheck	(14, OpCode.False);
						ValueTopMark(19, 14);
						ValueCheck	(19, OpCode.Ternary);
						ValueFinal	(24);
					},
					() =>
					{
						CodeCheck( 0, OpCode.Ternary);
						CodeCheck( 1, OpCode.Identifier);
						CodeCheck( 2, 0, "cond");
						CodeCheck( 6, 1); // size of true-expression for quick skip
						CodeCheck(10, OpCode.True);
						CodeCheck(11, 1); // size of false-expression for quick skip
						CodeCheck(15, OpCode.False);
						CodeCheck(16);
					},
					() =>
					{
						CodeCheck( 0, OpCode.Identifier);
						CodeCheck( 1, 0, "cond");
						CodeCheck( 5, OpCode.Ternary);
						CodeCheck( 6, 1); // size of true-expression for quick skip
						CodeCheck(10, OpCode.True);
						CodeCheck(11, OpCode.Else);
						CodeCheck(12, 1); // size of false-expression for quick skip
						CodeCheck(16, OpCode.False);
						CodeCheck(17);
					}
				);
			}
		}

		[Test]
		public void ROS_PExpr05_Logic()
		{
			var i = 0;
			foreach (var s in new string[]
			{
				"x && y",
				"x and y",
				"x || y",
				"x or y",
			})
			{
				Test(s,
					() =>
					{
						ValueCheck	( 0, 0, "x");
						ValueCheck	( 4, OpCode.Identifier);
						ValueTopMark( 9, 0);
						ValueCheck	( 9, 1, "y");
						ValueCheck	(13, OpCode.Identifier);
						ValueTopMark(18, 9);
						ValueCheck	(18, i >= 2 ? OpCode.LogicOr : OpCode.LogicAnd);
						ValueFinal	(23);
					},
					() =>
					{
						CodeCheck( 0, i >= 2 ? OpCode.LogicOr : OpCode.LogicAnd);
						CodeCheck( 1, OpCode.Identifier);
						CodeCheck( 2, 0, "x");
						CodeCheck( 6, 5); // size of second expression for quick skip
						CodeCheck(10, OpCode.Identifier);
						CodeCheck(11, 1, "y");
						CodeCheck(15);
					},
					() =>
					{
						CodeCheck( 0, OpCode.Identifier);
						CodeCheck( 1, 0, "x");
						CodeCheck( 5, i >= 2 ? OpCode.LogicOr : OpCode.LogicAnd);
						CodeCheck( 6, 5); // size of second expression for quick skip
						CodeCheck(10, OpCode.Identifier);
						CodeCheck(11, 1, "y");
						CodeCheck(15);
					}
				);
				i++;
			}
		}

		[Test]
		public void ROS_PExpr06_PreAndPost()
		{
			Test(
				"++x--",
				() =>
				{
					ValueCheck( 0, 0, "x");
					ValueCheck( 4, OpCode.Identifier);
					ValueCheck( 5, OpCode.Inc);
					ValueCheck( 6, OpCode.PostDec);
					ValueFinal(11);
				},
				() =>
				{
					CodeCheck(0, OpCode.PostDec);
					CodeCheck(1, OpCode.Inc);
					CodeCheck(2, OpCode.Identifier);
					CodeCheck(3, 0, "x");
					CodeCheck(7);
				},
				() =>
				{
					CodeCheck(0, OpCode.Identifier);
					CodeCheck(1, 0, "x");
					CodeCheck(5, OpCode.Inc);
					CodeCheck(6, OpCode.PostDec);
					CodeCheck(7);
				}
			);
		}

		[Test]
		public void ROS_PExpr07_StringAndChar()
		{
			Test(
				"\"string\" + 'c'",
				() =>
				{
					ValueCheck	( 0, 0, "string");
					ValueCheck	( 4, OpCode.String);
					ValueTopMark( 9, 0);
					ValueCheck	( 9, (byte)'c');
					ValueCheck	(10, OpCode.Char);
					ValueTopMark(15, 9);
					ValueCheck	(15, OpCode.Add);
					ValueFinal	(20);
				},
				() =>
				{
					CodeCheck(0, OpCode.Add);
					CodeCheck(1, OpCode.String);
					CodeCheck(2, 0, "string");
					CodeCheck(6, OpCode.Char);
					CodeCheck(7, (byte)'c');
					CodeCheck(8);
				},
				() =>
				{
					CodeCheck(0, OpCode.String);
					CodeCheck(1, 0, "string");
					CodeCheck(5, OpCode.Char);
					CodeCheck(6, (byte)'c');
					CodeCheck(7, OpCode.Add);
					CodeCheck(8);
				}
			);
		}

		[Test]
		public void ROS_PExpr08_CallWithUnary()
		{
			var i = 0;
			foreach (var s in new string[]
			{
				"abs(-1)",
				"abs -1",
				"abs(-x)",
				"abs -x",
			})
			{
				Test(s,
					() =>
					{
						ValueCheck	( 0, 0, "abs");
						ValueCheck	( 4, OpCode.Identifier);
						ValueTopMark( 9, 0);
						if (i < 2)
						{ 
							ValueCheck	( 9, -1);
							ValueCheck	(13, OpCode.Int);
							ValueTopMark(18, 9);
							ValueCheck	(18, OpCode.Call1);
							ValueFinal	(23);
						}
						else
						{
							ValueCheck	( 9, 1, "x");
							ValueCheck	(13, OpCode.Identifier);
							ValueCheck	(14, OpCode.Neg);
							ValueTopMark(19, 9);
							ValueCheck	(19, OpCode.Call1);
							ValueFinal	(24);
						}
					},
					() =>
					{
						CodeCheck(0, OpCode.Call1);
						CodeCheck(1, OpCode.Identifier);
						CodeCheck(2, 0, "abs");
						if (i < 2)
						{
							CodeCheck( 6, OpCode.Int);
							CodeCheck( 7, -1);
							CodeCheck(11);
						}
						else
						{
							CodeCheck( 6, OpCode.Neg);
							CodeCheck( 7, OpCode.Identifier);
							CodeCheck( 8, 1, "x");
							CodeCheck(12);
						}
					},
					() =>
					{
						if (i < 2)
						{
							CodeCheck( 0, OpCode.Identifier);
							CodeCheck( 1, 0, "abs");
							CodeCheck( 5, OpCode.Int);
							CodeCheck( 6, -1);
							CodeCheck(10, OpCode.Call1);
							CodeCheck(11);
						}
						else
						{
							CodeCheck( 0, OpCode.Identifier);
							CodeCheck( 1, 0, "abs");
							CodeCheck( 5, OpCode.Identifier);
							CodeCheck( 6, 1, "x");
							CodeCheck(10, OpCode.Neg);
							CodeCheck(11, OpCode.Call1);
							CodeCheck(12);
						}
					}
				);
				i++;
			}
		}

		[Test]
		public void ROS_PExpr09_CallTwoArgs()
		{
			foreach (var s in new string[]
			{
				"fn(x,y)",
				"fn x,y",
			})
			{
				Test(s,
					() =>
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
					},
					() =>
					{
						CodeCheck( 0, OpCode.Call2);
						CodeCheck( 1, OpCode.Identifier);
						CodeCheck( 2, 0, "fn");
						CodeCheck( 6, OpCode.Identifier);
						CodeCheck( 7, 1, "x");
						CodeCheck(11, OpCode.Identifier);
						CodeCheck(12, 2, "y");
						CodeCheck(16);
					},
					() =>
					{
						CodeCheck( 0, OpCode.Identifier);
						CodeCheck( 1, 0, "fn");
						CodeCheck( 5, OpCode.Identifier);
						CodeCheck( 6, 1, "x");
						CodeCheck(10, OpCode.Identifier);
						CodeCheck(11, 2, "y");
						CodeCheck(15, OpCode.Call2);
						CodeCheck(16);
					}
				);
			}
		}

		[Test]
		public void ROS_PExpr10_CallManyArgs()
		{
			foreach (var s in new string[]
			{
				"fn(null, this, base.field)",
				"fn null, this, base.field",
			})
			{
				Test(s,
					() =>
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
					},
					() =>
					{
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
					},
					() =>
					{
						CodeCheck( 0, OpCode.Identifier);
						CodeCheck( 1, 0, "fn");
						CodeCheck( 5, OpCode.Null);
						CodeCheck( 6, OpCode.This);
						CodeCheck( 7, OpCode.Base);
						CodeCheck( 8, OpCode.Dot);
						CodeCheck( 9, 1, "field");
						CodeCheck(13, OpCode.CallN);
						CodeCheck(14, (byte)4);
						CodeCheck(15);
					}
				);
			}
		}

		[Test]
		public void ROS_PExpr11_NestedCalls()
		{
			Test(
				"f g(x), h x",
				() =>
				{
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
				},
				() =>
				{
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
				},
				() =>
				{
					CodeCheck( 0, OpCode.Identifier);
					CodeCheck( 1, 0, "f");
					CodeCheck( 5, OpCode.Identifier);
					CodeCheck( 6, 1, "g");
					CodeCheck(10, OpCode.Identifier);
					CodeCheck(11, 2, "x");
					CodeCheck(15, OpCode.Call1);
					CodeCheck(16, OpCode.Identifier);
					CodeCheck(17, 3, "h");
					CodeCheck(21, OpCode.Identifier);
					CodeCheck(22, 2, "x"); // final string table must reuse strings
					CodeCheck(26, OpCode.Call1);
					CodeCheck(27, OpCode.Call2);
					CodeCheck(28);
				}
			);
		}

		[Test]
		public void ROS_PExpr12_MoreCalls()
		{
			Test(
				"f (g x, y), z, h()",
				() =>
				{
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
				},
				() =>
				{
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
				},
				() =>
				{
					CodeCheck( 0, OpCode.Identifier);
					CodeCheck( 1, 0, "f");
					CodeCheck( 5, OpCode.Identifier);
					CodeCheck( 6, 1, "g");
					CodeCheck(10, OpCode.Identifier);
					CodeCheck(11, 2, "x");
					CodeCheck(15, OpCode.Identifier);
					CodeCheck(16, 3, "y");
					CodeCheck(20, OpCode.Call2);
					CodeCheck(21, OpCode.Identifier);
					CodeCheck(22, 4, "z");
					CodeCheck(26, OpCode.Identifier);
					CodeCheck(27, 5, "h");
					CodeCheck(31, OpCode.Call0);
					CodeCheck(32, OpCode.CallN);
					CodeCheck(33, (byte)4);
					CodeCheck(34);
				}
			);
		}

		[Test]
		public void ROS_PExpr13_Variable()
		{
			Test("var x",
				() =>
				{
					ValueCheck	( 0, 0, "x");
					ValueCheck	( 4, OpCode.Identifier);
					ValueTopMark( 9, 0);
					ValueCheck	( 9, OpCode.Void);
					ValueTopMark(14, 9);
					ValueCheck	(14, OpCode.Void);
					ValueTopMark(19, 14);
					ValueCheck	(19, OpCode.Var);
					ValueFinal	(24);
				},
				() =>
				{
					CodeCheck( 0, OpCode.Var);
					CodeCheck( 1, 0, "x");
					CodeCheck( 5, OpCode.Void);	// type
					CodeCheck( 6, OpCode.Void);	// no initializer
					CodeCheck( 7);
				},
				() =>
				{
					CodeCheck( 0, OpCode.Void);  // type
					CodeCheck( 1, OpCode.Void);  // no initializer
					CodeCheck( 2, OpCode.Var);
					CodeCheck( 3, 0, "x");
					CodeCheck( 7);
				}
			);
		}

		[Test]
		public void ROS_PExpr14_TypedVariable()
		{
			Test(
				"var x int", // pure style
				() =>
				{
					ValueCheck	( 0, 0, "x");
					ValueCheck	( 4, OpCode.Identifier);
					ValueTopMark( 9, 0);
					ValueCheck	( 9, OpCode.Int);
					ValueTopMark(14, 9);
					ValueCheck	(14, OpCode.Void);
					ValueTopMark(19, 14);
					ValueCheck	(19, OpCode.Var);
					ValueFinal	(24);
				},
				() =>
				{
					CodeCheck( 0, OpCode.Var);
					CodeCheck( 1, 0, "x");
					CodeCheck( 5, OpCode.Int);	// type
					CodeCheck( 6, OpCode.Void);	// no initializer
					CodeCheck( 7);
				},
				() =>
				{
					CodeCheck(0, OpCode.Type);	// we need to know we are going to parse type in postfix mode
					CodeCheck(1, OpCode.Int);	// type
					CodeCheck(2, OpCode.Void);	// no initializer
					CodeCheck(3, OpCode.Var);
					CodeCheck(4, 0, "x");
					CodeCheck(8);
				}
			);
		}

		[Test]
		public void ROS_PExpr15_ArrayVariable()
		{
			foreach (var s in new string[]
			{
				"var a:byte[]", // ActionScript
				"var a:byte[\n]"
			})
			{
				Test(s,
					() =>
					{
						ValueCheck( 0, 0, "a");
						ValueCheck	( 4, OpCode.Identifier);
						ValueTopMark( 9, 0);
						ValueCheck	( 9, OpCode.Byte);
						ValueTopMark(14, 9);
						ValueCheck	(14, (byte)1);		// dimensions
						ValueCheck	(15, OpCode.Array);
						ValueTopMark(20, 9);			// byte[]
						ValueCheck	(20, OpCode.Void);	// no initializer
						ValueTopMark(25, 20);
						ValueCheck	(25, OpCode.Var);
						ValueFinal	(30);
					},
					() =>
					{
						CodeCheck( 0, OpCode.Var);
						CodeCheck( 1, 0, "a");
						CodeCheck( 5, OpCode.Array);
						CodeCheck( 6, (byte)1);			// dimensions
						CodeCheck( 7, OpCode.Byte);		// byte[]
						CodeCheck( 8, OpCode.Void);		// no initializer
						CodeCheck( 9);
					},
					() =>
					{
						CodeCheck( 0, OpCode.Type);
						CodeCheck( 1, OpCode.Byte);      // byte[]
						CodeCheck( 2, OpCode.Type);
						CodeCheck( 3, OpCode.Array);
						CodeCheck( 4, (byte)1);          // dimensions
						CodeCheck( 5, OpCode.Void);      // no initializer
						CodeCheck( 6, OpCode.Var);
						CodeCheck( 7, 0, "a");
						CodeCheck(11);
					}
				);
			}
		}

		[Test]
		public void ROS_PExpr16_CreateArray()
		{
			Test(
				"var a = new byte[n]",
				() =>
				{
					ValueCheck	( 0, 0, "a");
					ValueCheck	( 4, OpCode.Identifier);
					ValueTopMark( 9, 0);
					ValueCheck	( 9, OpCode.Void);
					ValueTopMark(14, 9);
					ValueCheck	(14, OpCode.Byte);
					ValueTopMark(19, 14);
					ValueCheck	(19, 1, "n");
					ValueCheck	(23, OpCode.Identifier);
					ValueTopMark(28, 19);
					ValueCheck	(28, (byte)2);		// two arguments to new[] - type and size
					ValueCheck	(29, OpCode.Array);
					ValueCheck	(30, OpCode.Create);
					ValueTopMark(35, 14);			// new byte[n]
					ValueCheck	(35, OpCode.Var);
					ValueFinal	(40);
				},
				() =>
				{
					CodeCheck( 0, OpCode.Var);
					CodeCheck( 1, 0, "a");
					CodeCheck( 5, OpCode.Void);		// auto-type variable
					CodeCheck( 6, OpCode.Create);
					CodeCheck( 7, OpCode.Array);
					CodeCheck( 8, (byte)2);			// two arguments to new[] - type and size
					CodeCheck( 9, OpCode.Byte);		// array of bytes
					CodeCheck(10, OpCode.Identifier);
					CodeCheck(11, 1, "n");			// size
					CodeCheck(15);
				},
				() =>
				{
					CodeCheck( 0, OpCode.Void);		// auto-type variable
					CodeCheck( 1, OpCode.Type);		// ... of bytes
					CodeCheck( 2, OpCode.Byte);
					CodeCheck( 3, OpCode.Identifier);
					CodeCheck( 4, 0, "n");			// size
					CodeCheck( 8, OpCode.Create);	// new
					CodeCheck( 9, OpCode.Array);	// ...[]
					CodeCheck(10, (byte)2);			// two arguments to new[] - type and size
					CodeCheck(11, OpCode.Var);
					CodeCheck(12, 1, "a");
					CodeCheck(16);
				}
			);
		}

		[Test]
		public void ROS_PExpr17_AutocallWithNew()
		{
			Test(
				"fn new pt 1, 2",
				() =>
				{
					ValueCheck	( 0, 0, "fn");
					ValueCheck	( 4, OpCode.Identifier);
					ValueTopMark( 9, 0);
					ValueCheck	( 9, 1, "pt");
					ValueCheck	(13, OpCode.Identifier);
					ValueTopMark(18, 9);
					ValueCheck	(18, 1);
					ValueCheck	(22, OpCode.Int);
					ValueTopMark(27, 18);
					ValueCheck	(27, 2);
					ValueCheck	(31, OpCode.Int);
					ValueTopMark(36, 27);
					ValueCheck	(36, OpCode.Call2);
					ValueCheck	(37, OpCode.Create);
					ValueTopMark(42, 9);
					ValueCheck	(42, OpCode.Call1);
					ValueFinal	(47);
				},
				() =>
				{
					CodeCheck(0, OpCode.Call1);
					CodeCheck(1, OpCode.Identifier);
					CodeCheck(2, 0, "fn");
					CodeCheck(6, OpCode.Create);
					CodeCheck(7, OpCode.Call2);
					CodeCheck(8, OpCode.Identifier);
					CodeCheck(9, 1, "pt");
					CodeCheck(13, OpCode.Int);
					CodeCheck(14, 1);
					CodeCheck(18, OpCode.Int);
					CodeCheck(19, 2);
					CodeCheck(23);
				},
				() =>
				{
					CodeCheck( 0, OpCode.Identifier);
					CodeCheck( 1, 0, "fn");
					CodeCheck( 5, OpCode.Identifier);
					CodeCheck( 6, 1, "pt");
					CodeCheck(10, OpCode.Int);
					CodeCheck(11, 1);
					CodeCheck(15, OpCode.Int);
					CodeCheck(16, 2);
					CodeCheck(20, OpCode.Create);
					CodeCheck(21, OpCode.Call2);
					CodeCheck(22, OpCode.Call1);
					CodeCheck(23);
				}
			);
		}
	}
}
