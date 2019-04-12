using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion
{
	public class OperationsProcessorTable : OperationsProcessor
	{
		static OperationsProcessorTable instance;
		static public OperationsProcessorTable Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new OperationsProcessorTable();
				}
				return instance;
			}
		}

		public override IList<string> GetPossibleCompletions(object obj)
		{
			var table = obj as Table;
			if (table == null)
			{
				throw new LuaIntellisenseException("For " + nameof(OperationsProcessorTable) + " object must be a table");
			}

			List<string> possibleCompletions = new List<string>();
			foreach(var key in table.Keys)
			{
				if (key.Type != DataType.String)
				{
					continue;
				}
				possibleCompletions.Add(key.String);
			}

			if (table.MetaTable != null) 
			{
				if(table.MetaTable is ICompletable)
				{
					var completable = table.MetaTable as ICompletable;
					possibleCompletions.AddRange(completable.PossibleCompletions);
				}
				else if(table.MetaTable is IHasCompletionProxy)
				{
					object obj2=GetProxy(table.MetaTable);
					possibleCompletions.AddRange(StaticGetPossibleCompletions(obj2));
				}
			}

			possibleCompletions = possibleCompletions.Distinct().ToList();

			return possibleCompletions;
		}

		public override bool TryProcessArrayAccess(object obj, CompletionOperations operation, out object outObj)
		{
			throw new LuaIntellisenseException("There is no ArrayAccess operation implementation for tables");
		}

		public override bool TryProcessCall(object obj, CompletionOperations operations, out object outObj)
		{
			throw new LuaIntellisenseException("There is no Call operation implementation for tables");
		}

		public override bool TryProcessGetMember(object obj, CompletionOperations operations, out object outObj)
		{
			var table = obj as Table;
			if (table == null)
			{
				throw new LuaIntellisenseException("For " + nameof(OperationsProcessorTable) + " object must be a table");
			}

			var getMember = operations.Current as GetMemberOperation;
			if (getMember == null)
			{
				throw new LuaIntellisenseException("For " + nameof(OperationsProcessorTable) + " "+nameof(TryProcessGetMember)+" must have current operation be a GetMember");
			}

			object newObject = table.RawGet(getMember.Name);

			if (newObject == null)
			{
				if (table.MetaTable != null)
				{
					if (table.MetaTable is ICompletable)
					{
						return TryProcessOperation(table.MetaTable as ICompletable, operations, out outObj);
					}
					if (table.MetaTable is IHasCompletionProxy)
					{
						return TryProcessOperation(table.MetaTable as IHasCompletionProxy, operations, out outObj);
					}
				}
				outObj = null;
				return false;
			}
			outObj = newObject;
			operations.MoveNext();
			return true;
		}
	}
}
