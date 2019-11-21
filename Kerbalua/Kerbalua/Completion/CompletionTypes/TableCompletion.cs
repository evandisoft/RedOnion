using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using RedOnion.KSP.Completion;
using static RedOnion.KSP.Debugging.QueueLogger;

namespace Kerbalua.Completion.CompletionTypes
{
	public class TableCompletion:CompletionObject
	{
		public Table table;

		public override string ToString()
		{
			return "TableCompletion("+table?.GetType().Name+")";
		}

		public TableCompletion(Table table)
		{
			this.table=table;
		}

		public override IList<string> GetPossibleCompletions()
		{
			if (table == null)
			{
				throw new LuaIntellisenseException("For " + nameof(TableCompletion) + " table was null");
			}

			List<string> possibleCompletions = new List<string>();
			foreach (var key in table.Keys)
			{
				if (key.Type != DataType.String)
				{
					continue;
				}
				possibleCompletions.Add(key.String);
			}

			Compl.Log("TryResolveMetatable");
			if (TryResolveMetatable(table.MetaTable, out CompletionObject completionObject))
			{
				IList<string> metatableCompletions=completionObject.GetPossibleCompletions();
				possibleCompletions.AddRange(metatableCompletions);
			}

			possibleCompletions = possibleCompletions.Distinct().ToList();

			return possibleCompletions;
		}

		bool TryResolveMetatable(Table metatable, out CompletionObject completionObject)
		{
			if (metatable != null)
			{
				Compl.Log("This metatable is not null");
				completionObject=GetCompletionObject(metatable);
				var index=metatable.RawGet("__index");
				if (completionObject is TableCompletion && index!=null && index.Table!=null)
				{
					Compl.Log("This metatable's index is safe.");
					completionObject=new TableCompletion(index.Table);
					return true;
				}

				if (!(completionObject is TableCompletion))
				{
					Compl.Log("This metatable's completion is handled another way.");
					return true;
				}
				Compl.Log("The metatable is not safe to use for completion");
			}
			completionObject=null;
			return false;
		}

		public bool TryTableGet(object key, out CompletionObject completionObject)
		{
			DynValue dynValue = table.RawGet(key);

			if (dynValue == null)
			{
				Compl.Log("dynvalue is null");
				TryResolveMetatable(table.MetaTable, out completionObject);
				return false;
			}

			completionObject=GetCompletionObject(dynValue);
			return true;
		}

		public override bool TryGetMember(CompletionOperations operations, out CompletionObject completionObject)
		{
			if (table == null)
			{
				throw new LuaIntellisenseException("Table was null in "+GetType());
			}

			var getMember = operations.Current as GetMemberOperation;
			if (getMember == null)
			{
				throw new LuaIntellisenseException("getMember was null in TryGetMember for "+GetType());
			}

			if(TryTableGet(getMember.Name, out completionObject))
			{
				operations.MoveNext();
				return true;
			}
			if (completionObject!=null)
			{
				return true;
			}

			return false;
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new LuaIntellisenseException("TableCompletion does not implement TryCall");
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			// PotentialFeature: We could potentially use variable names as well.

			if (table == null)
			{
				throw new LuaIntellisenseException("Table was null in "+GetType());
			}

			var getArrayAccess = operations.Current as ArrayAccessOperation;
			if (getArrayAccess == null)
			{
				throw new LuaIntellisenseException("getArrayAccess was null in TryArrayAccess for "+GetType());
			}

			Compl.Log("Trying array access");

			Compl.Log("exp was {"+getArrayAccess.exp?.GetText()+"}");
			var strNode=getArrayAccess.exp.@string();
			if (strNode!=null)
			{
				var stringLiteral=strNode.NORMALSTRING() ?? strNode.LONGSTRING() ?? strNode.CHARSTRING();

				string str=stringLiteral.ToString();

				str=str.Substring(1, str.Length-2);

				if (TryTableGet(str, out completionObject))
				{
					operations.MoveNext();
					return true;
				}
				if (completionObject!=null)
				{
					return true;
				}
			}
			var num=getArrayAccess.exp.number();
			if (num!=null)
			{
				var t=num.INT() ?? num.FLOAT() ?? num.HEX_FLOAT() ?? num.HEX();
				if (TryTableGet(float.Parse(t.ToString()), out completionObject))
				{
					operations.MoveNext();
					return true;
				}
				if (completionObject!=null)
				{
					return true;
				}
			}

			throw new LuaIntellisenseException("ArraAccessCompletion for Tables is only implemented for num and str");
		}
	}
}
