using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion
{
	public class TableCompletable : ICompletable
	{
		Table table;

		public TableCompletable(Table table)
		{
			this.table = table;
		}

		public IList<string> PossibleCompletions
		{
			get
			{
				List<string> possibleCompletions = new List<string>();

				foreach (var entry in table.Keys)
				{
					if (entry.Type != DataType.String)
					{
						continue;
					}

					possibleCompletions.Add(entry.String);
				}

				if (table.MetaTable != null && table.MetaTable is ICompletable)
				{
					var completable = table.MetaTable as ICompletable;
					possibleCompletions.AddRange(completable.PossibleCompletions);
				}

				possibleCompletions = possibleCompletions.Distinct().ToList();
				possibleCompletions.Sort();
				return possibleCompletions;
			}
		}

		public bool TryGetCompletion(string completionName, out object completion)
		{
			object newObject = table[completionName];
			if (newObject == null)
			{
				if (table.MetaTable != null && table.MetaTable is ICompletable)
				{
					var completable = table.MetaTable as ICompletable;
					if(completable.TryGetCompletion(completionName,out completion))
					{
						return true;
					}
				}
				else
				{
					completion = null;
					return false;
				}
			}

			completion = newObject;
			return true;
		}
	}
}
