using Bee.Run;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bee.Tests
{
	[TestClass]
	public class BeeStatementCodeTests: BeeCodeTestsBase
	{
		public void Test(string @string)
		{
			Unit(@string);
		}
		
		[TestMethod]
		public void CS01_return()
		{
			Test("return");
			Check(0, unchecked((byte)Opcode.Return));
			Check(1, unchecked((byte)Opcode.Undef));
			Check(2);
		}
		
		[TestMethod]
		public void CS02_return_expr()
		{
			Test("return a && b");
			Check(0, unchecked((byte)Opcode.Return));
			Check(1, unchecked((byte)Opcode.LogicAnd));
			Check(2, unchecked((byte)Opcode.Ident));
			Check(3, unchecked((byte)1));
			Check(4, unchecked((byte)'a'));
			Check(5, 3);
			Check(9, unchecked((byte)Opcode.Ident));
			Check(10, unchecked((byte)1));
			Check(11, unchecked((byte)'b'));
			Check(12);
		}
		
		[TestMethod]
		public void CS03_ifbreak()
		{
			Test("if cond then break");
			Check(0, unchecked((byte)Opcode.If));
			Check(1, unchecked((byte)Opcode.Ident));
			Check(2, unchecked((byte)4));
			Check(7, 1);
			Check(11, unchecked((byte)Opcode.Break));
			Check(12);
		}
		
		[TestMethod]
		public void CS04_ifelse()
		{
			Test("if cond: v = true\nelse v = false");
			Check(0, unchecked((byte)Opcode.If));
			Check(1, unchecked((byte)Opcode.Ident));
			Check(2, unchecked((byte)4));
			Check(7, 5);
			Check(11, unchecked((byte)Opcode.Assign));
			Check(12, unchecked((byte)Opcode.Ident));
			Check(13, unchecked((byte)1));
			Check(14, unchecked((byte)'v'));
			Check(15, unchecked((byte)Opcode.True));
			Check(16, unchecked((byte)Opcode.Else));
			Check(17, 5);
			Check(21, unchecked((byte)Opcode.Assign));
			Check(22, unchecked((byte)Opcode.Ident));
			Check(23, unchecked((byte)1));
			Check(24, unchecked((byte)'v'));
			Check(25, unchecked((byte)Opcode.False));
			Check(26);
		}
		
		[TestMethod]
		public void CS05_var()
		{
			Test((("var x = 1\n" + "var y = x\n") + "var z int = x\n") + "var i: int");
			Check(0, unchecked((byte)Opcode.Var));
			Check(1, unchecked((byte)1));
			Check(2, unchecked((byte)'x'));
			Check(3, unchecked((byte)Opcode.Undef));
			Check(4, unchecked((byte)Opcode.Int));
			Check(5, 1);
			Check(9, unchecked((byte)Opcode.Var));
			Check(10, unchecked((byte)1));
			Check(11, unchecked((byte)'y'));
			Check(12, unchecked((byte)Opcode.Undef));
			Check(13, unchecked((byte)Opcode.Ident));
			Check(14, unchecked((byte)1));
			Check(15, unchecked((byte)'x'));
			Check(16, unchecked((byte)Opcode.Var));
			Check(17, unchecked((byte)1));
			Check(18, unchecked((byte)'z'));
			Check(19, unchecked((byte)Opcode.Int));
			Check(20, unchecked((byte)Opcode.Ident));
			Check(21, unchecked((byte)1));
			Check(22, unchecked((byte)'x'));
			Check(23, unchecked((byte)Opcode.Var));
			Check(24, unchecked((byte)1));
			Check(25, unchecked((byte)'i'));
			Check(26, unchecked((byte)Opcode.Int));
			Check(27, unchecked((byte)Opcode.Undef));
			Check(28);
		}
		
		[TestMethod]
		public void CS06_while()
		{
			Test("while loop do loop = action()");
			Check(0, unchecked((byte)Opcode.While));
			Check(1, unchecked((byte)Opcode.Ident));
			Check(2, unchecked((byte)4));
			Check(11, unchecked((byte)Opcode.Assign));
			Check(12, unchecked((byte)Opcode.Ident));
			Check(18, unchecked((byte)Opcode.Ecall));
			Check(19, unchecked((byte)Opcode.Ident));
			Check(20, unchecked((byte)6));
			Check(27);
		}
		
		[TestMethod]
		public void CS07_dountil_v1()
		{
			Test("do again = action()\nuntil again");
			CS07_dountil_code();
		}
		
		[TestMethod]
		public void CS07_dountil_v2()
		{
			Test("do again = action() until again");
			CS07_dountil_code();
		}
		
		[TestMethod]
		public void CS07_dountil_v3()
		{
			Test("do again = action(); until again");
			CS07_dountil_code();
		}
		
		public void CS07_dountil_code()
		{
			Check(0, unchecked((byte)Opcode.Dountil));
			Check(5, unchecked((byte)Opcode.Assign));
			Check(6, unchecked((byte)Opcode.Ident));
			Check(7, unchecked((byte)5));
			Check(13, unchecked((byte)Opcode.Ecall));
			Check(14, unchecked((byte)Opcode.Ident));
			Check(15, unchecked((byte)6));
			Check(22, unchecked((byte)Opcode.Ident));
			Check(23, unchecked((byte)5));
			Check(29);
		}
		
		[TestMethod]
		public void CS08_for()
		{
			Test("for var i = 0; i < 5; i++; loop()");
			Check(0, unchecked((byte)Opcode.For));
			Check(1, unchecked((byte)Opcode.Var));
			Check(2, unchecked((byte)1));
			Check(3, unchecked((byte)'i'));
			Check(4, unchecked((byte)Opcode.Undef));
			Check(5, unchecked((byte)Opcode.Int));
			Check(6, 0);
			Check(10, unchecked((byte)Opcode.Less));
			Check(11, unchecked((byte)Opcode.Ident));
			Check(12, unchecked((byte)1));
			Check(13, unchecked((byte)'i'));
			Check(14, unchecked((byte)Opcode.Int));
			Check(15, 5);
			Check(23, unchecked((byte)Opcode.PostInc));
			Check(24, unchecked((byte)Opcode.Ident));
			Check(25, unchecked((byte)1));
			Check(26, unchecked((byte)'i'));
			Check(19, 4);
			Check(31, unchecked((byte)Opcode.Ecall));
			Check(32, unchecked((byte)Opcode.Ident));
			Check(38);
		}
		
		[TestMethod]
		public void CS09_catch()
		{
			Test("try it(); catch: print exception; finally: fin()");
			Check(0, unchecked((byte)Opcode.Catch));
			Check(5, unchecked((byte)Opcode.Ecall));
			Check(6, unchecked((byte)Opcode.Ident));
			Check(7, unchecked((byte)2));
			Check(14, unchecked((byte)Opcode.Undef));
			Check(15, unchecked((byte)0));
			Check(20, unchecked((byte)Opcode.Call));
			Check(21, unchecked((byte)Opcode.Ident));
			Check(22, unchecked((byte)5));
			Check(28, unchecked((byte)Opcode.Exception));
			Check(33, unchecked((byte)Opcode.Ecall));
			Check(34, unchecked((byte)Opcode.Ident));
			Check(35, unchecked((byte)3));
			Check(39);
		}
		
		[TestMethod]
		public void CS10_switch()
		{
			Test("switch x: case 0: break; case 1: return; default: continue");
			Check(0, unchecked((byte)Opcode.Switch));
			Check(1, unchecked((byte)Opcode.Ident));
			Check(2, unchecked((byte)1));
			Check(3, unchecked((byte)'x'));
			Check(8, unchecked((byte)Opcode.Int));
			Check(9, 0);
			Check(17, unchecked((byte)Opcode.Break));
		}
	}
}
