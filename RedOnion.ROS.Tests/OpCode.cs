using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace RedOnion.ROS.Tests
{
	[TestFixture]
	public class ROS_OpCode
	{
		[Test]
		public void ROS_OpCode01_BasicExtend()
		{
			Assert.AreEqual(ExCode.Void, (ExCode)0);
			Assert.AreEqual(ExCode.Void, ((OpCode)0).Extend());
			Assert.AreEqual(ExCode.Byte, ((OpCode)ExCode.Byte.Code()).Extend());
			Assert.AreEqual(ExCode.Double, ((OpCode)ExCode.Double.Code()).Extend());
			Assert.AreEqual(ExCode.Create, ((OpCode)ExCode.Create.Code()).Extend());
			Assert.AreEqual(ExCode.Assign, ((OpCode)ExCode.Assign.Code()).Extend());
			Assert.AreEqual(ExCode.BitOr, ((OpCode)ExCode.BitOr.Code()).Extend());
			Assert.AreEqual(ExCode.BitXor, ((OpCode)ExCode.BitXor.Code()).Extend());
			Assert.AreEqual(ExCode.BitAnd, ((OpCode)ExCode.BitAnd.Code()).Extend());
			Assert.AreEqual(ExCode.Add, ((OpCode)ExCode.Add.Code()).Extend());
			Assert.AreEqual(ExCode.Mul, ((OpCode)ExCode.Mul.Code()).Extend());
			Assert.AreEqual(ExCode.LogicOr, ((OpCode)ExCode.LogicOr.Code()).Extend());
			Assert.AreEqual(ExCode.LogicAnd, ((OpCode)ExCode.LogicAnd.Code()).Extend());
			Assert.AreEqual(ExCode.Plus, ((OpCode)ExCode.Plus.Code()).Extend());
			Assert.AreEqual(ExCode.Neg, ((OpCode)ExCode.Neg.Code()).Extend());
			Assert.AreEqual(ExCode.PostInc, ((OpCode)ExCode.PostInc.Code()).Extend());
		}

		[Test]
		public void ROS_OpCode02_BasicText()
		{
			Assert.AreEqual("void", ExCode.Void.Text());
			Assert.AreEqual("=", ExCode.Assign.Text());
			Assert.AreEqual("*", ExCode.Mul.Text());
			Assert.AreEqual("+", ExCode.Plus.Text());
			Assert.AreEqual("for", ExCode.For.Text());
			Assert.AreEqual("if", ExCode.If.Text());
			Assert.AreEqual("else", ExCode.Else.Text());
			Assert.AreEqual("public", ExCode.Public.Text());
			Assert.AreEqual("virtual", ExCode.Virtual.Text());
			Assert.AreEqual("class", ExCode.Class.Text());
			Assert.AreEqual("interface", ExCode.Interface.Text());
			Assert.Null(((ExCode)0xFFFF).Text());
		}

		[Test]
		public void ROS_OpCode03_Priority()
		{
			Assert.True(ExCode.Mul.Priority() > ExCode.Add.Priority());
			Assert.True(ExCode.Sub.Priority() > ExCode.ShiftRight.Priority());
			Assert.True(ExCode.As.Priority() > ExCode.Equals.Priority());
			Assert.True(ExCode.LogicAnd.Priority() > ExCode.LogicOr.Priority());
			Assert.True(ExCode.Less.Priority() > ExCode.Differ.Priority());
			Assert.True(ExCode.BitOr.Priority() > ExCode.Equals.Priority());
			Assert.True(ExCode.BitOr.StdPriority() < ExCode.Equals.StdPriority());
			Assert.True(ExCode.BitOr.Priority() < ExCode.ShiftLeft.Priority());
			Assert.True(ExCode.BitOr.StdPriority() < ExCode.ShiftLeft.StdPriority());
		}

		[Test]
		public void ROS_OpCode04_TableTest()
		{
			foreach (var fi in typeof(OpCode).GetFields().OrderBy(x => x.MetadataToken))
			{
				if (!fi.IsStatic)
					continue;
				var op = (OpCode)fi.GetValue(null);
				var ex = (ExCode)typeof(ExCode).GetField(fi.Name).GetValue(null);
				Assert.AreEqual(op.Code(), ex.Code());
				Assert.AreEqual(op.Extend(), ex);
			}

			var flags = true;
			var prev = ExCode.Void;
			foreach (var fi in typeof(ExCode).GetFields().OrderBy(x => x.MetadataToken))
			{
				if (!fi.IsStatic)
					continue;
				var op = (ExCode)fi.GetValue(null);
				if (flags)
				{
					if (op != ExCode.Void)
						continue;
					flags = false;
				}
				if ((byte)op != (byte)prev)
				{
					Assert.AreEqual(((OpCode)op).Extend(), op);
					if ((byte)op >= 0x80)
						Assert.True((byte)op == (ushort)op);
				}
				prev = op;
			}
		}
	}
}
