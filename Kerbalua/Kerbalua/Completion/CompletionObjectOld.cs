using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using System.Reflection;
using System.Linq;
using RedOnion.KSP.Completion;

namespace Kerbalua.Completion {
	public class CompletionObjectOld {
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

		public CompletionObjectOld(Table globals, IList<Segment> segments)
		{
			CurrentCompletionObject = globals;
			this.segments = segments;
		}

		public void ProcessCompletion()
		{
			while (ProcessNextSegment()) { }
		}

		public bool ProcessNextSegment()
		{
			if (Index >= segments.Count - 1) {
				return false;
			}

			switch (CurrentCompletionObject)
			{
				case ICompletable completable:
					SetNextResultFromICompletable(completable);
					break;
				case Table table:
					SetNextResultFromTable(table);
					break;
				case Type type:
					SetNextResultFromType(type);
					break;
				default:
					SetNextResultFromType(CurrentCompletionObject.GetType());
					break;
			}
			ProcessCurrentParts();
			Index++;
			return true;
		}

		private void SetNextResultFromICompletable(ICompletable completable)
		{
			Segment currentSegment = segments[Index];
			if (completable.TryGetCompletion(currentSegment.Name,out object completion))
			{
				CurrentCompletionObject = completion;
				return;
			}

			throw new LuaIntellisenseException("ICompletable did not output a completion for " + currentSegment.Name);
		}

		public IList<string> GetCurrentCompletions()
		{
			switch (CurrentCompletionObject)
			{
				case ICompletable completable:
					return GetCurrentICompletableCompletions(completable);
				case Table table:
					return GetCurrentTableCompletions(table);
				case Type type:
					return GetCurrentTypeCompletions(type);
				default:
					return GetCurrentTypeCompletions(CurrentCompletionObject.GetType());
			}
		}

		IList<string> FilterAndSortCompletions(IList<string> possibleCompletions)
		{
			string lowercaseCompletion = CurrentPartial.ToLower();
			var completions = new List<string>();
			foreach(var possibleCompletion in possibleCompletions)
			{
				if (possibleCompletion.ToLower().Contains(lowercaseCompletion))
				{
					completions.Add(possibleCompletion);
				}
			}
			completions.Sort();
			return completions;
		}

		private IList<string> GetCurrentICompletableCompletions(ICompletable completable)
		{
			return FilterAndSortCompletions(completable.PossibleCompletions);
		}

		void ThrowErrorIfHasParts()
		{
			Segment currentSegment = segments[Index];

			if (currentSegment.Parts.Count > 0)
			{
				Type partType = currentSegment.Parts[0].GetType();

				if (partType == typeof(CallPart))
				{
					throw new LuaIntellisenseException("Intellisense cannot handle call in this context");
				}

				if (partType == typeof(ArrayPart))
				{
					throw new LuaIntellisenseException("Intellisenes cannot handle array access in this context");
				}

				throw new LuaIntellisenseException("Unknown part type.");
			}
		}

		void SetNextResultFromTable(Table table)
		{
			Segment currentSegment = segments[Index];
			string name = currentSegment.Name;

			object newObject = table[name];
			if (newObject == null) {
				if(table.MetaTable!=null && table.MetaTable is ICompletable) 
				{
					SetNextResultFromICompletable(table.MetaTable as ICompletable);
				}
				else
				{
					throw new KeyNotInTableException();
				}
			}
			else if (newObject is ICompletable)
			{
				CurrentCompletionObject = newObject;
			}
			else if (newObject is Table) {
				CurrentCompletionObject = newObject;
			}
			else
			{
				DynValue dynValue = table.Get(name);

				CurrentCompletionObject = dynValue.UserData.Descriptor.Type;
			}
		}

		void ProcessCurrentParts()
		{
			switch (CurrentCompletionObject)
			{
				case ICompletable completable:
				case Table table:
					ThrowErrorIfHasParts();
					return;
			}

			Segment currentSegment = segments[Index];
			IList<Part> parts = currentSegment.Parts;
			var currentType = CurrentCompletionObject as Type;
			if (currentType == null)
			{
				currentType = CurrentCompletionObject.GetType();
			}
			if (parts.Count == 0) {
				return;
			}
			for(int i = 0;i < parts.Count;i++) { 
				if (parts[i] is CallPart) {
					MethodInfo mi = currentType.GetMethod("Invoke");
					if (mi == null) {
						throw new LuaIntellisenseException(
							"No invoke method for Part " + i+" segment "+Index +
							" in type "+currentType);
					}
					currentType = currentType.GetMethod("Invoke").ReturnType;
				} else if (parts[i] is ArrayPart) {
					PropertyInfo pi = currentType.GetProperty("Item");
					if (pi == null) {
						throw new LuaIntellisenseException(
							"No 'Item' property for Part " + i + " segment " + Index +
							" in type "+currentType);
					}
					currentType = pi.PropertyType;
				}
			}
		}

		void SetNextResultFromType(Type type)
		{
			Segment currentSegment = segments[Index];
			string currentName = currentSegment.Name;
			IList<Part> parts = currentSegment.Parts;
			if(parts.Count>0 && parts[0] is CallPart) {
				MethodInfo mi=null;
				try {
					mi = type.GetMethod(currentName);

				} catch(AmbiguousMatchException) {
					MethodInfo[] mis=type.GetMethods();
					foreach(var methodInfo in mis) {
						if (methodInfo.Name == currentName) {
							mi = methodInfo;
							break;
						}
					}
				}
				if (mi != null) {
					type = mi.ReturnType;
					parts.RemoveAt(0);
					ProcessCurrentParts();
					Index++;
					return;
				}
			}

			var p = type.GetProperty(currentName);
			if (p != null) {
				type = p.PropertyType;
				ProcessCurrentParts();
				Index++;
				return;
			}
			var f = type.GetField(currentName);
			if (f == null) {
				throw new LuaIntellisenseException(
					"No field, property, or method of name " + 
					currentName + " in segment " + Index +" for type "+type);
			}
			type = f.FieldType;
			return;
		}

		IList<string> GetCurrentTypeCompletions(Type type)
		{
			IList<string> completions 
				= FilterAndSortCompletions(ListAllMembers(type).ToList());

			return completions;
		}

		IList<string> GetCurrentTableCompletions(Table table)
		{
			List<string> completions = new List<string>();
			string partial = CurrentPartial;

			foreach (var entry in table.Keys) {
				if (entry.Type != DataType.String) {
					continue;
				}

				if (entry.String.ToLower().Contains(partial.ToLower())) {
					completions.Add(entry.String);
				}
			}

			if(table.MetaTable!=null && table.MetaTable is ICompletable)
			{
				var completable = table.MetaTable as ICompletable;
				var possibleCompletions = completable.PossibleCompletions;
				foreach(var possibleCompletion in possibleCompletions)
				{
					if (partial.ToLower().Contains(possibleCompletion.ToLower()))
					{
						completions.Add(possibleCompletion);
					}
				}
			}

			completions = completions.Distinct().ToList();
			completions.Sort();
			return completions;
		}

		static public HashSet<string> ListAllMembers(Type t)
		{
			var strs = new HashSet<string>();
			foreach (var member in t.GetMembers()) {
				if (member.Name.Contains("_")) {
					strs.Add(member.Name.Split('_')[1]);
				} else {
					strs.Add(member.Name);
				}
			}
			return strs;
		}
	}
}
