using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using System.Reflection;
using System.Linq;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion {
	public class CompletionObject {
		public object StartObject;
		public object CurrentCompletionObject;
		IList<Segment> segments;

		public string CurrentPartial {
			get {
				if (segments.Count == 0) {
					return "";
				}

				return segments[Index].Name;
			}
		}

		public int Index { get; private set; } = 0;

		public CompletionObject(object StartObject, IList<Segment> segments)
		{
			this.StartObject = StartObject;
			this.segments = segments;
			Reset();
		}

		public void Reset()
		{
			CurrentCompletionObject = StartObject;
			Index = 0;
		}

		public void ProcessAllSegments()
		{
			while (ProcessNextSegment()) { }
		}

		public bool ProcessNextSegment()
		{
			if (Index >= segments.Count - 1) {
				return false;
			}

			ICompletable currentCompletable = GetICompletable(CurrentCompletionObject);

			if(currentCompletable.TryGetCompletion(segments[Index]
				.Name,out object completion))
			{
				if(ShouldNotHaveParts(completion) && segments[Index].Parts.Count > 0)
				{
					return false;
				}

				CurrentCompletionObject = completion;

				Index++;
				return true;
			}

			return false;
		}

		bool ShouldNotHaveParts(object completable)
		{
			return completable is Table || completable is ICompletable;
		}

		ICompletable GetICompletable(object obj)
		{
			int limit = 100;
			int i = 0;
			for(i = 0; i < limit; i++)
			{
				if(obj is IHasCompletionProxy proxy)
				{
					obj = proxy.CompletionProxy;
				}
				else
				{
					break;
				}
			}
			if (i >= limit)
			{
				throw new LuaIntellisenseException("Could not resolve proxies at " + segments[Index].Name);
			}

			switch (obj)
			{
				case ICompletable completable:
					return completable;
				case Table table:
					return new TableCompletable(table);
				case Type type:
					return new TypeCompletable(type, segments[Index]);
				default:
					return new TypeCompletable(CurrentCompletionObject.GetType(), segments[Index]);
			}
		}

		public IList<string> GetCurrentCompletions()
		{
			return FilterAndSortCompletions(
				GetICompletable(CurrentCompletionObject)
					.PossibleCompletions);
		}

		IList<string> FilterAndSortCompletions(
			IList<string> possibleCompletions)
		{
			string lowercaseCompletion = CurrentPartial.ToLower();
			var completions = new List<string>();
			foreach(var possibleCompletion in possibleCompletions)
			{
				if (possibleCompletion.ToLower()
					.Contains(lowercaseCompletion))
				{
					completions.Add(possibleCompletion);
				}
			}
			completions.Sort();
			return completions;
		}
	}
}
