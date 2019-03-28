using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	public partial class Engine<P>
	{
		protected virtual void TypeReference(ref int at)
		{
			var op = ((OpCode)Code[at++]).Extend();
			if (op.Kind() <= OpKind.Number)
			{
				if (op == OpCode.Null || (byte)op >= OpCode.String.Code())
				{
					Value = new Value(Root.GetType(op));
					return;
				}
				if (op == OpCode.Identifier)
				{
					Value = Context.Get(Strings[CodeInt(ref at)]);
					return;
				}
				Value = new Value();
				return;
			}
			if (op == OpCode.Array)
			{
				if (Code[at++] != 0)
					throw new NotImplementedException("Fixed-typed and multi-dimensional arrays not implemented");
				TypeReference(ref at);
				Value = new Value(Root.GetType(OpCode.Array, Value));
				return;
			}
			if (op == OpCode.Dot)
			{
				Expression(ref at);
				var obj = Box(Value);
				if (obj != null)
					Value = new Value(obj, Strings[CodeInt(ref at)]);
				else if (HasOption(EngineOption.Silent))
					Value = new Value();
				else throw new InvalidOperationException("Null cannot be dereferenced");
				return;
			}
			throw new NotImplementedException(string.Format(Value.Culture,
				"Unknown type reference: {0:04X} {1}", (ushort)op, op));
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
				// this = undefined in methods and undefined == null (but undefined !== null)
				Value = Context.Self != null ? new Value(Context.Self)
					: HasOption(EngineOption.Strict) ? new Value() : new Value(Root);
				return;
			case OpCode.Base:
				Value = new Value(Context.Self?.BaseClass);
				return;
			case OpCode.Identifier:
				var name = Strings[CodeInt(ref at)];
				var which = Context.Which(name);
				if (which == null && HasOption(EngineOption.Strict))
				{
					if (!HasOption(EngineOption.Silent))
						throw new InvalidOperationException(string.Format(Value.Culture,
							"Variable {0} does not exist", name));
					Value = new Value();
					return;
				}
				Value = new Value(ValueKind.Reference, which ?? Context.Root, name);
				return;
			case OpCode.String:
				Value = Strings[CodeInt(ref at)];
				return;
			case OpCode.Char:
				Value = (char)Code[at++];
				return;
			case OpCode.WideChar:
				Value = (char)CodeUShort(ref at);
				return;
			case OpCode.LongChar:
				var ch1 = (char)CodeUShort(ref at);
				var ch2 = (char)CodeUShort(ref at);
				Value = new string(ch1, ch2);
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
			throw new NotImplementedException(string.Format(Value.Culture,
				"Unknown literal: {0:04X} {1}", (ushort)op, op));
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
				if (op.Kind() == OpKind.Special && (byte)op < OpCode.Dot.Code())
				{
					at++;
					goto next;
				}
				goto case OpCode.Call0;
			case OpCode.Call0:
				CountStatement();
				IObject self = null;
				if (create)
					TypeReference(ref at);
				else if (Code[at] != OpCode.Generic.Code())
					Expression(ref at);
				else
				{
					at++;
					self = Generic(ref at);
				}
				if (Value.IsReference)
				{
					self = Value.ptr as IObject;
					Value = Result;
				}
				if (self == Root && HasOption(EngineOption.Strict))
					self = null;
				var fn = Box(Value);
				Value = create ? new Value(fn.Create(0)) : fn.Call(self, 0);
				return;
			case OpCode.Call1:
				CountStatement();
				self = null;
				if (create)
					TypeReference(ref at);
				else if (Code[at] != OpCode.Generic.Code())
					Expression(ref at);
				else
				{
					at++;
					self = Generic(ref at);
				}
				if (Value.IsReference)
				{
					self = Value.ptr as IObject;
					Value = Result;
				}
				fn = Box(Value);
				Expression(ref at);
				using (Arguments.Guard())
				{
					Arguments.Add(Result);
					Value = create ? new Value(fn.Create(1)) : fn.Call(self, 1);
				}
				return;
			case OpCode.Call2:
				CountStatement();
				self = null;
				if (create)
					TypeReference(ref at);
				else if (Code[at] != OpCode.Generic.Code())
					Expression(ref at);
				else
				{
					at++;
					self = Generic(ref at);
				}
				if (Value.IsReference)
				{
					self = Value.ptr as IObject;
					Value = Result;
				}
				fn = Box(Value);
				Expression(ref at);
				using (Arguments.Guard())
				{
					Arguments.Add(Result);
					Expression(ref at);
					Arguments.Add(Result);
					Value = create ? new Value(fn.Create(2)) : fn.Call(self, 2);
				}
				return;
			case OpCode.CallN:
				CountStatement();
				int n = Code[at++];
				self = null;
				if (create)
					TypeReference(ref at);
				else if (Code[at] != OpCode.Generic.Code())
					Expression(ref at);
				else
				{
					at++;
					self = Generic(ref at);
				}
				if (Value.IsReference)
				{
					self = Value.ptr as IObject;
					Value = Result;
				}
				fn = Box(Value);
				var argc = n - 1;
				using (Arguments.Guard())
				{
					while (--n > 0)
					{
						Expression(ref at);
						Arguments.Add(Result);
					}
					Value = create ? new Value(fn.Create(argc)) : fn.Call(self, argc);
				}
				return;
			case OpCode.Index:
			case OpCode.IndexN:
				CountStatement();
				n = op == OpCode.Index ? 2 : Code[at++];
				Expression(ref at);
				self = null;
				if (Value.IsReference)
				{
					self = Value.ptr as IObject;
					Value = Result;
				}
				fn = Box(Value);
				argc = n - 1;
				using (Arguments.Guard())
				{
					while (--n > 0)
					{
						Expression(ref at);
						Arguments.Add(Result);
					}
					Value = fn.Index(self, argc);
				}
				return;
			case OpCode.Var:
				var name = Strings[CodeInt(ref at)];
				TypeReference(ref at);
				if (Value.Kind == ValueKind.Undefined)
				{
					Expression(ref at);
					Context.Vars.Add(name, Value);
					Value = new Value(Context.Vars, name);
					return;
				}
				// strongly typed variable - have to call converter
				CountStatement();
				fn = Box(Value);
				Expression(ref at);
				using (Arguments.Guard())
				{
					Arguments.Add(Result);
					Value = fn.Call(null, 1);
				}
				Context.Vars.Add(name, Value);
				Value = new Value(Context.Vars, name);
				return;
			case OpCode.Dot:
				Expression(ref at);
				fn = Box(Value);
				name = Strings[CodeInt(ref at)];
				if (fn != null)
					Value = new Value(fn, name);
				else if (HasOption(EngineOption.Silent))
					Value = new Value();
				else throw new InvalidOperationException("Null cannot be dereferenced");
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
			case OpCode.Function:
				Value = new Value(Function(null, ref at));
				return;
			case OpCode.Array:
				n = Code[at++];
				TypeReference(ref at);
				var arrCreator = Root.GetType(OpCode.Array, Result);
				if (arrCreator == null)
					throw new NotImplementedException("Array");
				if (n <= 1)
				{
					Value = new Value(arrCreator.Create(0));
					return;
				}
				using (Arguments.Guard())
				{
					for (int i = 1; i < n; i++)
					{
						Expression(ref at);
						Arguments.Add(Result);
					}
					Value = new Value(arrCreator.Create(n-1));
				}
				return;
			}
			throw new NotImplementedException(string.Format(Value.Culture,
				"Unknown special opcode: {0:04X} {1}", (ushort)op, op));
		}
		private IObject Generic(ref int at)
		{
			int n = Code[at++];
			if (n != 2)
				throw new NotImplementedException("Generic with more than one parameter");
			Expression(ref at);
			IObject self = null;
			if (Value.IsReference)
			{
				self = Value.ptr as IObject;
				Value = Result;
			}
			var fn = Box(Value);
			TypeReference(ref at);
			var gtype = Value.Object;
			if (gtype == null || (gtype.Features & ObjectFeatures.TypeReference) == 0)
				throw new NotImplementedException("Could not resolve generic argument");
			//TODO: use some interface for this
			if (fn is ReflectedObjects.ReflectedFunction rfn)
			{
				foreach (var method in rfn.Methods)
				{
					if (!method.IsGenericMethod)
						continue;
					var gargs = method.GetGenericArguments();
					if (gargs.Length != 1)
						continue;
					try
					{
						var mtd = method.MakeGenericMethod(gtype.Type);
						Value = new ReflectedObjects.ReflectedFunction(
							this, rfn.Creator, rfn.Name, mtd);
						return self;
					}
					catch
					{
						continue;
					}
				}
				throw new NotImplementedException("Did not find suitable generic function");
			}
			else if (fn is ReflectedObjects.ReflectedMethod mtd)
			{
				foreach (var method in mtd.Methods)
				{
					if (!method.IsGenericMethod)
						continue;
					var gargs = method.GetGenericArguments();
					if (gargs.Length != 1)
						continue;
					try
					{
						var mtd2 = method.MakeGenericMethod(gtype.Type);
						Value = new ReflectedObjects.ReflectedMethod(
							this, mtd.Creator, mtd.Name, mtd2);
						return self;
					}
					catch
					{
						continue;
					}
				}
				throw new NotImplementedException("Did not find suitable generic function");
			}
			throw new NotImplementedException("Unknown type for specialization: "
				+ (fn?.GetType().Name ?? "null"));
		}

		protected override void Binary(OpCode op, ref int at)
		{
			Expression(ref at);
			if (op == OpCode.LogicAnd || op == OpCode.LogicOr)
			{
				var n = CodeInt(ref at);
				if ((Value == true) == (op == OpCode.LogicOr))
				{
					Value = op == OpCode.LogicOr;
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
			case OpCode.OrAssign:
			case OpCode.XorAssign:
			case OpCode.AndAssign:
			case OpCode.LshAssign:
			case OpCode.RshAssign:
			case OpCode.AddAssign:
			case OpCode.SubAssign:
			case OpCode.MulAssign:
			case OpCode.DivAssign:
			case OpCode.ModAssign:
				if (!left.IsReference || left.ptr == null)
				{
					if (!HasOption(EngineOption.Silent))
						throw new InvalidOperationException("Cannot assign to " + left.Name);
					return;
				}
				if (left.Kind == ValueKind.Reference && left.ptr == Root
					&& HasOption(EngineOption.Strict) && !Root.Has((string)left.idx))
					throw new InvalidOperationException(string.Format(Value.Culture,
						"Global variable '{0}' does not exist", left.idx));
				if (op == OpCode.Assign)
				{
					if (left.Set(Value) || HasOption(EngineOption.Silent))
						return;
					throw new InvalidOperationException(left.Name + " is read only");
				}
				if (!left.Modify(op, Value) && !HasOption(EngineOption.Silent))
					throw new InvalidOperationException(left.Name + " is read only"); ;
				Value = left;
				return;
			case OpCode.BitOr:
			case OpCode.BitXor:
			case OpCode.BitAnd:
			case OpCode.ShiftLeft:
			case OpCode.ShiftRight:
			case OpCode.Add:
			case OpCode.Sub:
			case OpCode.Mul:
			case OpCode.Div:
			case OpCode.Mod:
			case OpCode.Equals:
			case OpCode.Differ:
			case OpCode.Less:
			case OpCode.More:
			case OpCode.LessEq:
			case OpCode.MoreEq:
				Value = left.Binary(op, Value);
				return;
			}
			throw new NotImplementedException(string.Format(Value.Culture,
				"Unknown binary operator: {0:04X} {1}", (ushort)op, op));
		}

		protected override void Unary(OpCode op, ref int at)
		{
			Expression(ref at);
			switch (op)
			{
			case OpCode.Plus:
				Value = +Value;
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
			// NOTE: Consider using Value.Modify
			case OpCode.PostInc:
				if (Value.IsReference)
					Value = Value.Self++;
				return;
			case OpCode.PostDec:
				if (Value.IsReference)
					Value = Value.Self--;
				return;
			case OpCode.Inc:
				++Value.Self;
				return;
			case OpCode.Dec:
				--Value.Self;
				return;
			}
			throw new NotImplementedException(string.Format(Value.Culture,
				"Unknown unary operator: {0:04X} {1}", (ushort)op, op));
		}
	}
}
