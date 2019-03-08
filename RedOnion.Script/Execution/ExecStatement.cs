using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	public partial class Engine
	{
		protected override void Statement(OpCode op, ref int at)
		{
			CountStatement();
			Debug.Assert(op.Kind() == OpKind.Statement || op.Kind() == OpKind.Statement2);
			switch (op)
			{
			default:
				throw new NotImplementedException();
			case OpCode.Block:
				if ((Options & Option.BlockScope) != 0)
					Ctx.Push(this);
				Block(ref at);
				if ((Options & Option.BlockScope) != 0)
					Ctx.Pop();
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
				if ((Options & Option.BlockScope) != 0)
					Ctx.Push(this);
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
				if (Value.Type != ValueKind.Undefined && !Value.Bool)
				{
					at = cend;
					if ((Options & Option.BlockScope) != 0)
						Ctx.Pop();
					return;
				}
				for (;;)
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
				if ((Options & Option.BlockScope) != 0)
					Ctx.Pop();
				if (Exit == OpCode.Break || Exit == OpCode.Continue)
					Exit = 0;
				return;
			case OpCode.While:
			case OpCode.Until:
				if ((Options & Option.BlockScope) != 0)
					Ctx.Push(this);
				test = at;
				do
				{
					at = test;
					Expression(ref at);
					if (Value.Bool == (op == OpCode.Until))
						break;
					Block(ref at);
				}
				while (Exit == 0 || Exit == OpCode.Continue);
				if ((Options & Option.BlockScope) != 0)
					Ctx.Pop();
				if (Exit == OpCode.Break || Exit == OpCode.Continue)
					Exit = 0;
				return;
			case OpCode.Do:
			case OpCode.DoUntil:
				if ((Options & Option.BlockScope) != 0)
					Ctx.Push(this);
				do
				{
					Block(ref at);
					if (Exit != 0 && Exit != OpCode.Continue)
						break;
					Expression(ref at);
				}
				while (Value.Bool != (op == OpCode.DoUntil));
				if ((Options & Option.BlockScope) != 0)
					Ctx.Pop();
				if (Exit == OpCode.Break || Exit == OpCode.Continue)
					Exit = 0;
				return;
			case OpCode.If:
				if ((Options & Option.BlockScope) != 0)
					Ctx.Push(this);
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
				if ((Options & Option.BlockScope) != 0)
					Ctx.Pop();
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
				var size = CodeInt(ref at);
				var body = at + size;
				Debug.Assert(Code[at + 2] == 0); // not generic
				at += 3;
				var argc = Code[at++];
				var ftsz = CodeInt(ref at);
				var ftat = at;
				at += ftsz;
				var args = argc == 0 ? null : new ArgInfo[argc];
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
				Ctx.Root.Set(fname, new Value(Root.Create(Strings, Code, at, size, ftat, args, null, Ctx.Vars)));
				at += size;
				Value = new Value(Ctx.Root, fname);
				return;
			}
		}
	}
}
