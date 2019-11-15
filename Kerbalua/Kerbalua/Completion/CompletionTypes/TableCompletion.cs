using System;
using System.Collections.Generic;
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

		public override IList<string> StaticGetPossibleCompletions()
		{
			throw new NotImplementedException();
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

			DynValue dynValue = table.RawGet(getMember.Name);

			if (dynValue == null)
			{
				CompletionQueue.Log("dynvalue is null");
				if (table.MetaTable != null)
				{
					CompletionQueue.Log("MetaTable is not null");
					completionObject=GetCompletionObject(table.MetaTable);
					var index=table.MetaTable.RawGet("__index");
					// If the index of the MetaTable is another table, we're safe
					if (completionObject is TableCompletion && index!=null && index.Table!=null)
					{
						completionObject=new TableCompletion(index.Table);
						return true;
					}
					// If it is not TableCompletion, its completion is handled another way
					if(!(completionObject is TableCompletion))
					{
						return true;
					}
				}
				completionObject = null;
				return false;
			}

			completionObject=GetCompletionObject(dynValue);
			return true;
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new NotImplementedException();
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new NotImplementedException();
		}
	}
}
