using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RedOnion.ROS.Parsing;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS
{
	#region Error (ParseError and RuntimeError)

	/// <summary>
	/// Base class for ParseError and RuntimeError
	/// </summary>
	public abstract class Error : Exception
	{
		/// <summary>Line with error</summary>
		protected string line;
		/// <summary>Line with error</summary>
		public virtual string Line => line;

		/// <summary>Line number of error</summary>
		protected int lineNumber = -1;
		/// <summary>Line number of error</summary>
		public virtual int LineNumber => lineNumber;

		protected Error(string message) : base(message) { }
		protected Error(string message, params object[] args)
			: base(string.Format(Value.Culture, message, args)) { }
		protected Error(Exception inner) : base(inner.Message, inner) { }
		protected Error(Exception inner, string message) : base(message, inner) { }
		protected Error(Exception inner, string message, params object[] args)
			: base(string.Format(Value.Culture, message, args), inner) { }
	}

	/// <summary>
	/// Error during parsing
	/// </summary>
	public class ParseError : Error
	{
		/// <summary>
		/// Start of erroneous token/span
		/// </summary>
		public int At { get; protected set; }
		/// <summary>
		/// End of erroneous token/span
		/// </summary>
		public int End { get; protected set; }
		/// <summary>
		/// Column of erroneous token/span
		/// </summary>
		public int Column { get; protected set; }
		/// <summary>
		/// Line number of end of erroneous token/span
		/// </summary>
		public int EndLine { get; protected set; }
		/// <summary>
		/// Character offset from the start of the source
		/// </summary>
		public int CharCounter { get; protected set; }

		private void Init(Scanner scanner)
		{
			line = scanner.Line;
			EndLine = lineNumber = scanner.LineNumber;
			At = scanner.At;
			End = scanner.End;
			Column = scanner.Column;
			CharCounter = scanner.CharCounter;
		}

		public ParseError(Scanner scanner, string message)
			: base(message)
			=> Init(scanner);
		public ParseError(Scanner scanner, string message, params object[] args)
			: base(message, args)
			=> Init(scanner);
		public ParseError(Scanner scanner, Exception inner)
			: base(inner)
			=> Init(scanner);
		public ParseError(Scanner scanner, Exception inner, string message)
			: base(inner, message)
			=> Init(scanner);
		public ParseError(Scanner scanner, Exception inner, string message, params object[] args)
			: base(inner, message, args)
			=> Init(scanner);
	}

	/// <summary>
	/// Error during execution
	/// </summary>
	public class RuntimeError : Error
	{
		/// <summary>
		/// The compiled code that caused the error
		/// </summary>
		public CompiledCode Code { get; }
		/// <summary>
		/// The position in the code the processor was at when the error occured.
		/// (The position is usually just after the instruction when the error is raised)
		/// </summary>
		public int CodeAt { get; }

		public override int LineNumber
		{
			get
			{
				if (lineNumber < 0)
				{
					lineNumber = 0;
					if (Code?.LineMap != null && Code.LineMap.Length > 0)
					{
						int it = Array.BinarySearch(Code.LineMap, CodeAt-1);
						if (it < 0)
						{
							it = ~it;
							if (it > 0)
								it--;
						}
						lineNumber = it;
					}
				}
				return lineNumber;
			}
		}

		private SourceLine? sourceLine;
		public SourceLine SourceLine
		{
			get
			{
				if (!sourceLine.HasValue)
				{
					var i = LineNumber;
					if (i >= 0 && Code?.Lines != null && i < Code.Lines.Count)
						sourceLine = Code.Lines[i];
					else sourceLine = new SourceLine();
				}
				return sourceLine.Value;
			}
		}
		public override string Line => SourceLine.Text;
		public int Position => SourceLine.Position;

		public RuntimeError(CompiledCode code, int at, Exception innerException, string message)
			: base(message ?? innerException.Message, innerException)
		{
			Code = code;
			CodeAt = at;
		}
		public RuntimeError(CompiledCode code, int at, Exception innerException)
			: base(innerException)
		{
			Code = code;
			CodeAt = at;
		}
		public RuntimeError(CompiledCode code, int at, string message)
			: base(message)
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

	#endregion

	#region CompiledCode and SourceLine

	/// <summary>
	/// Compiled code (at least string table and byte code)
	/// with possibly other references
	/// </summary>
	public class CompiledCode
	{
		public CompiledCode(string[] strings, byte[] code, int[] lineMap, bool prefix = false)
		{
			Strings = strings;
			Code = code;
			LineMap = lineMap;
		}
		/// <summary>
		/// String table
		/// </summary>
		public string[] Strings { get; }
		/// <summary>
		/// Compiled code
		/// </summary>
		public byte[] Code { get; }
		/// <summary>
		/// Index to Code for each line
		/// </summary>
		public int[] LineMap { get; }

		/// <summary>
		/// Prefix notation of the code is good for analyzers,
		/// code generators and recursive execution.
		/// Postfix notation is good for execution with pause-continue.
		/// </summary>
		public bool Prefix { get; }

		/// <summary>
		/// Path to source file
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// Source content
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Source content separated to lines
		/// </summary>
		public IList<SourceLine> Lines { get; set; }
	}
	/// <summary>
	/// Line of source content (with position and text)
	/// </summary>
	[DebuggerDisplay("{Position}: {Text}")]
	public struct SourceLine
	{
		public int Position;
		public string Text;
		public SourceLine(int position, string text)
		{
			Position = position;
			Text = text;
		}
		public override string ToString()
			=> string.Format(Value.Culture, "{0}: {1}", Position, Text);
	}

	#endregion

	#region Processor, Arguments and related

	/// <summary>
	/// The root of scripting engine, usually the main core
	/// </summary>
	public interface IProcessor
	{
		/// <summary>
		/// Event invoked on engine shutdown or reset.
		/// (All subscriptions are removed prior to executing the handlers.)
		/// </summary>
		event Action Shutdown;
		/// <summary>
		/// Event invoked on every physics update (Unity FixedUpdate).
		/// </summary>
		event Action PhysicsUpdate;
		/// <summary>
		/// Event invoked on every graphic update (Unity Update).
		/// </summary>
		event Action GraphicUpdate;

		/// <summary>
		/// Invoked by print function
		/// </summary>
		void Print(string msg);

		/// <summary>
		/// Log message
		/// </summary>
		void Log(string msg);
	}

	/// <summary>
	/// Exit code (of last statement, code block or whole program)
	/// </summary>
	public enum ExitCode : sbyte
	{
		/// <summary>
		/// End of block/program reached without return statement
		/// </summary>
		None = 0,
		/// <summary>
		/// Return statement
		/// </summary>
		Return = 1,
		/// <summary>
		/// Yield/wait statement
		/// </summary>
		Yield = 2,
		/// <summary>
		/// Countdown reached
		/// </summary>
		Countdown = 3,

		/// <summary>
		/// Exception (raise/throw statement)
		/// </summary>
		Exception = -1,
	}

	/// <summary>
	/// Argument name, type and default value for functions
	/// </summary>
	public struct ArgumentInfo
	{
		public string Name;
		public int Type;
		public int Value;
	}

	/// <summary>
	/// Argument list for function calls
	/// </summary>
	[DebuggerDisplay("{list.DebugString}")]
	public class ArgumentList
	{
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		protected ListCore<Value> list;
		public Core Core { get; }
		public Processor Processor => Core?.Processor;
		public ArgumentList(Core core)
			=> Core = core;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Length => list.Count;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Count => list.Count;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Size => list.Count;

		public Value Get(int argc, int index = 0)
		{
			var idx = Count - argc + index;
			return idx < Count ? list[idx] : new Value();
		}
		public ref Value GetRef(int argc, int index = 0)
		{
			var idx = Count - argc + index;
			return ref list.items[idx];
		}
		public void Add(Value value)
			=> list.Add(value);
		public void Push(Value value)
			=> list.Add(value);
		public void Add(ref Value value)
			=> list.Add(value);
		public void Push(ref Value value)
			=> list.Add(value);
		public ref Value Add()
			=> ref list.Add();
		public ref Value Push()
			=> ref list.Add();
		public ref Value Top()
			=> ref list.items[list.size-1];
		public ref Value Top(int fromEnd)
			=> ref list.items[list.size+fromEnd];
		public Value Pop()
			=> list.Pop();
		public void Pop(int count)
			=> list.Count -= count;
		public void Clear()
			=> list.Clear();

		public readonly struct AddGuard : IDisposable
		{
			readonly ArgumentList arglist;
			readonly int startSize;
			public AddGuard(ArgumentList arglist)
				=> startSize = (this.arglist = arglist).Count;
			public void Dispose()
				=> arglist.list.Count = startSize;
		}
		public AddGuard Guard()
			=> new AddGuard(this);
	}
	public readonly struct Arguments : IEnumerable<Value>
	{
		readonly ArgumentList list;
		public readonly int argc;
		public Processor Processor => list?.Processor;
		public Core Core => list?.Core;
		public int Length => argc;
		public int Count => argc;
		public int Size => argc;
		public Arguments(ArgumentList list, int argc)
		{
			this.list = list;
			this.argc = argc;
		}
		public Arguments(Arguments args, int argc)
		{
			this.list = args.list;
			this.argc = Math.Min(argc, args.argc);
		}

		public Value this[int i] => i >= argc ? Value.Void : list.Get(argc, i);
		public ref Value GetRef(int i) => ref list.GetRef(argc, i);
		public IEnumerator<Value> GetEnumerator()
		{
			for (int i = 0; i < argc; i++)
				yield return list.Get(argc, i);
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public Value[] ToArray()
		{
			var arr = new Value[argc];
			for (int i = 0; i < argc; i++)
				arr[i] = list.Get(argc, i);
			return arr;
		}
	}

	#endregion

	#region Attributes and interfaces for reflection

	/// <summary>
	/// Callable object (function which can also have properties).
	/// </summary>
	public interface ICallable
	{
		bool Call(ref Value result, object self, Arguments args, bool create);
	}
	/// <summary>
	/// Object with custom operator implementation.
	/// </summary>
	public interface IOperators
	{
		bool Unary(ref Value self, OpCode op);
		bool Binary(ref Value lhs, OpCode op, ref Value rhs);
	}
	/// <summary>
	/// Object that is able to convert itself into different type.
	/// </summary>
	public interface IConvert
	{
		bool Convert(ref Value self, Descriptor to);
	}
	/// <summary>
	/// Interface for type system ('is' and 'as' operators).
	/// </summary>
	public interface IType
	{
		Type Type { get; }
		bool IsType(object type);
	}

	/// <summary>
	/// Marker for potentially dangerous API.
	/// </summary>
	public class UnsafeAttribute : Attribute { }

	/// <summary>
	/// Alternative name for a member (if name not null),
	/// or readonly string field with path to implementation.
	/// </summary>
	public class AliasAttribute : Attribute
	{
		public string Name { get; }
		public AliasAttribute() { }
		public AliasAttribute(string name) => Name = name;
	}

	/// <summary>
	/// Link from instance to creator (ROS object to function).
	/// </summary>
	public class CreatorAttribute : Attribute
	{
		public Type Creator { get; }
		public CreatorAttribute(Type creator)
			=> Creator = creator;
	}

	/// <summary>
	/// Convert values to specified type (when presenting to script)
	/// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.ReturnValue|AttributeTargets.Parameter)]
	public class ConvertAttribute : Attribute
	{
		public Type Type { get; }
		public ConvertAttribute(Type type) => Type = type;
	}

	//TODO: use this in reflected descriptors to disable modification

	/// <summary>
	/// Make the value read-only for the script (disable writes and most methods)
	/// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.ReturnValue)]
	public class ReadOnlyContent : Attribute
	{
		public bool ContentIsReadOnly { get; }
		public ReadOnlyContent() => ContentIsReadOnly = true;
		public ReadOnlyContent(bool contentIsReadOnly) => ContentIsReadOnly = contentIsReadOnly;
	}
	/// <summary>
	/// Make items of collection read-only for the script (disable writes and most methods)
	/// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.ReturnValue)]
	public class ReadOnlyItems : Attribute
	{
		public bool ItemsAreReadOnly { get; }
		public ReadOnlyItems() => ItemsAreReadOnly = true;
		public ReadOnlyItems(bool itemsAreReadOnly) => ItemsAreReadOnly = itemsAreReadOnly;
	}

	#endregion
}
