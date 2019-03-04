using System;
using System.Linq;
using NUnit.Framework;
using RedOnion.Script;

namespace RedOnion.ScriptNUnit
{
	public class EngineTestsBase : Engine
	{
		public void Test(string script)
		{
			try
			{
				Execute(script);
			}
			catch (Exception e)
			{
				throw new Exception(String.Format("{0} in Eval: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, script), e);
			}
		}

		public void Test(object value, string script)
		{
			Test(script);
			Assert.AreEqual(value, Result.Native, "Test: <{0}>", script);
		}
	}

	[TestFixture]
	public class ExpressionTests : EngineTestsBase
	{
		[Test]
		public void Expression_01_Simple()
		{
			Test("onion",	"\"onion\"");	// string
			Test(1,			"1");           // integer
			Test(3.14,		"3.14");        // double
			Test(5.5f,		"5.5f");        // float
			Test(10,		"0xA");         // hex integer
			Test(-7,		"-7");          // should be parsed as true -7, not as -(7)
			Test(7, "+7");         
			Test(0.1,		".1");			// hope this won't cause problems
			Test(7u,		"7u");          // unsigned integer
		}

		[Test]
		public void Expression_02_AddMul()
		{
			Test(3,			"1+2");			// simple +
			Test(12,		"3*4");         // simple *
			Test(1.1,		"1+0.1");       // double vs. int
			Test(7,			"1+2*3");       // operator priority
			Test(8.5,		"1+2.5*3");     // mixed double and ints
			Test((long)2,	"1+1u");        // signed + unsigned integer
			Assert.AreEqual(ValueKind.Long, Result.Type, "not long");
			Test((ulong)2,	"1u+1");        // unsigned + signed integer
			Assert.AreEqual(ValueKind.ULong, Result.Type, "not ulong");
		}

		[Test]
		public void Expression_03_DivideAndNaN()
		{
			Test(12 / 5,	"12/5");        // integer division
			Test(12f / 5,	"12f/5");		// float division
			Test(null,		"0/0");         // undefined (althought JScript may return NaN)
			Assert.AreEqual(ValueKind.Undefined, Result.Type, "not undefined");
			Test(double.NaN, "0/.0");       // division by zero in float/double is NaN
			Assert.AreEqual(ValueKind.Double, Result.Type, "not double");
		}

		[Test]
		public void Expression_04_Variables()
		{
			Test(10,		"x = 2*3+4");   // assign
			Test(10,		"x");           // test
			Test("x10",		"\"x\" + x");   // string + integer
			Test(10,		"x++");         // post-increment
			Test(12,		"++x");         // pre-increment
			Test(12, "x--");         // post-decrement
			Test(10, "--x");         // pre-decrement
			Test(-10, "-x");         // additive inverse
			Test(10, "+x");          // for good measure
		}

		[Test]
		public void Expression_05_Properties()
		{
			Test("obj = new object");       // object creation
			Assert.AreEqual(ValueKind.Object, Result.Type, "not object type");
			Assert.IsNotNull(Result.Deref);

			Test("obj.x = 3.14");           // assign property (moreProps)
			Test(3.14,		"obj.x");       // test property

			Test("obj.s = \"xyz\"");
			Test("xyz",		"obj.s");
			Test(3,			"obj.s.length");// test internal property (baseProps)

			// obj.s needs to be boxed first - properties are otherwise lost
			Test(3.14,		"obj.s.bad = obj.x");
			Test(null,		"obj.s.bad");   // was assigned to boxed value and now lost

			Test("s = new string \"hello\"");// box
			Test("s.e = 2.7");              // preserved as s is boxed, not native string
			Test(2.7,		"s.e");         // test it (moreProps)
			Test(5,			"s.length");    // internal property (baseProps)

			// indexing is the same as accessing properties
			Test(2.7,		"s[\"e\"]");
			Test(5,			"s[\"length\"]");
			Test(3,			"obj[\"s\"][\"length\"]");
			Test(3,			"obj[\"s\", \"length\"]");
		}

		[Test]
		public void Expression_06_Compare()
		{
			Test(true,		"1 < 2");       // integers
			Test(true,		"1 > .1");      // integer vs. double - less
			Test(true,		"1 == 1.0");    // integer vs. double - equal
		}

		[Test]
		public void Expression_07_Ternary()
		{
			Test(true,		"true ? true : false");
			Test(false,		"false ? true : false");
			Test(2,			"0 != 0 ? 1 : 2");
		}
	}
}
