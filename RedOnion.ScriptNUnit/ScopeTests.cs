using System;
using System.Linq;
using NUnit.Framework;
using RedOnion.Script;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ROS_ScopeTests: StatementTestsBase
	{
		[SetUp]
		public void SetUp() // Repl mode so that we can overwrite functions
			=> Options = DefaultOptions | EngineOption.Repl;
		[TearDown]
		public void ResetEngine() => Reset();

		[Test]
		public void ROS_Scope01_Global()
		{
			Lines(1,
				"var x = 1",
				"function f",
				"	return x",
				"f()");

			// this = global
			Options = EngineOption.BlockScope | EngineOption.Repl;
			Lines(1,
				"var x = 1",
				"function f",
				"	return this.x",
				"f()");

			// this = undefined => null
			Options = EngineOption.BlockScope | EngineOption.Strict | EngineOption.Silent;
			Lines(null,
				"var x = 1",
				"function f",
				"	return this.x",
				"f()");

			Options = EngineOption.BlockScope | EngineOption.Strict | EngineOption.Repl;
			Expect<InvalidOperationException>(
				"var x = 1",
				"function f",
				"	return this.x",
				"f()");
			// just to test sctrict mode
			Expect<InvalidOperationException>(
				"y = 2");
			Options &= ~EngineOption.Repl;
			Expect<InvalidOperationException>(
				"function f",
				"	return");
		}

		[Test]
		public void ROS_Scope02_Local()
		{
			Lines(2,
				"var x = 1",
				"function f",
				"	var x = 2",
				"	return x",
				"f()");

			Lines(1,
				"var x = 1",
				"function f",
				"	var y = x",
				"	var x = 2",
				"	return y",
				"f()");
			/*
			JavaScript would actually return undefined
			because it moves all declarations to the top of the scopes
			effectively seeing it like this:

				var x
				x = 1
				function f
					var x,y
					y = x
					x = 2
					return y
			
			it is called hoisting
			*/
		}

		[Test]
		public void ROS_Scope03_Block()
		{
			var s =
				"var x = 1\r\n" +
				"function f\r\n" +
				"	if true\r\n" +
				"		var x = 2\r\n" +
				"	return x\r\n" +
				"f()";
			Options = EngineOption.None;
			Test(2, s);
			Options = EngineOption.BlockScope;
			Test(1, s);
		}

		[Test]
		public void ROS_Scope04_Closure()
		{
			Lines(2,
				"var x = 1",
				"function f",
				"	var x = 2",
				"	function g",
				"		return x",
				"	return g()",
				"f()");

			Lines(2,
				"var x = 1",
				"function f",
				"	var x = 2",
				"	function g",
				"		return x",
				"	return g",
				"f()()");
		}

		[Test]
		public void ROS_Scope05_Prototype()
		{
			Lines(1,
				"function f",
				"f.prototype = new object",
				"f.prototype.x = 1",
				"return (new f).x");
			Lines(2,
				"var x = 1",
				"function f",
				"	this.x = 2",
			//	"f.prototype = new object", // prototype is now autocreated when accessed
				"f.prototype.x = 3",
				"return (new f).x");
			Lines(2,
				"var x = 1",
				"function f",
				"	x = 2",
			//	"f.prototype = new object",
				"f.prototype.x = 3",
				"return (new f).x");
		}

		[Test]
		public void ROS_Scope06_SelfScope()
		{
			Lines(1,
				"var obj = new object",
				"obj.counter = 0",
				"function action",
				"	counter++",
				"obj.action = action",
				"obj.action",
				"return obj.counter");
		}

		[Test]
		public void ROS_Scope07_DocExample()
		{
			Test(1,
@"function MyClass
    // to allow MyClass() be used like new MyClass
    if this == null // or this === undefined
        return new MyClass

    // create some properties
    this.name = ""My Class""
    this.counter = 0

    // private properties
    var _total = 0

    // some method
    this.action = function
        counter++   // this.counter++
        _total++    // that private _total

    // read-only access to total
    this.getTotal = function
        return _total

var obj = new MyClass
obj.counter = 10
obj.action      // now obj.counter is 11
obj.getTotal    // returns 1
");
			Test(11, "obj.counter");
		}
	}
}
