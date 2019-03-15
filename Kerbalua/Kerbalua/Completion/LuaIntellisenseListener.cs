using System;
using System.Text;
using Antlr4.Runtime.Misc;
using Kerbalua.Utility;
using System.Collections.Generic;

namespace Kerbalua.Completion {
	public abstract class Part 
	{
		public string Symbol = "";
		public string Name = "";
		public override string ToString()
		{
			return Name+Symbol;
		}
	}
	public abstract class AnonPart:Part { }

	public abstract class NamedPart:Part { }

	public class NamedFieldPart : NamedPart { }
	public class NamedCallPart : NamedPart {
		public NamedCallPart() { Symbol = "()"; }
	}
	public class AnonArrayPart : AnonPart {
		public AnonArrayPart() { Symbol = "[]"; }
	}
	public class AnonCallPart : AnonPart {
		public AnonCallPart() { Symbol = "()"; }
	}

	public class Segment {
		public NamedPart NamedPart;
		public Stack<AnonPart> AnonParts = new Stack<AnonPart>();

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(NamedPart.ToString());
			foreach(var part in AnonParts) {
				sb.Append(part.ToString());
			}
			return sb.ToString();
		}
	}

	public class LuaIntellisenseListener : BackwardsLuaBaseListener {
		public string PartialCompletion = "";
		public bool HasStartSymbol = false;
		public string StartSymbol = "";
		public Stack<Segment> Segments = new Stack<Segment>();

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (HasStartSymbol) {
				sb.Append(StartSymbol);
				sb.Append(".");
			}
			foreach(var segment in Segments) {
				sb.Append(segment.ToString());
				sb.Append(".");
			}
			sb.Append(PartialCompletion);
			return sb.ToString();
		}

		public override void EnterSegment([NotNull] BackwardsLuaParser.SegmentContext context)
		{
			Segments.Push(new Segment());
		}

		public override void EnterBackwardsPartialCompletion([NotNull] BackwardsLuaParser.BackwardsPartialCompletionContext context)
		{
			PartialCompletion = Misc.ReverseString(context.BACKWARDS_NAME().ToString());
		}

		public override void EnterBackwardsCall([NotNull] BackwardsLuaParser.BackwardsCallContext context)
		{
			Segments.Peek().NamedPart = new NamedCallPart() {
				Name = Misc.ReverseString(context.BACKWARDS_NAME().ToString())
			};
		}

		public override void ExitBackwardsField([NotNull] BackwardsLuaParser.BackwardsFieldContext context)
		{
			Segments.Peek().NamedPart = new NamedFieldPart() {
				Name = Misc.ReverseString(context.BACKWARDS_NAME().ToString())
			};
		}

		public override void EnterBackwardsAnonCall([NotNull] BackwardsLuaParser.BackwardsAnonCallContext context)
		{
			Segments.Peek().AnonParts.Push(new AnonCallPart());
		}

		public override void EnterBackwardsAnonArray([NotNull] BackwardsLuaParser.BackwardsAnonArrayContext context)
		{
			Segments.Peek().AnonParts.Push(new AnonArrayPart());
		}

		public override void EnterBackwardsStartSymbol([NotNull] BackwardsLuaParser.BackwardsStartSymbolContext context)
		{
			HasStartSymbol = true;
			StartSymbol = Misc.ReverseString(context.BACKWARDS_NAME().ToString());
		}


	}
}
