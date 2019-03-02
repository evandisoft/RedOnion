using Bee.Run;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Linq;

namespace Bee.Tests
{
	[TestClass]
	public class BeeOpcodeTests
	{
		[TestMethod]
		public void OC01_extend()
		{
			Assert.AreEqual(Opcode.Undef, (Opcode)0);
			Assert.AreEqual(Opcode.Undef, ((Opcode)0).Extend());
			Assert.AreEqual(Opcode.Byte, ((Opcode)Opcode.Byte.Code()).Extend());
			Assert.AreEqual(Opcode.Double, ((Opcode)Opcode.Double.Code()).Extend());
			Assert.AreEqual(Opcode.Create, ((Opcode)Opcode.Create.Code()).Extend());
			Assert.AreEqual(Opcode.Assign, ((Opcode)Opcode.Assign.Code()).Extend());
			Assert.AreEqual(Opcode.BitOr, ((Opcode)Opcode.BitOr.Code()).Extend());
			Assert.AreEqual(Opcode.BitXor, ((Opcode)Opcode.BitXor.Code()).Extend());
			Assert.AreEqual(Opcode.BitAnd, ((Opcode)Opcode.BitAnd.Code()).Extend());
			Assert.AreEqual(Opcode.Add, ((Opcode)Opcode.Add.Code()).Extend());
			Assert.AreEqual(Opcode.Mul, ((Opcode)Opcode.Mul.Code()).Extend());
			Assert.AreEqual(Opcode.LogicOr, ((Opcode)Opcode.LogicOr.Code()).Extend());
			Assert.AreEqual(Opcode.LogicAnd, ((Opcode)Opcode.LogicAnd.Code()).Extend());
			Assert.AreEqual(Opcode.Plus, ((Opcode)Opcode.Plus.Code()).Extend());
			Assert.AreEqual(Opcode.Neg, ((Opcode)Opcode.Neg.Code()).Extend());
			Assert.AreEqual(Opcode.PostInc, ((Opcode)Opcode.PostInc.Code()).Extend());
		}
		
		[TestMethod]
		public void OC02_text()
		{
			Assert.AreEqual("undefined", Opcode.Undef.Text());
			Assert.AreEqual("=", Opcode.Assign.Text());
			Assert.AreEqual("*", Opcode.Mul.Text());
			Assert.AreEqual("+", Opcode.Plus.Text());
			Assert.AreEqual("for", Opcode.For.Text());
			Assert.AreEqual("if", Opcode.If.Text());
			Assert.AreEqual("else", Opcode.Else.Text());
			Assert.AreEqual("public", Opcode.Public.Text());
			Assert.AreEqual("virtual", Opcode.Virtual.Text());
			Assert.AreEqual("class", Opcode.Class.Text());
			Assert.AreEqual("interface", Opcode.Face.Text());
			Assert.IsNull(((Opcode)0xFFFF).Text());
		}
		
		[TestMethod]
		public void OC03_priority()
		{
			Assert.IsTrue(Opcode.Mul.Prior() > Opcode.Add.Prior());
			Assert.IsTrue(Opcode.Sub.Prior() > Opcode.ShiftRight.Prior());
			Assert.IsTrue(Opcode.As.Prior() > Opcode.Equals.Prior());
			Assert.IsTrue(Opcode.LogicAnd.Prior() > Opcode.LogicOr.Prior());
			Assert.IsTrue(Opcode.Less.Prior() > Opcode.Differ.Prior());
			Assert.IsTrue(Opcode.BitOr.Prior() > Opcode.Equals.Prior());
			Assert.IsTrue(Opcode.BitOr.Prior() < Opcode.ShiftLeft.Prior());
			Assert.IsTrue(Opcode.BitAnd.Prior() > Opcode.Mul.Prior());
			Assert.IsTrue(Opcode.BitAnd.Cprior() < Opcode.Equals.Cprior());
			Assert.IsTrue(Opcode.BitOr.Cprior() < Opcode.BitAnd.Cprior());
		}
		
		[TestMethod]
		public void OC04_reflection()
		{
			var flags = true;
			var prev = Opcode.Undef;
			foreach (FieldInfo fi in (typeof(Opcode)).GetFields().OrderBy(x => { return x.MetadataToken; }))
			{
				if (!fi.IsStatic)
				{
					continue;
				}
				var op = (Opcode)fi.GetValue(null);
				if (flags)
				{
					if (op != Opcode.Undef)
					{
						continue;
					}
					flags = false;
				}
				if (unchecked((byte)op) != unchecked((byte)prev))
				{
					Assert.AreEqual(op.Extend(), op, "Op: {0:X4} - extend failed", (ushort)op);
					if (unchecked((byte)op) >= 0x80 && unchecked((byte)op) != unchecked((byte)prev))
					{
						Assert.IsTrue(unchecked((byte)op) == ((ushort)op), "Op: {0:X4} - nonzero high part", (ushort)op);
					}
				}
				prev = op;
			}
		}
	}
}
