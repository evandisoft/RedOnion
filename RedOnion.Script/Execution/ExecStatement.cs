using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	public partial class Engine<P>
	{
		protected override void Autocall(bool weak = false)
		{
			IObject self = null;
			if (Value.IsReference)
			{
				self = Value.ptr as IObject;
				Value = Result;
			}
			if (self == Root && HasOption(EngineOption.Strict))
				self = null;
			if (Value.Kind == ValueKind.Object)
			{
				var obj = (IObject)Value.ptr;
				if (obj.HasFeature(ObjectFeatures.Function))
					Value = obj.Call(self, 0);
				return;
			}
			if (weak || HasOption(EngineOption.WeakAutocall))
				return;
			throw new InvalidOperationException(string.Format(Value.Culture,
				"Expression as statement with no effect (weak autocall disabled, {0} is not a function)",
				Value.Name));
		}
		protected override void Statement(OpCode op, ref int at)
		{
			if (op == OpCode.Autocall)
			{
				Expression(ref at);
				if (!HasOption(EngineOption.Autocall|EngineOption.WeakAutocall|EngineOption.Repl))
					return;
				Autocall();
				return;
			}
			CountStatement();
			Debug.Assert(op.Kind() == OpKind.Statement || op.Kind() == OpKind.Statement2);
			switch (op)
			{
			default:
				throw new NotImplementedException();
			case OpCode.Block:
				if (HasOption(EngineOption.BlockScope))
					Context.Push(this);
				Block(ref at);
				if (HasOption(EngineOption.BlockScope))
					Context.Pop();
				return;
			case OpCode.Return:
			case OpCode.Raise:
				Expression(ref at);
				goto case OpCode.Break;
			case OpCode.Break:
			case OpCode.Continue:
				Exit = op;
				return;

			case OpCode.For:
				if (HasOption(EngineOption.BlockScope))
					Context.Push(this);
				Expression(ref at);
				var test = at;
				var notest = Code[at] == 0;
				if (notest)
					++at;
				else
					Expression(ref at);
				var size = CodeInt(ref at);
				var last = at;
				var stts = at + size;
				var cend = (stts + 4) + BitConverter.ToInt32(Code, stts);
				if (Value.Kind != ValueKind.Undefined && !Value.Bool)
				{
					at = cend;
					if (HasOption(EngineOption.BlockScope))
						Context.Pop();
					return;
				}
				for (;; CountStatement())
				{
					at = stts;
					Block(ref at);
					if (Exit != 0 && Exit != OpCode.Continue)
						break;
					at = last;
					Expression(ref at);
					if (!notest)
					{
						at = test;
						Expression(ref at);
						if (!Value.Bool)
							break;
					}
				}
				at = cend;
				if (HasOption(EngineOption.BlockScope))
					Context.Pop();
				if (Exit == OpCode.Break || Exit == OpCode.Continue)
					Exit = 0;
				return;
			case OpCode.ForEach:
				if (HasOption(EngineOption.BlockScope))
					Context.Push(this);
				Expression(ref at);
				var element = Value;
				Expression(ref at);
				var list = Value.Object as IEnumerable<Value>;
				if (list == null)
				{
					if (HasOption(EngineOption.Silent))
					{
						Value = new Value();
						goto end_foreach;
					}
					throw new InvalidOperationException("Not enumerable: " + Value.Name);
				}
				stts = at;
				foreach (var value in list)
				{
					at = stts;
					element.Set(value);
					Block(ref at);
					if (Exit != 0 && Exit != OpCode.Continue)
						break;
					CountStatement();
				}
			end_foreach:
				if (HasOption(EngineOption.BlockScope))
					Context.Pop();
				if (Exit == OpCode.Break || Exit == OpCode.Continue)
					Exit = 0;
				return;

			case OpCode.While:
			case OpCode.Until:
				if (HasOption(EngineOption.BlockScope))
					Context.Push(this);
				test = at;
				for(;; CountStatement())
				{
					at = test;
					Expression(ref at);
					if (Value.Bool == (op == OpCode.Until))
						break;
					Block(ref at);
					if (Exit != 0 && Exit != OpCode.Continue)
						break;
				}
				if (HasOption(EngineOption.BlockScope))
					Context.Pop();
				if (Exit == OpCode.Break || Exit == OpCode.Continue)
					Exit = 0;
				return;
			case OpCode.Do:
			case OpCode.DoUntil:
				if (HasOption(EngineOption.BlockScope))
					Context.Push(this);
				for(var start = at;; CountStatement())
				{
					at = start;
					Block(ref at);
					if (Exit != 0 && Exit != OpCode.Continue)
						break;
					Expression(ref at);
					if (Value.Bool == (op == OpCode.DoUntil))
						break;
				}
				if (HasOption(EngineOption.BlockScope))
					Context.Pop();
				if (Exit == OpCode.Break || Exit == OpCode.Continue)
					Exit = 0;
				return;

			case OpCode.If:
				if (HasOption(EngineOption.BlockScope))
					Context.Push(this);
				Expression(ref at);
				if (Value.Bool)
				{
					Block(ref at);
					if (at < Code.Length && Code[at] == OpCode.Else.Code())
					{
						at++;
						size = CodeInt(ref at);
						at += size;
					}
				}
				else
				{
					size = CodeInt(ref at);
					at += size;
					if (at < Code.Length && Code[at] == OpCode.Else.Code())
					{
						at++;
						Block(ref at);
					}
				}
				if (HasOption(EngineOption.BlockScope))
					Context.Pop();
				return;
			}
		}

		protected override void Other(OpCode op, ref int at)
		{
			switch (op)
			{
			default:
				throw new NotImplementedException();
			case OpCode.Function:
				var fname = Strings[CodeInt(ref at)];
				if (HasOption(EngineOption.Strict)
					&& !HasOption(EngineOption.Silent|EngineOption.Repl)
					&& Root.Has(fname))
					throw new InvalidOperationException("Function " + fname + " already exists");
				Context.Root.Set(fname, Function(fname, ref at));
				Value = new Value(Context.Root, fname);
				return;
			}
		}

		protected IObject Function(string name, ref int at)
		{
			var size = CodeInt(ref at);
			var body = at + size;
			Debug.Assert(Code[at + 2] == 0); // not generic
			at += 3;
			var argc = Code[at++];
			var ftsz = CodeInt(ref at);
			var ftat = at;
			at += ftsz;
			var args = argc == 0 ? null : new ArgumentInfo[argc];
			for (var i = 0; i < argc; i++)
			{
				args[i].Name = Strings[CodeInt(ref at)];
				var tsz = CodeInt(ref at);
				args[i].Type = at;
				at += tsz;
				var vsz = CodeInt(ref at);
				args[i].Value = at;
				at += vsz;
			}
			Debug.Assert(at == body);
			at = body;
			size = CodeInt(ref at);
			var it = Root.Create(name, Compiled, at, size, ftat, args, null, Context.Vars);
			at += size;
			return it;
		}
	}
}
