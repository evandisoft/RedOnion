using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using RedOnion.Script.Execution;

namespace RedOnion.Script.Parsing
{
	partial class Parser
	{
		protected virtual void ParseType(Flag flags = Flag.None)
		{
			var bottom = OperatorAt;
			var unary = true;
		next:
			var code = lexer.Code;
			switch (code.Kind())
			{
			case OpKind.Literal:    //====================================================== literal
				switch (code)
				{
				case OpCode.Identifier: //----------------------------------------------- identifier
					if (!unary)
						break;
					if (lexer.Word.Length > 127)
						throw new ParseError(lexer, "Identifier name too long");
					if (lexer.Word == "object")
						Push(OpCode.Null);
					else
						Push(OpCode.Identifier, lexer.Word);
					Next();
					unary = false;
					goto next;
				case OpCode.Object:		//------------------------------------- object, string, char
				case OpCode.String:
				case OpCode.Char:
				case OpCode.WideChar:
				case OpCode.LongChar:
					goto type;
				}
				break;
			case OpKind.Number:		//------------------------------------------- bool / number type
			type:
				if (!unary)
					break;
				Push(code);
				Next();
				unary = false;
				goto next;
			case OpKind.Special:	//====================================================== special
				switch (code)
				{
				case OpCode.Dot:
					if (unary)
						break;
					if (Next().lexer.Word == null)
						throw new ParseError(lexer, "Expected word after '.'");
					if (lexer.Word.Length > 127)
						throw new ParseError(lexer, "Identifier name too long");
					Push(OpCode.Identifier, lexer.Word);
					PrepareOperator(OpCode.Dot);
					Next();
					unary = false;
					goto next;
				case OpCode.Generic:
					if (unary) // maybe throw, but we leave it here for possible future extensions
						break;
					if (Next().lexer.Curr != ']')
					{
						for (;;)
						{
							PushOperator(OpCode.Comma);
							ParseType(flags &~Flag.LimitedContext);
							if (lexer.Curr == ']')
								break;
							if (lexer.Curr != ',')
								throw new ParseError(lexer, "Expected ',' or ']'");
							Next();
						}
					}
					PrepareOperator(OpCode.Generic);
					Next();
					goto next;
				}
				break;
			case OpKind.Meta:
				if (lexer.Curr != '[')
					break;
				if (unary)
					break;
				if (Next().lexer.Curr != ']')
				{
					for (;;)
					{
						PushOperator(OpCode.Comma);
						ParseExpression(flags &~Flag.LimitedContext);
						if (lexer.Curr == ']')
							break;
						if (lexer.Curr != ',')
							throw new ParseError(lexer, "Expected ',' or ']'");
						Next();
					}
				}
				PrepareOperator(OpCode.Array);
				Next();
				goto next;
			}
			if (unary)
			{
				if (OperatorAt > bottom)
				{
					Debug.Assert(false);
					throw new ParseError(lexer, "Unexpected state in type recognition (operators on stack when expecting unary)");
				}
				Push(OpCode.Undefined);
				return;
			}
			while (OperatorAt > bottom)
				PrepareOperator(PopOperator());
		}
	}
}
