using System;
using System.Collections;
using NUnit.Framework;
using RedOnion.ROS.Objects;

namespace RedOnion.ROS.Tests
{
	public class CoreTests : Core
	{
		public void Test(string script, int countdown = 100)
		{
			try
			{
				Code = Compile(script);
				if (Globals == null) Globals = new Globals();
				Execute(countdown);
			}
			catch (Exception e)
			{
				throw new Exception(String.Format("{0} in Eval: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, script), e);
			}
		}
		public void Test(object value, string script, int countdown = 100)
		{
			Test(script, countdown);
			var result = Result.Object;
			Assert.AreEqual(value, result, "Different result: <{0}>", script);
			Assert.AreEqual(value.GetType(), result.GetType(), "Different type: <{0}>", script);
		}
		public void Expect<Ex>(string script, int countdown = 100) where Ex : Exception
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
		}

		[Test]
		public void ROS_Expr06_Logic()
		{
			Test(true,		"1 < 2");       // integers
			Test(true,		"1 > .1");      // integer vs. double - less
			Test(true,		"1 == 1.0");    // integer vs. double - equal
			Test(true,		"true || false");
			Test(true,		"false || true");
			Test(false,		"true && false");
			Test(false,		"false && true");
			Test(true,		"true && true");

			Test("var s = \"hello\"");
			Test(false, "s.length <= 3 || s[3] == '.'");
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
		}

		[Test]
		public void ROS_Expr09_NewLines()
		{
			foreach (var s in new[]
			{
				"(\n+1\n)",
				//"def pass x => x\npass(\n1\n)"
			})
			{
				Test(1, s);
			}
		}
	}
}
