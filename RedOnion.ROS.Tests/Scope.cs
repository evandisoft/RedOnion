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
		public void Reset() => ResetContext();

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
	}
}
