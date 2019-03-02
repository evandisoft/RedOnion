using System;
using System.Linq;
using System.Reflection;
using Xunit;
using RedOnion.Script.Execution;

namespace RedOnion.ScriptTests
{
	public class OpCodeTests
	{
		[Fact]
		public void OpCode_01_BasicExtend()
		{
			Assert.Equal(OpCode.Undefined, (OpCode)0);
			Assert.Equal(OpCode.Undefined, ((OpCode)0).Extend());
			Assert.Equal(OpCode.Byte, ((OpCode)OpCode.Byte.Code()).Extend());
			Assert.Equal(OpCode.Double, ((OpCode)OpCode.Double.Code()).Extend());
			Assert.Equal(OpCode.Create, ((OpCode)OpCode.Create.Code()).Extend());
			Assert.Equal(OpCode.Assign, ((OpCode)OpCode.Assign.Code()).Extend());
			Assert.Equal(OpCode.BitOr, ((OpCode)OpCode.BitOr.Code()).Extend());
			Assert.Equal(OpCode.BitXor, ((OpCode)OpCode.BitXor.Code()).Extend());
			Assert.Equal(OpCode.BitAnd, ((OpCode)OpCode.BitAnd.Code()).Extend());
			Assert.Equal(OpCode.Add, ((OpCode)OpCode.Add.Code()).Extend());
			Assert.Equal(OpCode.Mul, ((OpCode)OpCode.Mul.Code()).Extend());
			Assert.Equal(OpCode.LogicOr, ((OpCode)OpCode.LogicOr.Code()).Extend());
			Assert.Equal(OpCode.LogicAnd, ((OpCode)OpCode.LogicAnd.Code()).Extend());
			Assert.Equal(OpCode.Plus, ((OpCode)OpCode.Plus.Code()).Extend());
			Assert.Equal(OpCode.Neg, ((OpCode)OpCode.Neg.Code()).Extend());
			Assert.Equal(OpCode.PostInc, ((OpCode)OpCode.PostInc.Code()).Extend());
		}

		[Fact]
		public void OpCode_02_BasicText()
		{
			Assert.Equal("undefined", OpCode.Undefined.Text());
			Assert.Equal("=", OpCode.Assign.Text());
			Assert.Equal("*", OpCode.Mul.Text());
			Assert.Equal("+", OpCode.Plus.Text());
			Assert.Equal("for", OpCode.For.Text());
			Assert.Equal("if", OpCode.If.Text());
			Assert.Equal("else", OpCode.Else.Text());
			Assert.Equal("public", OpCode.Public.Text());
			Assert.Equal("virtual", OpCode.Virtual.Text());
			Assert.Equal("class", OpCode.Class.Text());
			Assert.Equal("interface", OpCode.Interface.Text());
			Assert.Null(((OpCode)0xFFFF).Text());
		}

		[Fact]
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

		[Fact]
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
					Assert.Equal(op.Extend(), op);
					if ((byte)op >= 0x80)
						Assert.True((byte)op == (ushort)op);
				}
				prev = op;
			}
		}
	}
}
