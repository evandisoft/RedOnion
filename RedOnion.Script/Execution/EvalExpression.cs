using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	public partial class Engine
	{
		protected virtual void TypeReference(ref int at)
		{
			var op = ((OpCode)Code[at++]).Extend();
			if (op.Kind() <= OpKind.Number)
			{
				if (op == OpCode.Null || (byte)op >= OpCode.String.Code())
				{
					Value = Root.Get(op);
					return;
				}
				if (op == OpCode.Identifier)
				{
					Value = Ctx.Get(Strings[CodeInt(ref at)]);
					return;
				}
				Value = new Value();
				return;
			}
			if (op == OpCode.Array)
			{
				if (Code[at++] != 0)
					throw new NotImplementedException("Fixed and multi-dimensional arrays not implemented");
				TypeReference(ref at);
				Value = Root.Get(OpCode.Array, Value);
				return;
			}
			throw new NotImplementedException();
		}

		protected override void Literal(OpCode op, ref int at)
		{
			switch (op)
			{
			case OpCode.Undefined:
				Value = new Value();
				return;
			case OpCode.Null:
				Value = new Value(ValueKind.Object, null);
				return;
			case OpCode.False:
				Value = false;
				return;
			case OpCode.True:
				Value = true;
				return;
			case OpCode.This:
				Value = new Value(Ctx.Self);
				return;
			case OpCode.Base:
				Value = new Value(Ctx.Self?.BaseClass);
				return;
			case OpCode.Identifier:
				var name = Strings[CodeInt(ref at)];
				Value = new Value(ValueKind.Reference, Ctx.Which(name) ?? Ctx.Root, name);
				return;
			case OpCode.String:
				Value = Strings[CodeInt(ref at)];
				return;
			case OpCode.Byte:
				Value = Code[at++];
				return;
			case OpCode.UShort:
				Value = CodeUShort(ref at);
				return;
			case OpCode.UInt:
				Value = CodeUInt(ref at);
				return;
			case OpCode.ULong:
				Value = CodeULong(ref at);
				return;
			case OpCode.SByte:
				Value = (sbyte)Code[at++];
				return;
			case OpCode.Short:
				Value = CodeShort(ref at);
				return;
			case OpCode.Int:
				Value = CodeInt(ref at);
				return;
			case OpCode.Long:
				Value = CodeLong(ref at);
				return;
			case OpCode.Float:
				Value = CodeFloat(ref at);
				return;
			case OpCode.Double:
				Value = CodeDouble(ref at);
				return;
			}
			throw new NotImplementedException();
		}

		protected override void Special(OpCode op, ref int at)
		{
			var create = false;
		next:
			switch (op)
			{
			case OpCode.Create:
				create = true;
				op = ((OpCode)Code[at]).Extend();
				if (op.Kind() == OpKind.Special && (byte)op < OpCode.Generic.Code())
				{
					at++;
					goto next;
				}
				goto case OpCode.Call0;
			case OpCode.Call0:
				if (create)
					TypeReference(ref at);
				else
					Expression(ref at);
				IObject self = null;
				if (Value.Type == ValueKind.Reference)
				{
					self = Value.ptr as IObject;
					Value = ((IProperties)Value.ptr).Get(Value.str);
				}
				var fn = Box(Value);
				Value = create ? new Value(fn.Create(0)) : fn.Call(self, 0);
				return;
			case OpCode.Call1:
				if (create)
					TypeReference(ref at);
				else
					Expression(ref at);
				self = null;
				if (Value.Type == ValueKind.Reference)
				{
					self = Value.ptr as IObject;
					Value = ((IProperties)Value.ptr).Get(Value.str);
				}
				fn = Box(Value);
				Expression(ref at);
				Args.Add(Result);
				Value = create ? new Value(fn.Create(1)) : fn.Call(self, 1);
				Args.Remove(1);
				return;
			case OpCode.Call2:
				if (create)
					TypeReference(ref at);
				else
					Expression(ref at);
				self = null;
				if (Value.Type == ValueKind.Reference)
				{
					self = Value.ptr as IObject;
					Value = ((IProperties)Value.ptr).Get(Value.str);
				}
				fn = Box(Value);
				Expression(ref at);
				Args.Add(Result);
				Expression(ref at);
				Args.Add(Result);
				Value = create ? new Value(fn.Create(2)) : fn.Call(self, 2);
				Args.Remove(2);
				return;
			case OpCode.CallN:
				int n = Code[at++];
				if (create)
					TypeReference(ref at);
				else
					Expression(ref at);
				self = null;
				if (Value.Type == ValueKind.Reference)
				{
					self = Value.ptr as IObject;
					Value = ((IProperties)Value.ptr).Get(Value.str);
				}
				fn = Box(Value);
				var argc = n - 1;
				while (--n > 0)
				{
					Expression(ref at);
					Args.Add(Result);
				}
				Value = create ? new Value(fn.Create(argc)) : fn.Call(self, argc);
				Args.Remove(argc);
				return;
			case OpCode.Index:
			case OpCode.IndexN:
				n = op == OpCode.Index ? 2 : Code[at++];
				Expression(ref at);
				self = null;
				if (Value.Type == ValueKind.Reference)
				{
					self = Value.ptr as IObject;
					Value = ((IProperties)Value.ptr).Get(Value.str);
				}
				fn = Box(Value);
				argc = n - 1;
				while (--n > 0)
				{
					Expression(ref at);
					Args.Add(Result);
				}
				Value = fn.Index(self, argc);
				Args.Remove(argc);
				return;
			case OpCode.Var:
				var name = Strings[CodeInt(ref at)];
				TypeReference(ref at);
				if (Value.Type == ValueKind.Undefined)
				{
					Expression(ref at);
					Ctx.Vars.Set(name, Value);
					return;
				}
				fn = Box(Value);
				Expression(ref at);
				Args.Add(Result);
				Value = fn.Call(null, 1);
				Args.Remove(1);
				Ctx.Vars.Set(name, Value);
				return;
			case OpCode.Dot:
				Expression(ref at);
				fn = Box(Value);
				name = Strings[CodeInt(ref at)];
				Value = new Value(fn, name);
				return;
			case OpCode.Ternary:
				Expression(ref at);
				if (Result.Bool)
				{
					at += 4;
					Expression(ref at);
					var fsz = CodeInt(ref at);
					at += fsz;
				}
				else
				{
					var tsz = CodeInt(ref at);
					at += tsz;
					at += 4;
					Expression(ref at);
				}
				return;
			}
			throw new NotImplementedException();
		}

		protected override void Binary(OpCode op, ref int at)
		{
			Expression(ref at);
			if (op == OpCode.LogicAnd || op == OpCode.LogicOr)
			{
				if ((Value == true) == (op == OpCode.LogicOr))
				{
					Value = op == OpCode.LogicOr;
					var n = CodeInt(ref at);
					at += n;
					return;
				}
				Expression(ref at);
				return;
			}
			var left = Value;
			Expression(ref at);
			switch (op)
			{
			case OpCode.Assign:
				left.Set(Value);
				return;
			case OpCode.OrAssign:
				left.Set(Value = left | Value);
				return;
			case OpCode.XorAssign:
				left.Set(Value = left ^ Value);
				return;
			case OpCode.AndAssign:
				left.Set(Value = left & Value);
				return;
			case OpCode.LshAssign:
				left.Set(Value = left.ShiftLeft(Value));
				return;
			case OpCode.RshAssign:
				left.Set(Value = left.ShiftRight(Value));
				return;
			case OpCode.AddAssign:
				left.Set(Value = left + Value);
				return;
			case OpCode.SubAssign:
				left.Set(Value = left - Value);
				return;
			case OpCode.MulAssign:
				left.Set(Value = left * Value);
				return;
			case OpCode.DivAssign:
				left.Set(Value = left / Value);
				return;
			case OpCode.ModAssign:
				left.Set(Value = left % Value);
				return;
			case OpCode.BitOr:
				Value = left | Value;
				return;
			case OpCode.BitXor:
				Value = left ^ Value;
				return;
			case OpCode.BitAnd:
				Value = left & Value;
				return;
			case OpCode.ShiftLeft:
				Value = left.ShiftLeft(Value);
				return;
			case OpCode.ShiftRight:
				Value = left.ShiftRight(Value);
				return;
			case OpCode.Add:
				Value = left + Value;
				return;
			case OpCode.Sub:
				Value = left - Value;
				return;
			case OpCode.Mul:
				Value = left * Value;
				return;
			case OpCode.Div:
				Value = left / Value;
				return;
			case OpCode.Mod:
				Value = left % Value;
				return;
			case OpCode.Equals:
				Value = new Value(left == Value);
				return;
			case OpCode.Differ:
				Value = new Value(left != Value);
				return;
			case OpCode.Less:
				Value = new Value(left < Value);
				return;
			case OpCode.More:
				Value = new Value(left > Value);
				return;
			case OpCode.LessEq:
				Value = new Value(left <= Value);
				return;
			case OpCode.MoreEq:
				Value = new Value(left >= Value);
				return;
			}
			throw new NotImplementedException();
		}

		protected override void Unary(OpCode op, ref int at)
		{
			Expression(ref at);
			switch (op)
			{
			case OpCode.Plus:
				Value = -Value;
				return;
			case OpCode.Neg:
				Value = -Value;
				return;
			case OpCode.Flip:
				Value = ~Value;
				return;
			case OpCode.Not:
				Value = new Value(!Value.Bool);
				return;
			case OpCode.PostInc:
				if (Value.Type == ValueKind.Reference)
					Value = Value.Self++;
				return;
			case OpCode.PostDec:
				if (Value.Type == ValueKind.Reference)
					Value = Value.Self--;
				return;
			case OpCode.Inc:
				++Value.Self;
				return;
			case OpCode.Dec:
				++Value.Self;
				return;
			}
			throw new NotImplementedException();
		}
	}
}
