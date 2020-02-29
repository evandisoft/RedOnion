using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MunOS.Repl
{
	public class OutputBuffer : IReadOnlyCollection<OutputBuffer.Record>
	{
		/// <summary>
		/// Output type. Can be casted to character (except for Info=0).
		/// </summary>
		public enum Type
		{
			Info = 0,
			Input = 'i',
			File = 'f',
			Return = 'r',
			Output = 'o',
			Error = 'e',
		}
		/// <summary>
		/// One output record (single or multiple-lines submitted at once).
		/// </summary>
		public readonly struct Record
		{
			/// <summary>Time the record was created.</summary>
			public readonly DateTime time;
			/// <summary>Type of the record.</summary>
			public readonly Type type;
			/// <summary>Text of the record (including all lines).</summary>
			public readonly string text;
			/// <summary>Number of lines.</summary>
			public readonly int lineCount;
			/// <summary>Number of characters.</summary>
			public int charCount => text.Length;

			public Record(Type type, string text, int lineCount = 1)
			{
				time = DateTime.Now;
				this.type = type;
				this.text = text;
				this.lineCount = lineCount;
			}
		}

		protected readonly Queue<Record> records = new Queue<Record>();
		protected readonly StringBuilder builder = new StringBuilder();
		protected string outputText;
		public int RecordCounter { get; protected set; }
		public int LineCount { get; protected set; }
		public int CharCount => builder.Length;
		int lineLimit, charLimit;
		public int LineLimit { get => lineLimit; set { lineLimit = value; CheckLimits(); } }
		public int CharLimit { get => charLimit; set { charLimit = value; CheckLimits(); } }
		protected bool newOutput;

		public OutputBuffer(int lineLimit = 256, int charLimit = 10240)
		{
			LineLimit = lineLimit;
			CharLimit = charLimit;
		}
		protected void CheckLimits()
		{
			while (LineCount > lineLimit || CharCount > charLimit)
			{
				var rec = records.Dequeue();
				LineCount -= rec.lineCount;
				builder.Remove(0, rec.charCount);
			}
		}

		public string GetString(out bool isNewOutput)
		{
			isNewOutput = newOutput;
			if (newOutput)
			{
				outputText = builder.ToString();
				newOutput = false;
			}
			return outputText;
		}

		public void Clear()
		{
			records.Clear();
			LineCount = 0;
			builder.Clear();
			newOutput = true;
		}

		static protected readonly char[] newLines = new char[] { '\r', '\n' };
		public void Add(Type type, string str)
		{
			var at = builder.Length;
			builder.Append('\n');
			if (type != Type.Info)
			{
				builder.Append((char)type);
				builder.Append("> ");
			}
			var ln = str.IndexOfAny(newLines);
			if (ln < 0)
			{
				builder.Append(str);
				records.Enqueue(new Record(type, builder.ToString(at, builder.Length - at)));
				LineCount++;
			}
			else
			{
				builder.Append(str.Substring(0, ln));
				int lineCount = 1;
				for (; ; )
				{
					ln++;
					var next = str.IndexOfAny(newLines, ln);
					if (next == ln && str[next] != str[ln-1])
					{
						ln++;
						next = str.IndexOfAny(newLines, ln);
					}
					lineCount++;
					builder.Append('\n');
					if (type != Type.Info)
					{
						builder.Append((char)type);
						builder.Append("> ");
					}
					if (next < 0)
					{
						builder.Append(str.Substring(ln));
						break;
					}
					builder.Append(str.Substring(ln, next-ln));
					ln = next;
				}
				records.Enqueue(new Record(type, builder.ToString(at, builder.Length - at), lineCount));
				LineCount += lineCount;
			}
			RecordCounter++;
			newOutput = true;
			CheckLimits();
		}
		public void AddText(string str)
			=> Add(Type.Info, str);
		public void AddReturnValue(string str)
			=> Add(Type.Return, str);
		public void AddOutput(string str)
			=> Add(Type.Output, str);
		public void AddError(string str)
			=> Add(Type.Error, str);
		public void AddSourceString(string str)
			=> Add(Type.Input, str);
		public void AddFileContent(string str)
			=> Add(Type.File, str);

		public int Count => records.Count;
		public IEnumerator<Record> GetEnumerator() => records.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => records.GetEnumerator();
	}
}
