using System;
using System.Collections.Generic;
using System.Diagnostics;
using RedOnion.ROS.Objects;

namespace RedOnion.ROS
{
	partial class Core
	{
		/// <summary>
		/// Execute or continue executing current code.
		/// Returns true if finished
		/// (<see cref="Exit" /> is set to <see cref="ExitCode.Countdown"/>
		/// when returning false).
		/// </summary>
		/// <param name="countdown">Countdown until auto-yield</param>
		public bool Execute(int countdown = 1000)
		{
			if (Exit == ExitCode.Exception)
				return true;
			var at = this.at;
			var code = this.code;
			var str = this.str;
			int blockEnd = ctx.BlockEnd;
			var op = OpCode.Void;
			try
			{
				for (; ; )
				{
					#region Block exit and countdown

					while (at == blockEnd)
					{
						if (ctx.BlockCount == 0 && ctx.BlockCode != OpCode.Return)
							goto finishNoReturn;

						if (countdown <= 0)
						{
							this.at = at;
							Exit = ExitCode.Countdown;
							Countdown = countdown;
							return false;
						}
						countdown--;

						switch (ctx.BlockCode)
						{
						default:
							blockEnd = ctx.Pop();
							continue;
						case OpCode.While:
						case OpCode.Until:
						case OpCode.For:
							at = ctx.BlockAt1;
							ctx.ResetTop();
							continue;
						case OpCode.Do:
						case OpCode.DoUntil:
						{
							ref var cond = ref vals.Top();
							if (cond.IsReference && !cond.desc.Get(ref cond, cond.num.Int))
								throw CouldNotGet(ref cond);
							if (cond.desc.Primitive != ExCode.Bool && !cond.desc.Convert(ref cond, Descriptor.Bool))
								throw InvalidOperation("Could not convert '{0}' to boolean", cond.Name);

							if (cond.num.Bool != (ctx.BlockCode == OpCode.Do))
							{
								blockEnd = ctx.Pop();
								continue;
							}
							at = ctx.BlockStart;
							ctx.ResetTop();
							continue;
						}
						case OpCode.ForEach:
						{
							var enu = (IEnumerator<Value>)vals.Top(-1).obj;
							if (!enu.MoveNext())
							{
								vals.Pop(2);
								blockEnd = ctx.Pop();
								continue;
							}
							ref var evar = ref vals.Top(-2);
							var value = enu.Current;
							if (!evar.desc.Set(ref evar, evar.num.Int, OpCode.Assign, ref value))
								throw InvalidOperation(
									"Property '{0}' of '{1}' is read only",
									evar.desc.NameOf(evar.obj, evar.num.Int), evar.desc.Name);
							at = ctx.BlockStart;
							ctx.ResetTop();
							continue;
						}
						case OpCode.Function:
						{
							ref var top = ref stack.Top();
							var vtop = top.vtop;
							if (top.create)
								vals.GetRef(vals.Count, vtop - 1) = self;
							vals.Pop(vals.Count - top.vtop);
							this.at = at = top.at;
							compiled = top.code;
							this.code = code = compiled.Code;
							this.str = str = compiled.Strings;
							self = top.prevSelf;
							if (ctx != top.context)
							{
								ctx.PopAll();
								ctx = top.context;
							}
							stack.Pop();
							continue;
						}
						// library end
						case OpCode.Return:
						{
							Debug.Assert(stack.Count > 0);
							ref var top = ref stack.Top();
							Debug.Assert(top.context == ctx);
							var vtop = top.vtop;
							ref var result = ref vals.GetRef(vals.Count, vtop - 1);
							vals.Pop(vals.Count - top.vtop);
							this.at = at = top.at;
							compiled = top.code;
							this.code = code = compiled.Code;
							this.str = str = compiled.Strings;
							self = top.prevSelf;
							ctx.BlockCode = top.blockCode;
							ctx.BlockEnd = top.blockEnd;
							blockEnd = ctx.BlockEnd;
							stack.Pop();
							continue;
						}
						}
					}
					if (countdown <= 0)
					{
						this.at = at;
						Exit = ExitCode.Countdown;
						Countdown = countdown;
						return false;
					}
					countdown--;

					#endregion

					//##############################################################################

					op = (OpCode)code[at++];
					this.at = at;
					switch (op)
					{

					#region Constants, literals and references

					//--------------------------------------------------------------------- literals

					case OpCode.Void:
						vals.Add(Value.Void);
						continue;
					case OpCode.Null:
						vals.Add(Value.Null);
						continue;
					case OpCode.False:
						vals.Add(Value.False);
						continue;
					case OpCode.True:
						vals.Add(Value.True);
						continue;
					case OpCode.This:
						vals.Add(self);
						continue;

					//TODO: Base, Value, Implicit, Exception, Default

					case OpCode.Identifier:
						Identifier(at);
						at += 4;
						continue;
					case OpCode.String:
						vals.Add(str[Int(code, at)]);
						at += 4;
						continue;
					case OpCode.Char:
						vals.Add((char)code[at++]);
						continue;
					case OpCode.WideChar:
						vals.Add((char)Short(code, at));
						at += 2;
						continue;

					//---------------------------------------------------------------------- numbers
					case OpCode.Byte:
						vals.Add(code[at++]);
						continue;
					case OpCode.UShort:
						vals.Add((ushort)Short(code, at));
						at += 2;
						continue;
					case OpCode.UInt:
						vals.Add((uint)Int(code, at));
						at += 4;
						continue;
					case OpCode.ULong:
						vals.Add((uint)Long(code, at));
						at += 8;
						continue;
					case OpCode.SByte:
						vals.Add((sbyte)code[at++]);
						continue;
					case OpCode.Short:
						vals.Add(Short(code, at));
						at += 2;
						continue;
					case OpCode.Int:
						vals.Add(Int(code, at));
						at += 4;
						continue;
					case OpCode.Long:
						vals.Add(Long(code, at));
						at += 8;
						continue;
					case OpCode.Float:
						vals.Add(Float(code, at));
						at += 4;
						continue;
					case OpCode.Double:
						vals.Add(Double(code, at));
						at += 8;
						continue;

					#endregion

					#region Special operators/expressions (call, index, new, ternary, ...)

					//---------------------------------------------------------------------- special
					case OpCode.Create:
						op = (OpCode)code[at++];
						switch (op)
						{
						case OpCode.Identifier:
						{
							Identifier(at);
							at += 4;
							this.at = at;
							Call(0, true, op);
							code = this.code;
							str = this.str;
							at = this.at;
							blockEnd = ctx.BlockEnd;
							continue;
						}
						case OpCode.Dot:
						{
							var name = str[Int(code, at)];
							at += 4;
							ref var it = ref vals.Top();
							if (it.IsReference && !it.desc.Get(ref it, it.num.Int))
								throw CouldNotGet(ref it);
							if (it.IsNumerOrChar)
								throw InvalidOperation("Numbers do not have properties");
							var idx = it.desc.Find(it.obj, name, true);
							if (idx < 0)
								throw InvalidOperation("'{0}' does not have property '{1}'", it.Name, name);
							it.SetRef(idx);
							this.at = at;
							Call(0, true, op);
							code = this.code;
							str = this.str;
							at = this.at;
							blockEnd = ctx.BlockEnd;
							continue;
						}
						case OpCode.Call0:
							this.at = at;
							Call(0, true, op);
							code = this.code;
							str = this.str;
							at = this.at;
							blockEnd = ctx.BlockEnd;
							continue;
						case OpCode.Call1:
							this.at = at;
							Call(1, true, op);
							code = this.code;
							str = this.str;
							at = this.at;
							blockEnd = ctx.BlockEnd;
							continue;
						case OpCode.Call2:
							this.at = at;
							Call(2, true, op);
							code = this.code;
							str = this.str;
							at = this.at;
							blockEnd = ctx.BlockEnd;
							continue;
						case OpCode.CallN:
						{
							var n = code[at++];
							this.at = at;
							Call(n-1, true, op);
							code = this.code;
							str = this.str;
							at = this.at;
							blockEnd = ctx.BlockEnd;
							continue;
						}
						}
						throw new NotImplementedException("Not implemented: OpCode.Create + " + op.ToString());

					case OpCode.Autocall:
					case OpCode.Call0:
						this.at = at;
						Call(0, false, op);
						code = this.code;
						str = this.str;
						at = this.at;
						blockEnd = ctx.BlockEnd;
						continue;
					case OpCode.Call1:
						this.at = at;
						Call(1, false, op);
						code = this.code;
						str = this.str;
						at = this.at;
						blockEnd = ctx.BlockEnd;
						continue;
					case OpCode.Call2:
						this.at = at;
						Call(2, false, op);
						code = this.code;
						str = this.str;
						at = this.at;
						blockEnd = ctx.BlockEnd;
						continue;
					case OpCode.CallN:
					{
						var n = code[at++];
						this.at = at;
						Call(n-1, false, op);
						code = this.code;
						str = this.str;
						at = this.at;
						blockEnd = ctx.BlockEnd;
						continue;
					}

					case OpCode.Index:
					{
						Dereference(1);
						ref var lhs = ref vals.Top(-2);
						if (lhs.IsReference && !lhs.desc.Get(ref lhs, lhs.num.Int))
							throw CouldNotGet(ref lhs);
						var idx = lhs.desc.IndexFind(ref lhs, new Arguments(Arguments, 1));
						if (idx < 0)
							throw InvalidOperation("'{0}' cannot be indexed by '{1}'", lhs.Name, vals.Top().ToString());
						lhs.SetRef(idx);
						vals.Pop(1);
						continue;
					}
					case OpCode.IndexN:
					{
						var n = code[at++];
						Dereference(n-1);
						ref var it = ref vals.Top(-n);
						if (it.IsReference && !it.desc.Get(ref it, it.num.Int))
							throw CouldNotGet(ref it);
						var idx = it.desc.IndexFind(ref it, new Arguments(Arguments, n - 1));
						if (idx < 0)
							throw InvalidOperation("'{0}' cannot be indexed by '{1}'", it.Name, vals.Top().ToString());
						it.SetRef(idx);
						vals.Pop(n - 1);
						continue;
					}
					case OpCode.Dot:
					{
						var name = str[Int(code, at)];
						at += 4;
						ref var it = ref vals.Top();
						if (it.IsReference && !it.desc.Get(ref it, it.num.Int))
							throw CouldNotGet(ref it);
						if (it.IsNumerOrChar)
							throw InvalidOperation("Numbers do not have properties");
						var idx = it.desc.Find(it.obj, name, true);
						if (idx < 0)
							throw InvalidOperation("'{0}' does not have property '{1}'", it.Name, name);
						it.SetRef(idx);
						continue;
					}
					case OpCode.Var:
					{
						var name = str[Int(code, at)];
						at += 4;
						// lhs is type, TODO: typed variables
						ref var rhs = ref vals.Top(-1);
						if (rhs.IsReference && !rhs.desc.Get(ref rhs, rhs.num.Int))
							throw CouldNotGet(ref rhs);
						var idx = ctx.Add(name, ref rhs);
						vals.Pop(1);
						ref var it = ref vals.Top();
						it.SetRef(ctx, idx);
						continue;
					}
					case OpCode.Array:
					{
						byte n = code[at++];
						ref var type = ref vals.Top(-n);
						if (type.desc != Descriptor.Void)
							throw new NotImplementedException("Typed array");
						var arr = new Value[n - 1];
						for (int i = 0; i < arr.Length; i++)
							arr[i] = vals.Top(i - arr.Length);
						type = new Value(arr);
						vals.Pop(n - 1);
						continue;
					}

					case OpCode.Ternary:
					{
						ref var cond = ref vals.Top();
						if (cond.IsReference && !cond.desc.Get(ref cond, cond.num.Int))
							throw CouldNotGet(ref cond);
						if (cond.desc.Primitive != ExCode.Bool && !cond.desc.Convert(ref cond, Descriptor.Bool))
							throw InvalidOperation("Could not convert '{0}' to boolean", cond.Name);
						int sz = Int(code, at);
						at += 4;
						var it = cond.num.Bool;
						vals.Pop();
						if (!it)
							at += sz + 5;
						continue;
					}

					#endregion

					#region Other operators

					//----------------------------------------------------------------------- assign
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
					{
						ref var lhs = ref vals.Top(-2);
						if (!lhs.IsReference)
							throw InvalidOperation(
								"Cannot {0} '{1}'",
								op == OpCode.Assign ? "assign to" : "modify",
								lhs.desc.Name);
						ref var rhs = ref vals.Top(-1);
						if (rhs.IsReference && !rhs.desc.Get(ref rhs, rhs.num.Int))
							throw CouldNotGet(ref rhs);
						if (!lhs.desc.Set(ref lhs, lhs.num.Int, op, ref rhs))
							throw InvalidOperation(
								"Property '{0}' of '{1}' {2}",
								lhs.desc.NameOf(lhs.obj, lhs.num.Int), lhs.desc.Name,
								op == OpCode.Assign ? "is read only" : "cannot be modified");
						if (op == OpCode.Assign)
							lhs = rhs;
						vals.Pop(1);
						continue;
					}

					//------------------------------------------------------------- binary and logic
					case OpCode.BitOr:
					case OpCode.BitXor:
					case OpCode.BitAnd:
					case OpCode.ShiftLeft:
					case OpCode.ShiftRight:
					case OpCode.Add:
					case OpCode.Sub:
					case OpCode.Mul:
					case OpCode.Div:
					case OpCode.Equals:
					case OpCode.Differ:
					case OpCode.Less:
					case OpCode.More:
					case OpCode.LessEq:
					case OpCode.MoreEq:
					{
						ref var lhs = ref vals.Top(-2);
						if (lhs.IsReference && !lhs.desc.Get(ref lhs, lhs.num.Int))
							throw CouldNotGet(ref lhs);
						ref var rhs = ref vals.Top(-1);
						if (rhs.IsReference && !rhs.desc.Get(ref rhs, rhs.num.Int))
							throw CouldNotGet(ref rhs);
						if (!lhs.desc.Binary(ref lhs, op, ref rhs)
						&& !rhs.desc.Binary(ref lhs, op, ref rhs))
							throw InvalidOperation(
								"Binary operator '{0}' not supported on operands '{1}' and '{2}'",
								op.Text(), lhs.desc.Name, rhs.desc.Name);
						vals.Pop(1);
						continue;
					}
					//------------------------------------------------------------------ other logic
					case OpCode.NullCol:
					case OpCode.LogicOr:
					case OpCode.LogicAnd:
					{
						ref var lhs = ref vals.Top();
						if (lhs.IsReference && !lhs.desc.Get(ref lhs, lhs.num.Int))
							throw CouldNotGet(ref lhs);
						if (op != OpCode.NullCol
							&& lhs.desc.Primitive != ExCode.Bool
							&& !lhs.desc.Convert(ref lhs, Descriptor.Bool))
							throw InvalidOperation("Could not convert '{0}' to boolean", lhs.Name);
						int sz = Int(code, at);
						at += 4;
						if (op == OpCode.NullCol ? !lhs.IsNull : lhs.num.Bool == (op == OpCode.LogicOr))
						{
							at += sz;
							continue;
						}
						vals.Pop(1);
						continue;
					}
					case OpCode.Identity:
					case OpCode.NotIdentity:
					{
						ref var lhs = ref vals.Top(-2);
						if (lhs.IsReference && !lhs.desc.Get(ref lhs, lhs.num.Int))
							throw CouldNotGet(ref lhs);
						ref var rhs = ref vals.Top(-1);
						if (rhs.IsReference && !rhs.desc.Get(ref rhs, rhs.num.Int))
							throw CouldNotGet(ref rhs);
						lhs = (lhs.desc == rhs.desc && lhs.obj == rhs.obj
							&& lhs.num.Long == rhs.num.Long) == (op == OpCode.Identity);
						vals.Pop(1);
						continue;
					}

					//------------------------------------------------------------------------ unary
					case OpCode.Plus:
					case OpCode.Neg:
					case OpCode.Flip:
					case OpCode.Not:
					{
						ref var it = ref vals.Top();
						if (it.IsReference && !it.desc.Get(ref it, it.num.Int))
							throw CouldNotGet(ref it);
						if (!it.desc.Unary(ref it, op))
							throw InvalidOperation(
								"Unary operator '{0}' not supported on operand '{1}'",
								op.Text(), it.desc.Name);
						continue;
					}

					//----------------------------------------------------------------- post and pre
					case OpCode.PostInc:
					case OpCode.PostDec:
					case OpCode.Inc:
					case OpCode.Dec:
					{
						ref var it = ref vals.Top();
						if (!it.IsReference)
							throw InvalidOperation("Cannot modify '{0}'", it.desc.Name);
						if (!it.desc.Set(ref it, it.num.Int, op, ref it))
							throw InvalidOperation(
								"Property '{0}' of '{1}' cannot be modified",
								it.desc.NameOf(it.obj, it.num.Int), it.desc.Name);
						continue;
					}
					#endregion

					//=================================================================== statements

					#region Simple statements (return, break, continue)

					case OpCode.Raise:
					//TODO: try..catch..finally
					case OpCode.Return:
						if (stack.size > 0)
						{
							ref var top = ref stack.Top();
							var vtop = top.vtop;
							ref var result = ref vals.GetRef(vals.Count, vtop - 1);
							if (top.create)
								result = self;
							else
							{
								result = vals.Top();
								if (result.IsReference && !result.desc.Get(ref result, result.num.Int))
									throw CouldNotGet(ref result);
							}
							this.at = at = top.at;
							compiled = top.code;
							this.code = code = compiled.Code;
							this.str = str = compiled.Strings;
							self = top.prevSelf;
							if (ctx != top.context)
							{
								vals.Pop(vals.Count - top.vtop);
								ctx.PopAll();
								ctx = top.context;
							}
							else
							{
								ctx.BlockCode = top.blockCode;
								ctx.BlockEnd = top.blockEnd;
							}
							blockEnd = ctx.BlockEnd;
							stack.Pop();
							continue;
						}
						Exit = op == OpCode.Return ? ExitCode.Return : ExitCode.Exception;
						goto finish;

					case OpCode.Break:
						at = blockEnd;
						while (ctx.BlockCount > 0
						&& (ctx.BlockCode < OpCode.For || ctx.BlockCode > OpCode.DoUntil))
							at = blockEnd = ctx.Pop();
						if (ctx.BlockCount > 0)
							ctx.BlockCode = OpCode.Block;
						continue;
					case OpCode.Continue:
					{
						var origin = at;
						while (ctx.BlockCount > 0
						&& (ctx.BlockCode < OpCode.For || ctx.BlockCode > OpCode.DoUntil))
							at = blockEnd = ctx.Pop();
						if (ctx.BlockCount == 0)
						{
							at = origin;
							throw InvalidOperation("No block to continue");
						}
						at = blockEnd;
						switch (ctx.BlockCode)
						{
						case OpCode.Do:
						case OpCode.DoUntil:
						case OpCode.For:
							at = ctx.BlockAt1;
							continue;
						}
						continue;
					}
					#endregion

					#region Loops (for, foreach, while/until and do-while/until)

					// for; init size; test size; init; test; last size; last; block size; block
					case OpCode.For:
					{
						var iniSz = Int(code, at);
						at += 4;
						var tstSz = Int(code, at);
						at += 4;
						var finAt = at + iniSz + tstSz + 4;
						var finSz = Int(code, finAt - 4);
						var blkAt = finAt + finSz + 4;
						var blkSz = Int(code, blkAt - 4);
						ctx.Push(blkAt, blockEnd = blkAt + blkSz, op, finAt, at + iniSz);
						continue;
					}
					// foreach; var size; list size; var; list; block size; block
					case OpCode.ForEach:
					{
						var varSz = Int(code, at);
						at += 4;
						var listSz = Int(code, at);
						at += 4;
						var blkAt = at + varSz + listSz + 4;
						var blkSz = Int(code, blkAt - 4);
						ctx.Push(blkAt, blockEnd = blkAt + blkSz, op, at, at + varSz);
						continue;
					}

					// while/until; cond size; cond + marker; block size; block
					case OpCode.While:
					case OpCode.Until:
					{
						var csz = Int(code, at);
						at += 4;
						var bsz = Int(code, at + csz);
						ctx.Push(at + csz + 4, blockEnd = at + csz + 4 + bsz, op, at);
						continue;
					}
					// do; cond size; block size; block; cond
					case OpCode.Do:
					case OpCode.DoUntil:
					{
						int csz = Int(code, at);
						at += 4;
						int bsz = Int(code, at);
						at += 4;
						ctx.Push(at, blockEnd = at + bsz + csz, op, at + bsz);
						continue;
					}
					#endregion

					#region Conditional, loop conditions and block

					case OpCode.If:
					case OpCode.Unless:
					{
						ref var cond = ref vals.Top();
						if (cond.IsReference && !cond.desc.Get(ref cond, cond.num.Int))
							throw CouldNotGet(ref cond);
						if (cond.desc.Primitive != ExCode.Bool && !cond.desc.Convert(ref cond, Descriptor.Bool))
							throw InvalidOperation("Could not convert '{0}' to boolean", cond.Name);

						if (cond.num.Bool == (op == OpCode.If))
							goto case OpCode.Block;
						int sz = Int(code, at);
						at += 4 + sz;
						if (at == blockEnd)
							continue;
						if ((OpCode)code[at] != OpCode.Else)
							continue;
						at++;
						goto case OpCode.Block;
					}
					case OpCode.Block:
					{
						int sz = Int(code, at);
						at += 4;
						ctx.Push(at, blockEnd = at + sz);
						continue;
					}
					case OpCode.Else:
					{
						int sz = Int(code, at);
						at += sz + 4;
						continue;
					}
					// while/until; cond size; cond + marker; block size; block
					// for; ini size; ini; cond size; cond; fin size; fin; block size; block
					case OpCode.Cond:
					{
						if (ctx.BlockCode == OpCode.ForEach)
						{
							ref var evar = ref vals.Top(-2);
							if (!evar.IsReference)
								throw InvalidOperation("Enumeration variable is not a reference"); ;
							ref var list = ref vals.Top(-1);
							if (list.IsReference && !list.desc.Get(ref list, list.num.Int))
								throw CouldNotGet(ref list);
							var enu = list.desc.Enumerate(list.obj);
							if (enu == null)
								throw InvalidOperation(list.Name + " is not enumerable");
							list.desc = null;
							list.obj = enu.GetEnumerator();
							list.num.Int = 0;
							at = blockEnd;
							ctx.LockTop();
							continue;
						}
						ref var cond = ref vals.Top();
						if (cond.IsReference && !cond.desc.Get(ref cond, cond.num.Int))
							throw CouldNotGet(ref cond);
						if (cond.desc.Primitive != ExCode.Bool && !cond.desc.Convert(ref cond, Descriptor.Bool))
							throw InvalidOperation("Could not convert '{0}' to boolean", cond.Name);

						op = ctx.BlockCode;
						var test = cond.num.Bool;
						vals.Pop();
						if (test != (op == OpCode.While || op == OpCode.For))
						{
							at = blockEnd;
							blockEnd = ctx.Pop();
							continue;
						}
						if (op != OpCode.For)
						{
							at += 4;
							continue;
						}
						at = ctx.BlockStart;
						continue;
					}
					#endregion

					#region Various statements (pop, yield)

					case OpCode.Pop:
						countdown++; // do not even count this instruction
						result = vals.Pop();
						if (ctx.BlockCode != OpCode.For)
							continue;
						if (at == ctx.BlockStart - 4)   // end of final expression
							at = ctx.BlockAt2;          // test expression
						else if (at == ctx.BlockAt2)	// end of init expression
							ctx.LockTop();
						continue;
					case OpCode.Yield:
						this.at = at;
						Exit = ExitCode.Yield;
						result = Value.Void;
						return false;
					#endregion

					#region Function

					case OpCode.Function:
					// byte      OpCode.Function
					// string    name (int index to strings)
					// int       size of header (number of following bytes)
					// ushort    type/access flags (reserved for methods)
					// byte      number of generic parameters
					// byte      number of arguments
					// int       size of return type
					// code      return type (e.g. OpCode.Void)
					// arguments - array of
					//   string    argument name (int index to strings)
					//   int       argument type size
					//   code      argument type (OpCode.Void for universal)
					//   int       argument default value size
					//   code      argument default value (OpCode.Void for none)
					// (end of header)
					// 
					// int       code size
					// code      code
					// int       tail size
					// int       number of captured variables
					// string[]  list of captured variables (indexes to strings)
					{
						// header
						var fnidx = Int(code, at);
						var fname = fnidx < 0 ? null : str[fnidx];
						at += 4;
						var size = Int(code, at);
						at += 4;
						var body = at + size;
						Debug.Assert(code[at + 2] == 0); // not generic
						at += 3;
						var argc = code[at++];
						var ftsz = Int(code, at);
						at += 4;
						var ftat = at;
						at += ftsz;
						var args = argc == 0 ? null : new ArgumentInfo[argc];
						for (var i = 0; i < argc; i++)
						{
							args[i].Name = str[Int(code, at)];
							at += 4;
							var tsz = Int(code, at);
							at += 4;
							args[i].Type = at;
							at += tsz;
							var vsz = Int(code, at);
							at += 4;
							args[i].Value = at;
							at += vsz;
						}

						// body
						Debug.Assert(at == body);
						at = body;
						size = Int(code, at);
						at += 4;
						int bodyAt = at;
						int bodySz = size;
						at += size;

						// tail
						size = Int(code, at);
						at += 4;
						HashSet<string> cvars = null;
						if (size >= 8)
						{
							int nvars = Int(code, at);
							cvars = new HashSet<string>();
							for (int i = 0, j = at+4; i < nvars; i++, j += 4)
								cvars.Add(str[Int(code, j)]);
						}
						at += size;

						var it = new Function(fname, null,
							compiled, bodyAt, bodySz, ftat, args, ctx, cvars, processor);
						if (fname?.Length > 0)
							ctx.Add(fname, it);
						else vals.Add(new Value(it));
						continue;
					}
					#endregion

					default:
						throw new NotImplementedException("Not implemented: " + op.ToString());
					}
				}
			finishNoReturn:
				Exit = ExitCode.None;
			finish:
				if (vals.Count > 0)
					result = vals.Pop();
				if (result.IsReference && !result.desc.Get(ref result, result.num.Int))
					throw CouldNotGet(ref result);
				vals.Clear();
				ctx.PopAll();
				Countdown = countdown;
				return true;
			}
			catch (Exception ex)
			{
				this.at = at;
				Exit = ExitCode.Exception;
				if (ex is RuntimeError re)
				{
					result = new Value(re);
					throw;
				}
				re = new RuntimeError(compiled, at, ex);
				result = new Value(re);
				throw re;
			}
		}
	}
}
