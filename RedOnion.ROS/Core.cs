using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Parsing;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS
{
	public class Core : ICore
	{
		protected int at;
		protected byte[] code;
		protected string[] str;
		protected ArgumentList vals = new ArgumentList();
		protected Context ctx;
		protected Value self; // `this` for current code
		protected Value mself; // `this` for next method call
		protected Value result;

		public UserObject Globals { get; set; }
		public OpCode Exit { get; protected set; }
		public Value Result => result;
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
				mself = Value.Null;
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

		public void ResetContext() => ctx = null;
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
			try
			{
				while (at < code.Length)
				{
					if (countdown <= 0)
						return false;
					countdown--;
					var op = (OpCode)code[at++];
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
						if (op == OpCode.Identifier)
						{
							Identifier(at);
							at += 4;
							ref var it = ref vals.Top();
							if (it.desc.Call(ref it, ref mself, new Arguments(), true))
								continue;
							throw InvalidOperation("Could not create new {0}", it.Name);
						}
						if (op == OpCode.Call0)
						{
							ref var it = ref vals.Top();
							if (it.desc.Call(ref it, ref mself, new Arguments(), true))
								continue;
							throw InvalidOperation("Could not create new {0}", it.Name);
						}
						throw new NotImplementedException("Not implemented: OpCode.Create + " + op.ToString());

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
						if (ctx == null)
							ctx = new Context();
						var idx = ctx.Add(name, ref vals.Top());
						vals.Pop(1);
						ref var it = ref vals.Top();
						it.SetRef(ctx, idx);
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

					case OpCode.Autocall:
					{
						ref var it = ref vals.Top();
						if (it.IsReference && !it.desc.Get(ref it, it.num.Int))
							throw InvalidOperation(
								"Property '{0}' of '{1}' is write only",
								it.desc.NameOf(it.obj, it.num.Int), it.desc.Name);
						it.desc.Call(ref it, ref mself, new Arguments(), false);
						mself = Value.Null;
						continue;
					}

					default:
						throw new NotImplementedException("Not implemented: " + op.ToString());
					}
				}
				Exit = OpCode.Void;
				if (vals.Count == 0)
					result = Value.Void;
				else
				{
					result = vals.Pop();
					if (result.IsReference && !result.desc.Get(ref result, result.num.Int))
						throw InvalidOperation(
							"Property '{0}' of '{1}' is write only",
							result.desc.NameOf(result.obj, result.num.Int), result.desc.Name);
				}
				vals.Clear();
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
			int hi = Int(code, at+4);
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
	}
}
