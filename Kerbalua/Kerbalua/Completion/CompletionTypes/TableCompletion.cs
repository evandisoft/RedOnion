using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion.CompletionTypes
{
	public class TableCompletion:CompletionObject
	{
		public Table table;

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

			CompletionQueue.Log("TryResolveMetatable");
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
				CompletionQueue.Log("This metatable is not null");
				completionObject=GetCompletionObject(metatable);
				var index=metatable.RawGet("__index");
				if (completionObject is TableCompletion && index!=null && index.Table!=null)
				{
					CompletionQueue.Log("This metatable's index is safe.");
					completionObject=new TableCompletion(index.Table);
					return true;
				}

				if (!(completionObject is TableCompletion))
				{
					CompletionQueue.Log("This metatable's completion is handled another way.");
					return true;
				}
				CompletionQueue.Log("The metatable is not safe to use for completion");
			}
			completionObject=null;
			return false;
		}

		public bool TryTableGet(object key, out CompletionObject completionObject)
		{
			DynValue dynValue = table.RawGet(key);

			if (dynValue == null)
			{
				CompletionQueue.Log("dynvalue is null");
				return TryResolveMetatable(table.MetaTable, out completionObject);
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

			var str=getArrayAccess.exp.@string();
			if (str!=null)
			{
				if(TryTableGet(str, out completionObject))
				{
					operations.MoveNext();
					return true;
				}
			}
			var num=getArrayAccess.exp.number();
			if (num!=null)
			{
				if(TryTableGet(num, out completionObject))
				{
					operations.MoveNext();
					return true;
				}
			}

			throw new LuaIntellisenseException("ArraAccessCompletion for Tables is only implemented for num and str");
		}
	}
}
