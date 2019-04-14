using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Parsing;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS
{
	[DebuggerDisplay("{DebugString}")]
	public class Core : ICore
	{
		protected int at;
		protected byte[] code;
		protected string[] str;
		protected ArgumentList vals = new ArgumentList();
		protected Context ctx;
		protected Value self; // `this` for current code
		protected Value result;
		protected struct SavedContext
		{
			public Context context;
			public Value prevSelf;
			public CompiledCode code;
			public int at, vtop;
			public bool create;
		}
		protected ListCore<SavedContext> stack;

		public UserObject Globals { get; set; }
		public OpCode Exit { get; protected set; }
		public Value Result => result;
		public int Countdown { get; set; }
		public ArgumentList Arguments => vals;

		protected CompiledCode compiled;
		public CompiledCode Code
		{
			get => compiled;
			protected set
			{
				at = 0;
				code = value?.Code;
				str = value?.Strings;
				vals.Clear();
				self = Value.Null;
				Exit = OpCode.Void;
				result = Value.Void;
				compiled = value;
			}
		}

		protected Parser Parser { get; } = new Parser();
		public CompiledCode Compile(string source, string path = null)
			=> Parser.Compile(source, path);

		public virtual void Log(string msg)
			=> Debug.Print(msg);
		public void Log(string msg, params object[] args)
			=> Log(string.Format(Value.Culture, msg, args));
		[Conditional("DEBUG")]
		public void DebugLog(string msg)
			=> Debug.Print(msg);
		[Conditional("DEBUG")]
		public void DebugLog(string msg, params object[] args)
			=> Log(string.Format(Value.Culture, msg, args));

		~Core() => Dispose(false);
		public void Dispose() => Dispose(true);
		protected virtual void Dispose(bool disposing) { }

		public void ResetContext() => ctx?.Reset();
		public bool Execute(string script, string path = null, int countdown = 1000)
		{
			Code = Compile(script, path);
			if (Globals == null) Globals = new Globals();
			return Execute(countdown);
		}
		public bool Execute(int countdown = 1000)
		{
			var at = this.at;
			var code = this.code;
			var str = this.str;
			int blockEnd = code.Length;
			if (ctx != null)
			{
				ctx.RootStart = 0;
				ctx.RootEnd = code.Length;
				blockEnd = ctx.BlockEnd;
			}
			try
			{
				for (; ; )
				{
					if (ctx != null)
					{
						while (at == blockEnd && ctx.BlockCount > 0)
						{
							if (countdown <= 0)
							{
								this.at = at;
								Exit = OpCode.Yield;
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
								ref var top = ref stack.Top();
								var vtop = top.vtop;
								ref var result = ref vals.GetRef(vals.Count, vtop - 1);
								if (top.create) result = self;
								vals.Pop(vals.Count - top.vtop);
								this.at = at = top.at;
								compiled = top.code;
								this.code = code = compiled.Code;
								this.str = str = compiled.Strings;
								self = top.prevSelf;
								ctx.PopAll();
								ctx = top.context;
								stack.Pop();
								continue;
							}
						}
					}
					if (countdown <= 0)
					{
						this.at = at;
						Exit = OpCode.Yield;
						Countdown = countdown;
						return false;
					}
					countdown--;
					if (at >= code.Length)
						break;
					var op = (OpCode)code[at++];
					this.at = at;
					switch (op)
					{
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
					case OpCode.UInt:
						vals.Add((uint)Int(code, at));
						at += 4;
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

					case OpCode.Create:
						op = (OpCode)code[at++];
						switch (op)
						{
						case OpCode.Identifier:
						{
							Identifier(at);
							at += 4;
							ref var it = ref vals.Top();
							if (it.IsReference && !it.desc.Get(ref it, it.num.Int))
								throw CouldNotGet(ref it);
							if (it.desc.Primitive == ExCode.Function)
							{
								this.at = at;
								blockEnd = CallFunction((Function)it.desc, null, null, 0, true);
								code = this.code;
								str = this.str;
								at = this.at;
								continue;
							}
							if (it.desc.Call(ref it, null, new Arguments(), true))
								continue;
							throw InvalidOperation("Could not create new {0}", it.Name);
						}
						case OpCode.Call0:
						{
							object self = null;
							Descriptor selfDesc = null;
							int idx = -1;
							ref var it = ref vals.Top();
							if (it.IsReference)
							{
								selfDesc = it.desc;
								self = it.obj;
								idx = it.num.Int;
								if (!it.desc.Get(ref it, idx))
									throw CouldNotGet(ref it);
							}
							if (it.desc.Primitive == ExCode.Function)
							{
								this.at = at;
								blockEnd = CallFunction((Function)it.desc, selfDesc, self, 0, true);
								code = this.code;
								str = this.str;
								at = this.at;
								continue;
							}
							if (it.desc.Call(ref it, self, new Arguments(), true))
								continue;
							throw InvalidOperation((self != null ? selfDesc.NameOf(self, idx) : it.Name)
								+ " cannot create object given zero arguments");
						}
						case OpCode.Call1:
						{
							Dereference(1);
							object self = null;
							Descriptor selfDesc = null;
							int idx = -1;
							ref var it = ref vals.Top(-2);
							if (it.IsReference)
							{
								selfDesc = it.desc;
								self = it.obj;
								idx = it.num.Int;
								if (!it.desc.Get(ref it, idx))
									throw CouldNotGet(ref it);
							}
							if (it.desc.Primitive == ExCode.Function)
							{
								this.at = at;
								blockEnd = CallFunction((Function)it.desc, selfDesc, self, 1, true);
								code = this.code;
								str = this.str;
								at = this.at;
								continue;
							}
							if (!it.desc.Call(ref it, self, new Arguments(Arguments, 1), true))
								throw InvalidOperation((self != null ? selfDesc.NameOf(self, idx) : it.Name)
									+ " cannot create object given that argument");
							vals.Pop(1);
							continue;
						}
						case OpCode.Call2:
						{
							Dereference(2);
							object self = null;
							Descriptor selfDesc = null;
							int idx = -1;
							ref var it = ref vals.Top(-3);
							if (it.IsReference)
							{
								selfDesc = it.desc;
								self = it.obj;
								idx = it.num.Int;
								if (!it.desc.Get(ref it, idx))
									throw CouldNotGet(ref it);
							}
							if (it.desc.Primitive == ExCode.Function)
							{
								this.at = at;
								blockEnd = CallFunction((Function)it.desc, selfDesc, self, 2, true);
								code = this.code;
								str = this.str;
								at = this.at;
								continue;
							}
							if (!it.desc.Call(ref it, self, new Arguments(Arguments, 2), true))
								throw InvalidOperation((self != null ? selfDesc.NameOf(self, idx) : it.Name)
									+ " cannot create object given these two arguments");
							vals.Pop(2);
							continue;
						}
						case OpCode.CallN:
						{
							var n = code[at++];
							Dereference(n-1);
							object self = null;
							Descriptor selfDesc = null;
							int idx = -1;
							ref var it = ref vals.Top(-n);
							if (it.IsReference)
							{
								selfDesc = it.desc;
								self = it.obj;
								idx = it.num.Int;
								if (!it.desc.Get(ref it, idx))
									throw CouldNotGet(ref it);
							}
							if (it.desc.Primitive == ExCode.Function)
							{
								this.at = at;
								blockEnd = CallFunction((Function)it.desc, selfDesc, self, n - 1, true);
								code = this.code;
								str = this.str;
								at = this.at;
								continue;
							}
							if (!it.desc.Call(ref it, self, new Arguments(Arguments, n - 1), true))
								throw InvalidOperation("{0} cannot create object given these {1} arguments",
									self != null ? selfDesc.NameOf(self, idx) : it.Name, n - 1);
							vals.Pop(n - 1);
							continue;
						}
						}
						throw new NotImplementedException("Not implemented: OpCode.Create + " + op.ToString());

					case OpCode.Autocall:
					case OpCode.Call0:
					{
						object self = null;
						Descriptor selfDesc = null;
						int idx = -1;
						ref var it = ref vals.Top();
						if (it.IsReference)
						{
							selfDesc = it.desc;
							self = it.obj;
							idx = it.num.Int;
							if (!it.desc.Get(ref it, idx))
								throw CouldNotGet(ref it);
						}
						if (it.desc.Primitive == ExCode.Function)
						{
							this.at = at;
							blockEnd = CallFunction((Function)it.desc, selfDesc, self, 0, false);
							code = this.code;
							str = this.str;
							at = this.at;
							continue;
						}
						if (it.desc.Call(ref it, self, new Arguments(), false))
							continue;
						if (op == OpCode.Autocall)
							continue;
						throw InvalidOperation((self != null ? selfDesc.NameOf(self, idx) : it.Name)
							+ " cannot be called with zero arguments");
					}
					case OpCode.Call1:
					{
						Dereference(1);
						object self = null;
						Descriptor selfDesc = null;
						int idx = -1;
						ref var it = ref vals.Top(-2);
						if (it.IsReference)
						{
							selfDesc = it.desc;
							self = it.obj;
							idx = it.num.Int;
							if (!it.desc.Get(ref it, idx))
								throw CouldNotGet(ref it);
						}
						if (it.desc.Primitive == ExCode.Function)
						{
							this.at = at;
							blockEnd = CallFunction((Function)it.desc, selfDesc, self, 1, false);
							code = this.code;
							str = this.str;
							at = this.at;
							continue;
						}
						if (!it.desc.Call(ref it, self, new Arguments(Arguments, 1), false))
							throw InvalidOperation((self != null ? selfDesc.NameOf(self, idx) : it.Name)
								+ " cannot be called with that argument");
						vals.Pop(1);
						continue;
					}
					case OpCode.Call2:
					{
						Dereference(2);
						object self = null;
						Descriptor selfDesc = null;
						int idx = -1;
						ref var it = ref vals.Top(-3);
						if (it.IsReference)
						{
							selfDesc = it.desc;
							self = it.obj;
							idx = it.num.Int;
							if (!it.desc.Get(ref it, idx))
								throw CouldNotGet(ref it);
						}
						if (it.desc.Primitive == ExCode.Function)
						{
							this.at = at;
							blockEnd = CallFunction((Function)it.desc, selfDesc, self, 2, false);
							code = this.code;
							str = this.str;
							at = this.at;
							continue;
						}
						if (!it.desc.Call(ref it, self, new Arguments(Arguments, 2), false))
							throw InvalidOperation((self != null ? selfDesc.NameOf(self, idx) : it.Name)
								+ " cannot be called with these two arguments");
						vals.Pop(2);
						continue;
					}
					case OpCode.CallN:
					{
						var n = code[at++];
						Dereference(n-1);
						object self = null;
						Descriptor selfDesc = null;
						int idx = -1;
						ref var it = ref vals.Top(-n);
						if (it.IsReference)
						{
							selfDesc = it.desc;
							self = it.obj;
							idx = it.num.Int;
							if (!it.desc.Get(ref it, idx))
								throw CouldNotGet(ref it);
						}
						if (it.desc.Primitive == ExCode.Function)
						{
							this.at = at;
							blockEnd = CallFunction((Function)it.desc, selfDesc, self, n - 1, false);
							code = this.code;
							str = this.str;
							at = this.at;
							continue;
						}
						if (!it.desc.Call(ref it, self, new Arguments(Arguments, n - 1), false))
							throw InvalidOperation("{0} cannot be called with these {1} arguments",
								self != null ? selfDesc.NameOf(self, idx) : it.Name, n - 1);
						vals.Pop(n - 1);
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
						//TODO: convert to hard index if we know the type for sure
						var name = str[Int(code, at)];
						at += 4;
						ref var it = ref vals.Top();
						if (it.IsReference && !it.desc.Get(ref it, it.num.Int))
							throw CouldNotGet(ref it);
						if (it.IsNumber)
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
						if (ctx == null)
							ctx = new Context(0, code.Length);
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
					case OpCode.LogicOr:
					case OpCode.LogicAnd:
					{
						ref var lhs = ref vals.Top();
						if (lhs.IsReference && !lhs.desc.Get(ref lhs, lhs.num.Int))
							throw CouldNotGet(ref lhs);
						if (lhs.desc.Primitive != ExCode.Bool && !lhs.desc.Convert(ref lhs, Descriptor.Bool))
							throw InvalidOperation("Could not convert '{0}' to boolean", lhs.Name);
						int sz = Int(code, at);
						at += 4;
						if (lhs.num.Bool == (op == OpCode.LogicOr))
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
							vals.Pop(vals.Count - top.vtop);
							this.at = at = top.at;
							compiled = top.code;
							this.code = code = compiled.Code;
							this.str = str = compiled.Strings;
							self = top.prevSelf;
							ctx.PopAll();
							ctx = top.context;
							stack.Pop();
							continue;
						}
						Exit = op;
						goto finish;

					case OpCode.Break:
						at = blockEnd;
						if (ctx == null)
							continue;
						while (ctx.BlockCount > 0
						&& (ctx.BlockCode < OpCode.For || ctx.BlockCode > OpCode.DoUntil))
							at = blockEnd = ctx.Pop();
						if (ctx.BlockCount > 0)
							ctx.BlockCode = OpCode.Block;
						continue;
					case OpCode.Continue:
					{
						if (ctx == null)
							throw InvalidOperation("No block to continue");
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
						if (ctx == null)
							ctx = new Context(0, code.Length);
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
						if (ctx == null)
							ctx = new Context(0, code.Length);
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
						if (ctx == null)
							ctx = new Context(0, code.Length);
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
						if (ctx == null)
							ctx = new Context(0, code.Length);
						ctx.Push(at, blockEnd = at + bsz + csz, op, at + bsz);
						continue;
					}

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
						if (ctx == null)
							ctx = new Context(0, code.Length);
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
							var enu = list.desc.Enumerate(ref list);
							if (enu == null)
								throw InvalidOperation(list.Name + " is not enumerable");
							list.desc = null;
							list.obj = enu;
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
					case OpCode.Pop:
						countdown++; // do not even count this instruction
						result = vals.Pop();
						if (ctx?.BlockCode == OpCode.For && at == ctx.BlockStart - 4)
							at = ctx.BlockAt2;
						continue;
					case OpCode.Yield:
						this.at = at;
						Exit = OpCode.Yield;
						result = Value.Void;
						return false;

					case OpCode.Function:
					{
						var fname = str[Int(code, at)];
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
						Debug.Assert(at == body);
						at = body;
						size = Int(code, at);
						at += 4;
						var it = new Function(fname, null,
							compiled, at, size, ftat, args, ctx);
						if (fname.Length != 0)
						{
							if (ctx == null)
								ctx = new Context(0, code.Length);
							ctx.Add(fname, it);
						}
						else vals.Add(new Value(it));
						at += size;
						continue;
					}

					default:
						throw new NotImplementedException("Not implemented: " + op.ToString());
					}
				}
				Exit = OpCode.Void;
			finish:
				if (vals.Count > 0)
					result = vals.Pop();
				if (result.IsReference && !result.desc.Get(ref result, result.num.Int))
					throw CouldNotGet(ref result);
				vals.Clear();
				ctx?.PopAll();
				Countdown = countdown;
				return true;
			}
			catch (Exception ex)
			{
				this.at = at;
				if (ex is RuntimeError)
					throw;
				throw new RuntimeError(compiled, at, ex);
			}
		}

		protected void Identifier(int at)
		{
			var code = this.code;
			int idx = Int(code, at);
			int found;
			string name;
			if (idx >= 0)
			{
				// encountered this for the first time
				// => try to find it in local variables
				name = str[idx];
				found = ctx?.Find(name) ?? -1;
				if (found >= 0)
				{
					// we found it to be local, mark it as such
					// and embed known index to speed things up
					// TODO: do that in parser/compiler
					idx = ~found;
					code[at++] = (byte)idx;
					code[at++] = (byte)(idx >> 8);
					code[at++] = (byte)(idx >> 16);
					code[at++] = (byte)(idx >> 24);
					ref var local = ref vals.Push();
					local.obj = local.desc = ctx;
					local.num = new Value.NumericData(found, idx);
					return;
				}
				// not local - the index may be changing,
				// but at least skip the search to this+globals
				// note: the method could be transfered to other objects
				// making this-index not reliable (and globals as well - libs)
				idx = ~(idx | 0x40000000);
				code[at++] = (byte)idx;
				code[at++] = (byte)(idx >> 8);
				code[at++] = (byte)(idx >> 16);
				code[at++] = (byte)(idx >> 24);
				// note: neither idx nor found is used later, only the name
				// TODO: use similar bound-index approach for strong types
			}
			else
			{
				// not the first time, let us decode it
				found = ~idx;
				if ((found & 0x40000000) == 0)
				{
					// local, marked previously, this speeds up loops
					ref var local = ref vals.Push();
					local.obj = local.desc = ctx;
					local.num = new Value.NumericData(found, idx);
					return;
				}
				// get back the original index and load the name
				idx = found & 0x3FFFFFFF;
				name = str[idx];
			}
			// try this first
			if (self.obj != null
			&& (found = self.desc.Find(self.obj, name, false)) >= 0)
			{
				ref var it = ref vals.Push();
				it.desc = self.desc;
				it.obj = self.obj;
				it.num = new Value.NumericData(found, ~found);
				return;
			}
			// try globals last
			if ((found = Globals?.Find(name) ?? -1) >= 0)
			{
				ref var it = ref vals.Push();
				it.obj = it.desc = Globals;
				it.num = new Value.NumericData(found, ~found);
				return;
			}
			throw InvalidOperation("Variable '{0}' not found", name);
		}

		protected void Dereference(int argc)
		{
			for (int i = 0; i < argc; i++)
			{
				ref var arg = ref vals.Top(i - argc);
				if (arg.IsReference && !arg.desc.Get(ref arg, arg.num.Int))
					throw CouldNotGet(ref arg);
			}
		}

		protected int CallFunction(Function fn, Descriptor selfDesc, object self, int argc, bool create)
		{
			ref var ret = ref stack.Add();
			ret.context = ctx;
			ret.prevSelf = this.self;
			ret.code = compiled;
			ret.at = at;
			ret.vtop = vals.Size - argc;
			ret.create = create;
			compiled = fn.Code;
			code = compiled.Code;
			str = compiled.Strings;
			at = fn.CodeAt;
			ctx = fn.Context;
			ctx.Push(at, at + fn.CodeSize, OpCode.Function);
			if (create)
				this.self = new Value(new UserObject(fn.Prototype));
			else this.self = new Value(selfDesc, self);
			var args = new Value[argc];
			for (int i = 0; i < argc; i++)
				args[i] = vals.Top(i - argc);
			ctx.Add("arguments", args);
			for (int i = 0; i < fn.ArgumentCount; i++)
			{
				if (i < argc)
					ctx.Add(fn.ArgumentName(i), ref args[i]);
				else ctx.Add(fn.ArgumentName(i), fn.ArgumentDefault(i));
			}
			result = Value.Void;
			return at + fn.CodeSize;
		}

		public static unsafe float Float(byte[] code, int at)
		{
			var v = Int(code, at);
			return *(float*)&v;
		}
		public static unsafe double Double(byte[] code, int at)
		{
			var v = Long(code, at);
			return *(double*)&v;
		}
		public static long Long(byte[] code, int at)
		{
			uint lo = (uint)Int(code, at);
			int hi = Int(code, at + 4);
			return lo | ((long)hi << 32);
		}
		public static int Int(byte[] code, int at)
		{
			int v = code[at++];
			v |= code[at++] << 8;
			v |= code[at++] << 16;
			return v | (code[at++] << 24);
		}
		public static short Short(byte[] code, int at)
		{
			int v = code[at++];
			return (short)(v | (code[at++] << 8));
		}

		static internal InvalidOperationException InvalidOperation(string msg)
			=> new InvalidOperationException(msg);
		static internal InvalidOperationException InvalidOperation(string msg, params object[] args)
			=> new InvalidOperationException(string.Format(Value.Culture, msg, args));
		static internal InvalidOperationException CouldNotGet(ref Value it)
			=> new InvalidOperationException(string.Format(Value.Culture,
			"Could not get '{0}' of '{1}'", it.desc.NameOf(it.obj, it.num.Int), it.desc.Name));

		private string DebugString
		{
			get
			{
				var code = this.compiled;
				if (code == null)
					return "null";
				var lineMap = code.LineMap;
				if (lineMap == null || lineMap.Length == 0)
					return "no line map";
				var lines = code.Lines;
				if (lines == null)
					return "no lines";
				int i = Array.BinarySearch(lineMap, at - 1);
				if (i < 0)
				{
					i = ~i;
					if (i > 0)
						i--;
				}
				return i >= 0 && lines != null && i < lines.Count ? lines[i].ToString() : "#" + i;
			}
		}
	}
}
