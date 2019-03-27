using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	public interface IErrorWithLine
	{
		int LineNumber { get; }
		string Line { get; }
	}
	public class RuntimeError : Exception, IErrorWithLine
	{
		public CompiledCode Code { get; }
		public int CodeAt { get; }

		private int _lineNumber = -1;
		public int LineNumber
		{
			get
			{
				if (_lineNumber < 0)
				{
					_lineNumber = 0;
					if (Code?.LineMap != null && Code.LineMap.Length > 0)
					{
						int it = Array.BinarySearch(Code.LineMap, CodeAt-1);
						if (it < 0)
						{
							it = ~it;
							if (it > 0)
								it--;
							_lineNumber = it;
						}
					}
				}
				return _lineNumber;
			}
		}

		private CompiledCode.SourceLine? sourceLine;
		public CompiledCode.SourceLine SourceLine
		{
			get
			{
				if (!sourceLine.HasValue)
				{
					var i = LineNumber;
					if (i >= 0 && Code?.Lines != null && i < Code.Lines.Count)
						sourceLine = Code.Lines[i];
					else sourceLine = new CompiledCode.SourceLine();
				}
				return sourceLine.Value;
			}
		}
		public string Line => SourceLine.Text;
		public int Position => SourceLine.Position;

		public RuntimeError(CompiledCode code, int at, Exception innerException, string message)
			: base(message ?? innerException.Message, innerException)
		{
			Code = code;
			CodeAt = at;
		}
		public RuntimeError(CompiledCode code, int at, Exception innerException)
			: base(innerException.Message, innerException)
		{
			Code = code;
			CodeAt = at;
		}
		public RuntimeError(CompiledCode code, int at, string message)
			: base(message, null)
		{
			Code = code;
			CodeAt = at;
		}
		public RuntimeError(CompiledCode code, int at, string message, params object[] args)
			: base(string.Format(Value.Culture, message, args), null)
		{
			Code = code;
			CodeAt = at;
		}
	}
}
