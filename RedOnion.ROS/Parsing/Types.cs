using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RedOnion.ROS.Parsing
{
	partial class Parser
	{
		protected virtual void ParseType(Flag flags = Flag.None)
		{
			var bottom = operators.size;
			var unary = true;
		next:
			var op = ExCode;
			switch (op.Kind())
			{
			case OpKind.Literal:    //====================================================== literal
				switch (op)
				{
				case ExCode.Identifier: //----------------------------------------------- identifier
					if (!unary)
						break;
					if (Word.Length > 127)
						throw new ParseError(this, "Identifier name too long");
					Push(ExCode.Identifier, Word);
					Next();
					unary = false;
					goto next;
				case ExCode.String:		//--------------------------------------------- string, char
				case ExCode.Char:
				case ExCode.WideChar:
				case ExCode.LongChar:
					goto type;
				}
				break;
			case OpKind.Number:		//------------------------------------------- bool / number type
			type:
				if (!unary)
					break;
				Push(op);
				Next();
				unary = false;
				goto next;
			case OpKind.Special:	//====================================================== special
				switch (op)
				{
				case ExCode.Dot:
					if (unary)
						break;
					if (Next().Word == null)
						throw new ParseError(this, "Expected word after '.'");
					if (Word.Length > 127)
						throw new ParseError(this, "Identifier name too long");
					Push(ExCode.Identifier, Word);
					PrepareOperator(ExCode.Dot, bottom);
					Next();
					unary = false;
					goto next;
				}
				break;
			case OpKind.Meta:
				if (Curr != '[')
					break;
				if (unary)
					break;
				if (Next(true).Curr != ']')
				{
					for (;;)
					{
						PushOperator(ExCode.Comma);
						ParseExpression((flags &~Flag.Limited) | Flag.Hungry);
						if (Curr == ']')
							break;
						if (Curr != ',')
							throw new ParseError(this, "Expected ',' or ']'");
						Next();
					}
				}
				PrepareOperator(ExCode.Array, bottom);
				Next();
				goto next;
			}
			if (unary)
			{
				if (operators.size > bottom)
				{
					Debug.Assert(false);
					throw new ParseError(this, "Unexpected state in type recognition (operators on stack when expecting unary)");
				}
				Push(ExCode.Void);
				return;
			}
			while (operators.size > bottom)
				PrepareOperator(PopOperator(), bottom);
		}
	}
}
