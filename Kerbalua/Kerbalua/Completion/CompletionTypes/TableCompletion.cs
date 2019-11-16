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
			if (metatable == null)
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
				if (!(completionObject is TableCompletion))
				{
					return true;
				}
			}
			completionObject=null;
			return false;
		}

		public bool TryTableGet(string key, out CompletionObject completionObject)
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

			return TryTableGet(getMember.Name, out completionObject);
		}

		public override bool TryCall(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new NotImplementedException();
		}

		public override bool TryArrayAccess(CompletionOperations operations, out CompletionObject completionObject)
		{
			throw new NotImplementedException();
			//PotentialFeature: This will only be doable after we're parsing out the contents
			// of the array access and discovering that it is a primitive.
			// We can have the parser figure out whether it is a string, number.
			// could also check if it was a global variable.

			//if (table == null)
			//{
			//	throw new LuaIntellisenseException("Table was null in "+GetType());
			//}

			//var getMember = operations.Current as GetMemberOperation;
			//if (getMember == null)
			//{
			//	throw new LuaIntellisenseException("getMember was null in TryGetMember for "+GetType());
			//}

			//return TryTableGet(getMember.Name, out completionObject);
		}
	}
}
