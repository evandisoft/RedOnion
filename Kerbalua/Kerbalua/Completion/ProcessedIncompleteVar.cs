using System;
using System.Text;
using System.Collections.Generic;

namespace Kerbalua.Completion {
	public class ProcessedIncompleteVar {
		public bool Success = false;
		public List<Segment> Segments = new List<Segment>();

		public ProcessedIncompleteVar(IncompleteLuaParser.IncompleteVarContext incompleteVar)
		{
			if (incompleteVar == null) {
				return;
			}
			ProcessIncompleteVar(incompleteVar);
		}

		string ProcessIncompleteName(IncompleteLuaParser.IncompleteNameContext incompleteName)
		{
			if (incompleteName.NAME() == null) {
				return incompleteName.keyword().GetText();
			}
			return incompleteName.NAME().ToString();
		}

		void ProcessIncompleteVar(IncompleteLuaParser.IncompleteVarContext incompleteVar)
		{
			if (incompleteVar.incompleteName()!=null) {
				Segments.Add(new Segment() { Name = ProcessIncompleteName(incompleteVar.incompleteName()) });

				Success = true;
				return;
			}

			if (incompleteVar.varName() == null) {
				return;
			}

			Segments.Add(new Segment() { Name = incompleteVar.varName().NAME().ToString() });
			foreach(var suffix in incompleteVar.varSuffix()) {
				ProcessVarSuffix(suffix);
			}

			ProcessIncompleteVarSuffix(incompleteVar.incompleteVarSuffix());
		}

		void ProcessVarSuffix(IncompleteLuaParser.VarSuffixContext varSuffix)
		{
			foreach (var nameAndArgs in varSuffix.nameAndArgs()) {
				ProcessNameAndArgs(nameAndArgs);
			}

			if (varSuffix.exp() != null) {
				Segments[Segments.Count - 1].Parts.Add(new ArrayPart());
			} else {
				Segments.Add(new Segment() { Name = varSuffix.NAME().ToString() });
			}
		}

		void ProcessNameAndArgs(IncompleteLuaParser.NameAndArgsContext nameAndArgs)
		{
			if (nameAndArgs.NAME() != null) {
				Segments.Add(new Segment() { Name = nameAndArgs.NAME().ToString() });
			}

			Segments[Segments.Count - 1].Parts.Add(new CallPart());
		}

		void ProcessIncompleteVarSuffix(IncompleteLuaParser.IncompleteVarSuffixContext incompleteVarSuffix)
		{
			foreach (var nameAndArgs in incompleteVarSuffix.nameAndArgs()) {
				ProcessNameAndArgs(nameAndArgs);
			}

			if (incompleteVarSuffix.incompleteExp() != null) {
				return;
			}

			if (incompleteVarSuffix.incompleteName() == null) {
				Segments.Add(new Segment() { Name = "" });
			} else {
				Segments.Add(new Segment() { Name = ProcessIncompleteName(incompleteVarSuffix.incompleteName()) });
			}
			Success = true;
		}
	}


	public abstract class Part {
		public string Symbol = "";
		public override string ToString()
		{
			return Symbol;
		}
	}

	public class ArrayPart : Part {
		public ArrayPart() { Symbol = "[]"; }
	}
	public class CallPart : Part {
		public CallPart() { Symbol = "()"; }
	}

	public class Segment {
		public string Name = "";
		public List<Part> Parts = new List<Part>();

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Name);
			foreach (var part in Parts) {
				sb.Append(part.ToString());
			}
			return sb.ToString();
		}
	}
}
