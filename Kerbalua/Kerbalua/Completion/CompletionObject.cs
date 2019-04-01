using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using System.Reflection;
using RedOnion.KSP.Lua.Proxies;
using RedOnion.KSP.API;
using System.Linq;

namespace Kerbalua.Completion {
	public class CompletionObject {
		public Table CurrentTable { get; private set; }
		public Type CurrentType { get; private set; }
		public InteropObject CurrentInterop { get; private set; }
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

		public CompletionObject(Table globals, IList<Segment> segments)
		{
			CurrentTable = globals;
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

			if (CurrentTable != null) {
				SetNextResultFromTable();
				return true;
			}

			if (CurrentType != null) {
				SetNextResultFromType();
				return true;
			}

			if (CurrentInterop != null)
			{
				SetNextResultFromInterop();
				return true;
			}

			throw new Exception("Both currentTable and currentType are null");
		}

		public IList<string> GetCurrentCompletions()
		{
			if (CurrentTable != null) {
				return GetCurrentTableCompletions();
			}
			if (CurrentType != null) {
				return GetCurrentTypeCompletions();
			}
			if (CurrentInterop != null)
			{
				return GetCurrentInteropCompletions();
			}

			throw new Exception("Both currentTable and currentType are null");
		}

		private IList<string> GetCurrentInteropCompletions()
		{
			List<string> completions = new List<string>();
			string partial = CurrentPartial;
			foreach(var member in CurrentInterop.Members)
			{
				if (member.Name.ToLower().Contains(partial.ToLower()))
				{
					completions.Add(member.Name);
				}
			}
			completions.Sort();
			return completions;
		}

		private void SetNextResultFromInterop()
		{
			Segment currentSegment = segments[Index];
			string name = currentSegment.Name;
			var member = CurrentInterop.Members.Where((m) => m.Name == name).FirstOrDefault();
			if (member != default(IMember))
			{
				if (member is Native native)
				{
					CurrentInterop= null;
					CurrentType = native.Get().GetType();
					ProcessCurrentParts();
					Index++;
					return;
				}
				if (member is InteropObject interop)
				{
					CurrentInterop = interop;
					if (currentSegment.Parts.Count > 0)
					{
						Type partType = currentSegment.Parts[0].GetType();

						if (partType == typeof(CallPart))
						{
							throw new InteropCallException();
						}

						if (partType == typeof(ArrayPart))
						{
							throw new InteropArrayAccessException();
						}

						throw new LuaIntellisenseException("Unknown part type.");
					}
					Index++;
					return;
				}
			}
			throw new LuaIntellisenseException(name + " not found in " + CurrentInterop.Name);
		}

		void SetNextResultFromTable()
		{
			Segment currentSegment = segments[Index];
			string name = currentSegment.Name;
			object newObject = CurrentTable[name];
			if (newObject == null) {
				if(CurrentTable.MetaTable!=null && CurrentTable.MetaTable is Globals globals)
				{
					var member=globals.Members.Where((m) => m.Name == name).FirstOrDefault();
					if (member != default(IMember))
					{
						if(member is Native native)
						{
							CurrentTable = null;
							CurrentType = native.Get().GetType();
							ProcessCurrentParts();
							Index++;
							return;
						}
						if(member is InteropObject interop)
						{
							CurrentTable = null;
							CurrentInterop = interop;
							if (currentSegment.Parts.Count > 0)
							{
								Type partType = currentSegment.Parts[0].GetType();

								if (partType == typeof(CallPart))
								{
									throw new InteropCallException();
								}

								if (partType == typeof(ArrayPart))
								{
									throw new InteropArrayAccessException();
								}

								throw new LuaIntellisenseException("Unknown part type.");
							}
							Index++;
							return;
						}
					}
				}
				throw new KeyNotInTableException();
			}

			if (newObject is ProxyTable proxyTable)
			{
				CurrentTable = null;
				CurrentType = proxyTable.ProxiedObject.GetType();
				ProcessCurrentParts();
				Index++;
				return;
			}

			if (newObject is Table newTable) {
				// Function calls cannot happen on tables and
				// array access requires evaluating expressions, which we
				// will not do in order to avoid side affects.
				// We will only follow explicit member access.
				if (currentSegment.Parts.Count > 0) {
					Type partType = currentSegment.Parts[0].GetType();

					if (partType == typeof(CallPart)) {
						throw new TableCallException();
					}

					if (partType == typeof(ArrayPart)) {
						throw new TableArrayAccessException();
					}

					throw new LuaIntellisenseException("Unknown part type.");
				}

				CurrentType = null;
				CurrentTable = newTable;
				Index++;
				return;
			}

			CurrentTable = null;
			CurrentType = newObject.GetType();
			ProcessCurrentParts();
			Index++;
		}

