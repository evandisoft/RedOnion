using Bee.Run;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	public partial class Parser
	{
		/// <summary>
		/// parse type (reference, not declaration/definition)
		/// </summary>
		public Parser Type(Flag flags = Flag.None)
		{
			this.Type_(flags);
			return this;
		}
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Type_(Flag flags = Flag.None)
		{
			var bottom = OpsAt;
			var unary = true;
		next:
			var code = Opcode;
			switch (code.Kind())
			{
			case Opkind.Literal:
				switch (code)
				{
				case Opcode.Ident:
					if (!unary)
					{
						break;
					}
					if (Word.Length > 127)
					{
						throw new ParseError(this, "Identifier name too long");
					}
					if (Word == "object")
					{
						Cgen.Push(Opcode.Null);
					}
					else
					{
						Cgen.Push(Opcode.Ident, Word);
					}
					Next();
					unary = false;
					goto next;
				case Opcode.Object:
				case Opcode.String:
				case Opcode.Char:
				case Opcode.Wchar:
				case Opcode.Lchar:
					goto type;
				}
				break;
			case Opkind.Number:
			type:
				if (!unary)
				{
					break;
				}
				Cgen.Push(code);
				Next();
				unary = false;
				goto next;
			case Opkind.Special:
				switch (code)
				{
				case Opcode.Dot:
					if (unary)
					{
						break;
					}
					if (Next().Word == null)
					{
						throw new ParseError(this, "Expected word after '.'");
					}
					if (Word.Length > 127)
					{
						throw new ParseError(this, "Identifier name too long");
					}
					Cgen.Push(Opcode.Ident, Word);
					Cgen.Prepare(Opcode.Dot);
					Next();
					unary = false;
					goto next;
				case Opcode.Generic:
					if (unary)
					{
						break;
					}
					if (Next().Curr != ']')
					{
						for (;;)
						{
							Op(Opcode.Comma);
							Type(flags & (~Flag.Limit));
							if (Curr == ']')
							{
								break;
							}
							if (Curr != ',')
							{
								throw new ParseError(this, "Expected ',' or ']'");
							}
							Next();
						}
					}
					Cgen.Prepare(Opcode.Generic);
					Next();
					goto next;
				}
				break;
			case Opkind.Meta:
				if (Curr != '[')
				{
					break;
				}
				if (unary)
				{
					break;
				}
				if (Next().Curr != ']')
				{
					for (;;)
					{
						Op(Opcode.Comma);
						Expression(flags & (~Flag.Limit));
						if (Curr == ']')
						{
							break;
						}
						if (Curr != ',')
						{
							throw new ParseError(this, "Expected ',' or ']'");
						}
						Next();
					}
				}
				Cgen.Prepare(Opcode.Array);
				Next();
				goto next;
			}
			if (unary)
			{
				if (OpsAt > bottom)
				{
					Debug.Assert(false);
					throw new ParseError(this, "Unexpected state in type recognition (operators on stack when expecting unary)");
				}
				Cgen.Push(Opcode.Undef);
				return;
			}
			while (OpsAt > bottom)
			{
				Cgen.Prepare(Pop());
			}
		}
	}
}
