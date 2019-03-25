using System;
using System.Linq;
using NUnit.Framework;
using RedOnion.Script;

// return
// return a && b
// if cond then break
// if cond: v = true
// else v = false
// var x = 1
// var y = x
// var z int = x
// var i: uint
// var j as long = z
// while loop do loop = action()
// do stop = action()
// until stop
// do stop = action() until stop
// do stop = action(); until stop
// for var i = 0; i < 5; i++; action()
// try it(); catch: print exception; finally: fin()
// switch x: case 0: break; case 1: return; default: continue

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ROS_ParseStatementsTests : ParseTestsBase
	{
		public void Test(string value)
		{
			try
			{
				Unit(value);
			}
			catch (Exception e)
			{
				throw new Exception(String.Format("{0}: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, value), e);
			}
		}
		public void Lines(params string[] lines)
			=> Test(string.Join(Environment.NewLine, lines));

		[Test]
		public void ROS_PStts01_Return()
		{
			Test("return");
			CodeCheck(0, OpCode.Return);
			CodeCheck(1, OpCode.Undefined);
			CodeCheck(2);
		}

		[Test]
		public void ROS_PStts02_ReturnExpression()
		{
			Test("return a && b");
			CodeCheck( 0, OpCode.Return);
			CodeCheck( 1, OpCode.LogicAnd);
			CodeCheck( 2, OpCode.Identifier);
			CodeCheck( 3, 0, "a");
			CodeCheck( 7, 5); // size of second argument for short-circuit
			CodeCheck(11, OpCode.Identifier);
			CodeCheck(12, 1, "b");
			CodeCheck(16);
		}

		[Test]
		public void ROS_PStts03_IfThenBreak()
		{
			Test("if cond then break");
			CodeCheck( 0, OpCode.If);
			CodeCheck( 1, OpCode.Identifier);
			CodeCheck( 2, 0, "cond");
			CodeCheck( 6, 1); // size of then-block
			CodeCheck(10, OpCode.Break);
			CodeCheck(11);
		}

		[Test]
		public void ROS_PStts04_IfElse()
		{
			Test("if cond: v = true\nelse v = false");
			CodeCheck( 0, OpCode.If);
			CodeCheck( 1, OpCode.Identifier);
			CodeCheck( 2, 0, "cond");
			CodeCheck( 6, 7); // size of then-block
			CodeCheck(10, OpCode.Assign);
			CodeCheck(11, OpCode.Identifier);
			CodeCheck(12, 1, "v");
			CodeCheck(16, OpCode.True);
			CodeCheck(17, OpCode.Else);
			CodeCheck(18, 7); // size of else-block
			CodeCheck(22, OpCode.Assign);
			CodeCheck(23, OpCode.Identifier);
			CodeCheck(24, 1, "v"); // must not be duplicit in string table
			CodeCheck(28, OpCode.False);
			CodeCheck(29);
		}

		[Test]
		public void ROS_PStts05_Variables()
		{
			Test(
				"var x = 1\n" +
				"var y = x\n" +
				"var z int = x\n" +
				"var i: uint\n" +
				"var j as long = z");
			CodeCheck( 0, OpCode.Var);
			CodeCheck( 1, 0, "x");
			CodeCheck( 5, OpCode.Undefined);
			CodeCheck( 6, OpCode.Int);
			CodeCheck( 7, 1);
			CodeCheck(11, OpCode.Var);
			CodeCheck(12, 1, "y");
			CodeCheck(16, OpCode.Undefined);
			CodeCheck(17, OpCode.Identifier);
			CodeCheck(18, 0, "x");
			CodeCheck(22, OpCode.Var);
			CodeCheck(23, 2, "z");
			CodeCheck(27, OpCode.Int);
			CodeCheck(28, OpCode.Identifier);
			CodeCheck(29, 0, "x");
			CodeCheck(33, OpCode.Var);
			CodeCheck(34, 3, "i");
			CodeCheck(38, OpCode.UInt);
			CodeCheck(39, OpCode.Undefined);
			CodeCheck(40, OpCode.Var);
			CodeCheck(41, 4, "j");
			CodeCheck(45, OpCode.Long);
			CodeCheck(46, OpCode.Identifier);
			CodeCheck(47, 2, "z");
			CodeCheck(51);
		}

		[Test]
		public void ROS_PStts06_WhileDo()
		{
			Test("while loop do loop = action()");
			CodeCheck( 0, OpCode.While);
			CodeCheck( 1, OpCode.Identifier);
			CodeCheck( 2, 0, "loop");
			CodeCheck( 6, 12); // size of do-block
			CodeCheck(10, OpCode.Assign);
			CodeCheck(11, OpCode.Identifier);
			CodeCheck(12, 0, "loop");
			CodeCheck(16, OpCode.Call0);
			CodeCheck(17, OpCode.Identifier);
			CodeCheck(18, 1, "action");
			CodeCheck(22);
		}

		[Test]
		public void ROS_PStts07_DoUntil_v1()
		{
			Test("do stop = action()\nuntil stop");
			ROS_PStts07_DoUntil();
		}

		[Test]
		public void ROS_PStts07_DoUntil_v2()
		{
			Test("do stop = action() until stop");
			ROS_PStts07_DoUntil();
		}

		[Test]
		public void ROS_PStts07_DoUntil_v3()
		{
			Test("do stop = action(); until stop");
			ROS_PStts07_DoUntil();
		}

		public void ROS_PStts07_DoUntil()
		{
			CodeCheck( 0, OpCode.DoUntil);
			CodeCheck( 1, 12); // do-block size
			CodeCheck( 5, OpCode.Assign);
			CodeCheck( 6, OpCode.Identifier);
			CodeCheck( 7, 0, "stop");
			CodeCheck(11, OpCode.Call0);
			CodeCheck(12, OpCode.Identifier);
			CodeCheck(13, 1, "action");
			CodeCheck(17, OpCode.Identifier);
			CodeCheck(18, 0, "stop");
			CodeCheck(22);
		}

		[Test]
		public void ROS_PStts08_For()
		{
			Test("for var i = 0; i < 5; i++; action()");
			CodeCheck( 0, OpCode.For);
			CodeCheck( 1, OpCode.Var);
			CodeCheck( 2, 0, "i");
			CodeCheck( 6, OpCode.Undefined);
			CodeCheck( 7, OpCode.Int);
			CodeCheck( 8, 0);
			CodeCheck(12, OpCode.Less);
			CodeCheck(13, OpCode.Identifier);
			CodeCheck(14, 0, "i");
			CodeCheck(18, OpCode.Int);
			CodeCheck(19, 5);
			CodeCheck(23, 6); // loop-expr size
			CodeCheck(27, OpCode.PostInc);
			CodeCheck(28, OpCode.Identifier);
			CodeCheck(29, 0, "i");
			CodeCheck(33, 6); // do-block size
			CodeCheck(37, OpCode.Call0);
			CodeCheck(38, OpCode.Identifier);
			CodeCheck(39, 1, "action");
			CodeCheck(43);
		}

		[Test]
		public void ROS_PStts09_TryCatchFinally()
		{
			Test("try it(); catch: print exception; finally: fin()");
			CodeCheck( 0, OpCode.Try);
			CodeCheck( 1, 6); // try-block size
			CodeCheck( 5, OpCode.Call0);
			CodeCheck( 6, OpCode.Identifier);
			CodeCheck( 7, 0, "it");
			CodeCheck(11, 16); // size of all catch-blocks
			CodeCheck(15, -1); // exception var-name (empty)
			CodeCheck(19, OpCode.Undefined); // any exception
			CodeCheck(20, 7); // catch-block size
			CodeCheck(24, OpCode.Call1);
			CodeCheck(25, OpCode.Identifier);
			CodeCheck(26, 1, "print");
			CodeCheck(30, OpCode.Exception);
			CodeCheck(31, 6); // finally-block size
			CodeCheck(35, OpCode.Call0);
			CodeCheck(36, OpCode.Identifier);
			CodeCheck(37, 2, "fin");
			CodeCheck(41);
		}

		[Test]
		public void ROS_PStts10_SwitchCase()
		{
			Test("switch x: case 0: break; case 1: return; default: continue");
			CodeCheck( 0, OpCode.Switch);
			CodeCheck( 1, OpCode.Identifier);
			CodeCheck( 2, 0, "x");
			CodeCheck( 6, 27); // size of all case-blocks
			CodeCheck(10, OpCode.Int);
			CodeCheck(11, 0); // case 0
			CodeCheck(15, 1); // case 0 block size
			CodeCheck(19, OpCode.Break);
			CodeCheck(20, OpCode.Int);
			CodeCheck(21, 1); // case 1
			CodeCheck(25, 2); // case 1 block size
			CodeCheck(29, OpCode.Return);
			CodeCheck(30, OpCode.Undefined);
			CodeCheck(31, OpCode.Undefined); // default
			CodeCheck(32, 1); // default block size
			CodeCheck(36, OpCode.Continue);
			CodeCheck(37);
		}

		[Test]
		public void ROS_PStts11_Lambda()
		{
			Lines(
				"fn def e",
				"	print e",
				"print true");
			CodeCheck( 0, OpCode.Call1);
			CodeCheck( 1, OpCode.Identifier);
			CodeCheck( 2, 2, "fn");
			CodeCheck( 6, OpCode.Function);
			CodeCheck( 7, 23);
			CodeCheck(14, (byte)1); // one arg (e)
			CodeCheck(15, 1); // undefined return type
			CodeCheck(19, OpCode.Undefined);
			CodeCheck(20, 0, "e");
			CodeCheck(24, 1);
			CodeCheck(28, OpCode.Undefined); // no type
			CodeCheck(29, 1);
			CodeCheck(33, OpCode.Undefined); // no default
			CodeCheck(34, 11); // block size
			CodeCheck(38, OpCode.Call1);
			CodeCheck(39, OpCode.Identifier);
			CodeCheck(40, 1, "print");
			CodeCheck(44, OpCode.Identifier);
			CodeCheck(45, 0, "e");
			// important: 49-38 = 11 - the block size @34
			CodeCheck(49, OpCode.Call1);
			CodeCheck(50, OpCode.Identifier);
			CodeCheck(51, 1, "print");
			CodeCheck(55, OpCode.True);
			CodeCheck(56);

			Reset();
			Lines(
				"fn def e => print e",
				"print true");
			CodeCheck(0, OpCode.Call1);
			CodeCheck(1, OpCode.Identifier);
			CodeCheck(2, 2, "fn");
			CodeCheck(6, OpCode.Function);
			CodeCheck(7, 23);
			CodeCheck(14, (byte)1); // one arg (e)
			CodeCheck(15, 1); // undefined return type
			CodeCheck(19, OpCode.Undefined);
			CodeCheck(20, 0, "e");
			CodeCheck(24, 1);
			CodeCheck(28, OpCode.Undefined); // no type
			CodeCheck(29, 1);
			CodeCheck(33, OpCode.Undefined); // no default
			CodeCheck(34, 12); // block size
			CodeCheck(38, OpCode.Return); // lambda uses return
			CodeCheck(39, OpCode.Call1);
			CodeCheck(40, OpCode.Identifier);
			CodeCheck(41, 1, "print");
			CodeCheck(45, OpCode.Identifier);
			CodeCheck(46, 0, "e");
			// important: 50-38 = 12 - the block size @34
			CodeCheck(50, OpCode.Call1);
			CodeCheck(51, OpCode.Identifier);
			CodeCheck(52, 1, "print");
			CodeCheck(56, OpCode.True);
			CodeCheck(57);
		}
	}
}
