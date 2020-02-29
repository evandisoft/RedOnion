using System;
using System.Collections;
using NUnit.Framework;
using RedOnion.ROS.Objects;

namespace RedOnion.ROS.Tests
{
	public class CoreTests : Processor.WithEvents
	{
		public void Test(string script, int countdown = 1000)
		{
			try
			{
				Code = Compile(script);
				if (Globals == null) Globals = new Globals();
				Assert.IsTrue(Execute(countdown));
			}
			catch (Exception e)
			{
				throw new Exception(String.Format("{0} in Eval: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, script), e);
			}
		}
		public void Test(object value, string script, int countdown = 1000)
		{
			Test(script, countdown);
			if (result.IsFpNumber)
			{
				Assert.IsTrue(value is double || value is float || value is decimal,
					"Different type! Actual: {0} Expected: {1}\n<{2}>",
					Result.desc.Type.Name, value.GetType().Name, script);
				var result = Result.ToDouble();
				var expect = ((IConvertible)value).ToDouble(Value.Culture);
				Assert.IsTrue(
					double.IsNaN(result) && double.IsNaN(expect) ||
					Math.Abs(result - expect) < (value is float ? 1e-6 : 1e-12),
					"Different result! Actual: {0} Expected: {1}\n<{2}>",
					Result, value, script);
			}
			else
			{
				var result = Result.Box();
				Assert.AreEqual(value, result, "Different result: <{0}>", script);
				Assert.AreEqual(value?.GetType(), result?.GetType(), "Different type: <{0}>", script);
			}
		}
		public void Expect<Ex>(string script, int countdown = 1000) where Ex : Exception
		{
			try
			{
				Code = Compile(script);
				Execute(countdown);
				Assert.Fail("Should throw " + typeof(Ex).Name);
			}
			catch (Ex)
			{
			}
			catch (Exception e)
			{
				if (e is RuntimeError re && re.InnerException is Ex)
					return;
				throw new Exception(String.Format("{0} in Eval: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, script), e);
			}
		}
		public void Lines(params string[] lines)
			=> Test(string.Join(Environment.NewLine, lines));
		public void Lines(object value, params string[] lines)
			=> Test(value, string.Join(Environment.NewLine, lines));
		public void Expect<Ex>(params string[] lines) where Ex : Exception
			=> Expect<Ex>(string.Join(Environment.NewLine, lines));
	}

	[TestFixture]
	public class ROS_Expressions : CoreTests
	{
		[Test]
		public void ROS_Expr01_Simple()
		{
			Test("onion",	"\"onion\"");	// string
			Test(1,			"1");           // integer
			Test(3.14,		"3.14");        // double
			Test(5.5f,		"5.5f");        // float
			Test(10,		"0xA");         // hex integer
			Test(-7,		"-7");          // should be parsed as true -7, not as -(7)
			Test(7,         "+7");          // unary plus
			Test(.1,		".1");			// double starting with dot
			Test(7u,		"7u");          // unsigned integer
		}

		[Test]
		public void ROS_Expr02_AddMul()
		{
			Test(3,			"1+2");			// simple +
			Test(12,		"3*4");         // simple *
			Test(1.1,		"1+0.1");       // double vs. int
			Test(7,			"1+2*3");       // operator priority
			Test(8.5,		"1+2.5*3");     // mixed double and ints
			Test((long)2,	"1+1u");        // signed + unsigned integer
			Test((long)2,	"1u+1");        // unsigned + signed integer
		}

		[Test]
		public void ROS_Expr03_DivideAndNaN()
		{
			Test(12 / 5,	"12/5");        // integer division
			Test(12f / 5,	"12f/5");		// float division
			Test(double.NaN,"0/0");         // division by zero is NaN
			Test(double.NaN,"0/.0");        // division by zero is NaN
		}

		[Test]
		public void ROS_Expr04_Variables()
		{
			Test(			"var x");		// declare
			Test(10,		"x = 2*3+4");   // assign
			Test(10,		"x");           // test
			Test("x10",		"\"x\" + x");   // string + integer
			Test(10,		"x++");         // post-increment
			Test(12,		"++x");         // pre-increment
			Test(12,        "x--");         // post-decrement
			Test(10,        "--x");         // pre-decrement
			Test(-10,        "-x");         // unary minus
			Test(10,         "+x");         // unary plus
			Test(11,        "x+=1");        // compound assignment - add
			Test(22,		"x*=2");        // compound assignment - mul

			Test(1, "var v = 1");  // integer
			Test(2.0, "v += 1.0"); // double
			Test(1.5, "v = math.clamp v, math.max(0, 1.0), math.min(1.5, 2)");
			//Test("var i, j");//TODO: maybe only as statement, because `if var x = true, y = false` could be a problem (return last?)
		}

		[Test]
		public void ROS_Expr05_Properties()
		{
			Test("var obj = new object");	// object creation
			Assert.IsTrue(Result.obj is UserObject);

			Test("obj.x = 3.14");           // assign property
			Test(3.14,		"obj.x");       // test property

			Test("obj.s = \"xyz\"");
			Test("xyz",		"obj.s");
			Test(3,			"obj.s.length");// test internal property
			Test(3,			"obj[\"s\"].length");
			Test(3,			"obj[\"s\"][\"length\"]");
			Test(3,			"obj[\"s\", \"length\"]");

			Test("obj.test = new obj");     // derived object
			Test(3.14, "obj.test.x");       // inherited property
			Test("obj.test.x = 2.7");
			Test(3.14, "obj.x");            // must not change parent's property
			Test(2.7, "obj.test.x");
			Test("var test = new obj.test");
			Test(2.7, "test.x");
		}

		[Test]
		public void ROS_Expr06_Logic()
		{
			Test(true,		"1 < 2");			// integers
			Test(true,		"1 > .1");			// integer vs. double - less
			Test(true,		"1 == 1.0");		// integer vs. double - equal

			Test(true,		"true || false");	// logic or
			Test(true,		"false || true");
			Test(false,		"true && false");	// logic and
			Test(false,		"false and true");
			Test(true,		"true && true");

			Test(true,      "null ?? true");    // null-coalescing operator
			Test(false,		"false ?? true");

			Test(1,			"0 or 1");			// or with number
			Test(1,			"1 || 2");
			Test("me",		"null or \"me\"");  // or with null
			Test(null,		"null and \"me\""); // and with null

			Test(false,		"not true");		// logic not
			Test(true,		"not false");
			Test(true,		"not 0");
			Test(false,		"not 1");

			Test(true,		"1 === 1");			// identity with numbers
			Test(false,		"1 !== 1");
			Test(false,		"1 === 2");
			Test(true,		"\"a\" === \"a\"");	// identity with strings
			Test(false,		"\"a\" === \"b\"");
			Test(true,		"\"a\" !== \"b\"");
			Test(true,		"\"a\" == \"A\"");	// equality is case insensitive
			Test(false,		"\"a\" === \"A\"");	// identity is case sensitive

			// this was causing some problems (fixed)
			Test("var s = \"hello\"");
			Test(false, "s.length <= 3 || s[3] == '.'");

			// type tests
			Test(true, "s is string");
			Test(false, "s is int");
			Test(true, "1 is int");
			Test(true, "2.0 is double");
			Test(true, "3f is! double"); // is! means 'is not' = negated 'is'
			Test(true, "4f is float");

			// property existence tests (comes from JavaScript)
			Test(true, "\"length\" in s");
			Test(false, "\"blah\" in s");
		}

		[Test]
		public void ROS_Expr07_Ternary()
		{
			Test(true,		"true ? true : false");
			Test(false,		"false ? true : false");
			Test(2,			"0 != 0 ? 1 : 2");
		}

		[Test]
		public void ROS_Expr08_ArrayLiteral()
		{
			foreach (var s in new[] { "[1,2]", "[\n1\n,\n2\n]" })
			{
				Test(s);
				var arr = Result.obj as IList;
				Assert.NotNull(arr);
				Assert.AreEqual(2, arr.Count);
				Assert.IsTrue(arr[0].Equals(1));
				Assert.IsTrue(arr[1].Equals(2));
			}
			foreach (var s in new[] { "[]", "[\n]" })
			{
				Test(s);
				var arr = Result.obj as Array;
				Assert.NotNull(arr);
				Assert.AreEqual(0, arr.Length);
			}
			foreach (var s in new[] { "[true]", "[\ntrue\n]" })
			{
				Test(s);
				var arr = Result.obj as IList;
				Assert.NotNull(arr);
				Assert.AreEqual(1, arr.Count);
				Assert.IsTrue(arr[0].Equals(true));
			}
			Test(@"var a = [""hello"",""world""]");
			Test(2, "a.length");
			Test("hello", "a[0]");
			Test("world", "a[1]");

			Lines(3,
				"var x = 1",
				"var y = 2",
				"var a = [x, y]",
				"return a[0] + a[1]");
		}

		[Test]
		public void ROS_Expr09_NewLines()
		{
			foreach (var s in new[]
			{
				"(\n+1\n)",
				"-1\\\n+ 2",
				"def pass x => x\npass(\n1\n)",
				"def sum2 x,y => x+y"
				+ "\nsum2 pass(2),\npass(-1)"
			})
			{
				Test(1, s);
			}
		}

		[Test]
		public void ROS_Expr10_Print()
		{
			Test("3.14", @"print ""{0:F2}"", math.pi");
		}

		[Test]
		public void ROS_Expr11_Strings()
		{
			Test(true, "\"x\".contains \"x\"");
			Test(true, "\"xyz\".contains \"y\"");
			Test(true, "\"xyz\".contains \"yz\"");
			Test(false, "\"xyz\".contains \"a\"");

			Test("yz", "\"xyz\".substr 1");
			Test("y", "\"xyz\".substring 1, 1");
		}
	}
}
