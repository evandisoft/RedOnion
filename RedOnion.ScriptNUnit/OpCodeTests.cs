using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RedOnion.Script;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class OpCodeTests
	{
		[Test]
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

		[Test]
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
			Assert.Null(((OpCode)0xFFFF).Text());
		}

		[Test]
		public void OpCode_03_Priority()
		{
			Assert.True(OpCode.Mul.Priority() > OpCode.Add.Priority());
			Assert.True(OpCode.Sub.Priority() > OpCode.ShiftRight.Priority());
			Assert.True(OpCode.As.Priority() > OpCode.Equals.Priority());
			Assert.True(OpCode.LogicAnd.Priority() > OpCode.LogicOr.Priority());
			Assert.True(OpCode.Less.Priority() > OpCode.Differ.Priority());
			Assert.True(OpCode.BitOr.Priority() > OpCode.Equals.Priority());
			Assert.True(OpCode.BitOr.StdPriority() < OpCode.Equals.StdPriority());
			Assert.True(OpCode.BitOr.Priority() < OpCode.ShiftLeft.Priority());
			Assert.True(OpCode.BitOr.StdPriority() < OpCode.ShiftLeft.StdPriority());
		}

		[Test]
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
					Assert.AreEqual(op.Extend(), op);
					if ((byte)op >= 0x80)
						Assert.True((byte)op == (ushort)op);
				}
				prev = op;
			}
		}
	}
}