		void ProcessCurrentParts()
		{
			Segment currentSegment = segments[Index];
			IList<Part> parts = currentSegment.Parts;
			if (parts.Count == 0) {
				return;
			}
			for(int i = 0;i < parts.Count;i++) { 
				if (parts[i] is CallPart) {
					MethodInfo mi = CurrentType.GetMethod("Invoke");
					if (mi == null) {
						throw new LuaIntellisenseException(
							"No invoke method for Part " + i+" segment "+Index +
							" in type "+CurrentType);
					}
					CurrentType = CurrentType.GetMethod("Invoke").ReturnType;
				} else if (parts[i] is ArrayPart) {
					PropertyInfo pi = CurrentType.GetProperty("Item");
					if (pi == null) {
						throw new LuaIntellisenseException(
							"No 'Item' property for Part " + i + " segment " + Index +
							" in type "+CurrentType);
					}
					CurrentType = pi.PropertyType;
				}
			}
		}

		void SetNextResultFromType()
		{
			CurrentTable = null;
			Segment currentSegment = segments[Index];
			string currentName = currentSegment.Name;
			IList<Part> parts = currentSegment.Parts;
			if(parts.Count>0 && parts[0] is CallPart) {
				MethodInfo mi=null;
				try {
					mi = CurrentType.GetMethod(currentName);

				} catch(AmbiguousMatchException) {
					MethodInfo[] mis=CurrentType.GetMethods();
					foreach(var methodInfo in mis) {
						if (methodInfo.Name == currentName) {
							mi = methodInfo;
							break;
						}
					}
				}
				if (mi != null) {
					CurrentType = mi.ReturnType;
					parts.RemoveAt(0);
					ProcessCurrentParts();
					Index++;
					return;
				}
			}

			var p = CurrentType.GetProperty(currentName);
			if (p != null) {
				CurrentType = p.PropertyType;
				ProcessCurrentParts();
				Index++;
				return;
			}
			var f = CurrentType.GetField(currentName);
			if (f == null) {
				throw new LuaIntellisenseException(
					"No field, property, or method of name " + 
					currentName + " in segment " + Index +" for type "+CurrentType);
			}
			CurrentType = f.FieldType;
			ProcessCurrentParts();
			Index++;
			return;
		}

		IList<string> GetCurrentTypeCompletions()
		{
			List<string> completions = new List<string>();
			string partial = CurrentPartial;
			foreach (var memberName in ListAllMembers(CurrentType)) {
				if (memberName.ToLower().Contains(partial.ToLower())) {
					completions.Add(memberName);
				}
			}

			completions.Sort();
			return completions;
		}

		IList<string> GetCurrentTableCompletions()
		{
			List<string> completions = new List<string>();
			string partial = CurrentPartial;

			foreach (var entry in CurrentTable.Keys) {
				if (entry.Type != DataType.String) {
					continue;
				}

				if (entry.String.ToLower().Contains(partial.ToLower())) {
					completions.Add(entry.String);
				}
			}

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
