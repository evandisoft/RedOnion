using System;
using System.Collections;
using NUnit.Framework;
using RedOnion.ROS.Objects;

namespace RedOnion.ROS.Tests
{
	[TestFixture]
	public class ROS_Scope : StatementTests
	{
		[TearDown]
		public override void Reset() => base.Reset();

		[Test]
		public void ROS_Scope01_Global()
		{
			Lines(1,
				"var x = 1",
				"function f",
				"  return x",
				"f()");
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

			Reset();
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
			Lines(1,
				"var x = 1",
				"function f",
				"	if true",
				"		var x = 2",		// this is hidden for the return
				"	return x",			// global x
				"f");
		}

		[Test]
		public void ROS_Scope04_Arguments()
		{
			Lines(3,
				"function f a,b",
				"  return arguments.length",
				"f 1,2,3");
			Lines(2,
				"function f",
				"  return arguments[0]",
				"f 2");
			/* TODO: default arguments
			Lines(2,
				"def second a = 1, b = 2 => b",
				"second 1");
			*/
		}

		[Test]
		public void ROS_Scope05_Prototype()
		{
			Lines(1,
				"function f",
				"f.prototype.x = 1",
				"return (new f).x");	// prototype.x

			Reset();
			Lines(2,
				"var x = 1",
				"function f",
				"  this.x = 2",
				"f.prototype.x = 3",
				"return (new f).x");	// this.x

			Reset();
			Lines(3,
				"var x = 1",
				"function f",
				"  x = 2",				// global x
				"f.prototype.x = 3",
				"return (new f).x");    // prototype.x

			// it is questionable whether `x` inside `f`
			// shoud be `this.x` or global `x` here,
			// but we want functions to see each other
			// regardless of the position in the scope
			// and therefore have the same behaviour
			// for variables as well (`f` sees global `x`
			// which has precedence over `this.x`)
			Reset();
			Lines(3,
				"function f",
				"  x = 2",				// global x !!
				"var x = 1",
				"f.prototype.x = 3",
				"return (new f).x");	// this.x
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
		public void ROS_Scope07_Closure()
		{
			Lines(1,
				"var x = 1",
				"function f",
				"  return x",			// global x
				"f()");

			Reset();
			Lines(2,
				"var x = 1",
				"function f",
				"	var x = 2",
				"	function g",
				"		return x",		// captured var x = 2 from f
				"	return g()",
				"f()");

			Reset();
			Lines(3,
				"var x = 1",
				"function f",
				"	var x = 2",
				"	function g",
				"		return x + 1",	// captured var x = 2 from f
				"	return g",
				"f()()");

			Reset();
			Lines(4,
				"function f x",
				"  function g",
				"    return x + 2",		// captured argument of f
				"  return g()",
				"f 2");

			Reset();
			Lines(5,
				"function f x",
				"  function g",
				"    function h x",
				"      return x + 2",	// argument of h (ignore argument of f)
				"    return h",
				"  return g()",
				"(f 1)(3)");

			Reset();
			Lines(1,
				"def test",
				"  return def",			// return lambda
				"    return 1",
				"return test()()");

			Reset();
			Lines(1,
				"var x = 0",
				"def set v",
				"  x = v",              // function without return (see Core.Execute(int) .. while (at == blockEnd) .. case OpCode.Function)
				"def test",
				"  if x == 0; set 1",	// captured function
				"  else set x+1",
				"test",
				"return x");
			var test = Value.Void;
			Assert.IsTrue(ctx.Get(ref test, ctx.Find("test")));
			Execute(test.obj as Function);
			var xval = Value.Void;
			Assert.IsTrue(ctx.Get(ref xval, ctx.Find("x")));
			Assert.IsTrue(xval.IsInt);
			Assert.AreEqual(2, xval.ToInt());
			Execute(test.obj as Function);
			Assert.IsTrue(ctx.Get(ref xval, ctx.Find("x")));
			Assert.IsTrue(xval.IsInt);
			Assert.AreEqual(3, xval.ToInt());
		}

		[Test]
		public void ROS_Scope08_ClosureModify()
		{
			Lines(3,
				"var x = 0",
				"def inc => ++x",
				"def add y => x += y",
				"inc",
				"add 2");

			/* TODO: create new OpCode.Local and make parser decide the index
			 * TODO: multiple cousins for closures in loop
			
			! Although the following appears to work, I am not sure why, it should not
			! because all the closures should now share single context `var j=i` does not matter
			Reset();
			Lines("123",
				"var s = \"\"",
				"var a = new list",
				"for var i = 0; i < 3; i++",
				"  a.add def f",
				"    var j = i",
				"    s += j + 1",
				"for var f in a; f",
				"return s");

			Reset();
			Lines(6,
				"var sum = 0",
				"var vals = [1, 2, 3]",
				"var fns = new list",
				"var index = 0",
				"while index < vals.length",
				"  var value = vals[index++]",
				"  fns.add def => sum += value",
				"do fns[--index]() while index > 0",
				"return sum");
			*/
		}

		[Test]
		public void ROS_Scope09_DocExample()
		{
			Test(1, @"
def MyClass
    // to allow MyClass() be used like new MyClass
    if this == null
        return new MyClass

    // create some properties
    this.name = ""My Class""
    this.counter = 0

    // private properties
    var _total = 0

    // some method
    this.action = def
        counter++   // this.counter++
        _total++    // that private _total

    // read-only access to total
    this.getTotal = def => _total

var obj = new MyClass
obj.counter = 10
obj.action      // now obj.counter is 11
obj.getTotal    // returns 1
");
			Test(11, "obj.counter");
		}

		[Test]
		public void ROS_Scope10_Run()
		{
			if (Globals == null)
				Globals = new Globals();
			Globals.Add("test", "var x = 3.14; return x");
			Test(3.14, "var x = 2.72; run.source test");
			Test(2.72, "x");
			Test(3.14, "run.library.source test");
			Test(3.14, "x");
			Globals.Add("test2", "var x = 1.41");
			Test(ExitCode.Return, 1.41, "run.library.source test2; return x");
		}

		[Test]
		public void ROS_Scope11_Events()
		{
			YieldLines(ExitCode.Return, 3,
				"var x = 0",
				"def update => x++",
				"system.update.add update",
				"wait; wait; wait",
				"system.update.remove update",
				"return x");

			Reset();
			YieldLines(ExitCode.Return, 7,
				"var x = 0",
				"def add y => x += y",
				"def update => add x+1",
				"system.update.add update",
				"wait; wait; wait",
				"system.update.remove update",
				"return x");
		}
	}
}
