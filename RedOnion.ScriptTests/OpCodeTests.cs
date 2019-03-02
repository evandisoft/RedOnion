using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedOnion.Script.Execution;

namespace RedOnion.ScriptTests
{
	[TestClass]
	public class OpCodeTests
	{
		[TestMethod]
		public void OpCode_01_BasicExtend()
		{
			Assert.AreEqual(OpCode.Undefined, (OpCode)0);
			Assert.AreEqual(OpCode.Undefined, ((OpCode)0).Extend());
			Assert.AreEqual(OpCode.Byte, ((OpCode)OpCode.Byte.Code()).Extend());
			Assert.AreEqual(OpCode.Double, ((OpCode)OpCode.Double.Code()).Extend());
			Assert.AreEqual(OpCode.Create, ((OpCode)OpCode.Create.Code()).Extend());
			Assert.AreEqual(OpCode.Assign, ((OpCode)OpCode.Assign.Code()).Extend());
			Assert.AreEqual(OpCode.BitOr, ((OpCode)OpCode.BitOr.Code()).Extend());
			Assert.AreEqual(OpCode.BitXor, ((OpCode)OpCode.BitXor.Code()).Extend());
			Assert.AreEqual(OpCode.BitAnd, ((OpCode)OpCode.BitAnd.Code()).Extend());
			Assert.AreEqual(OpCode.Add, ((OpCode)OpCode.Add.Code()).Extend());
			Assert.AreEqual(OpCode.Mul, ((OpCode)OpCode.Mul.Code()).Extend());
			Assert.AreEqual(OpCode.LogicOr, ((OpCode)OpCode.LogicOr.Code()).Extend());
			Assert.AreEqual(OpCode.LogicAnd, ((OpCode)OpCode.LogicAnd.Code()).Extend());
			Assert.AreEqual(OpCode.Plus, ((OpCode)OpCode.Plus.Code()).Extend());
			Assert.AreEqual(OpCode.Neg, ((OpCode)OpCode.Neg.Code()).Extend());
			Assert.AreEqual(OpCode.PostInc, ((OpCode)OpCode.PostInc.Code()).Extend());
		}

		[TestMethod]
		public void OpCode_02_BasicText()
		{
			Assert.AreEqual("undefined", OpCode.Undefined.Text());
			Assert.AreEqual("=", OpCode.Assign.Text());
			Assert.AreEqual("*", OpCode.Mul.Text());
			Assert.AreEqual("+", OpCode.Plus.Text());
			Assert.AreEqual("for", OpCode.For.Text());
			Assert.AreEqual("if", OpCode.If.Text());
			Assert.AreEqual("else", OpCode.Else.Text());
			Assert.AreEqual("public", OpCode.Public.Text());
			Assert.AreEqual("virtual", OpCode.Virtual.Text());
			Assert.AreEqual("class", OpCode.Class.Text());
			Assert.AreEqual("interface", OpCode.Interface.Text());
			Assert.IsNull(((OpCode)0xFFFF).Text());
		}

		[TestMethod]
		public void OpCode_03_Priority()
		{
			Assert.IsTrue(OpCode.Mul.Priority() > OpCode.Add.Priority());
			Assert.IsTrue(OpCode.Sub.Priority() > OpCode.ShiftRight.Priority());
			Assert.IsTrue(OpCode.As.Priority() > OpCode.Equals.Priority());
			Assert.IsTrue(OpCode.LogicAnd.Priority() > OpCode.LogicOr.Priority());
			Assert.IsTrue(OpCode.Less.Priority() > OpCode.Differ.Priority());
			Assert.IsTrue(OpCode.BitOr.Priority() > OpCode.Equals.Priority());
			Assert.IsTrue(OpCode.BitOr.StdPriority() < OpCode.Equals.StdPriority());
			Assert.IsTrue(OpCode.BitOr.Priority() < OpCode.ShiftLeft.Priority());
			Assert.IsTrue(OpCode.BitOr.StdPriority() < OpCode.ShiftLeft.StdPriority());
		}

		[TestMethod]
		public void OpCode_04_TableTest()
		{
			var flags = true;
			var prev = OpCode.Undefined;
			foreach (FieldInfo fi in typeof(OpCode).GetFields().OrderBy(x => x.MetadataToken))
			{
				if (!fi.IsStatic)
					continue;
				var op = (OpCode)fi.GetValue(null);
				if (flags)
				{
					if (op != OpCode.Undefined)
						continue;
					flags = false;
				}
				if ((byte)op != (byte)prev)
				{
					Assert.AreEqual(op.Extend(), op, "Op: {0:X4} - extend failed", (ushort)op);
					if ((byte)op >= 0x80)
						Assert.IsTrue((byte)op == (ushort)op, "Op: {0:X4} - nonzero high part", (ushort)op);
				}
				prev = op;
			}
		}
	}
}
