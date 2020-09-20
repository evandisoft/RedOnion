using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using RedOnion.Collections;
using RedOnion.ROS.Parsing;

namespace RedOnion.ROS
{
	#region Error (ParseError and RuntimeError)

	public class InvalidOperation : InvalidOperationException
	{
		public InvalidOperation() { }
		public InvalidOperation(string message) : base(message) { }
		public InvalidOperation(string message, params object[] args)
			: base(string.Format(Value.Culture, message, args)) { }
		public InvalidOperation(Exception inner) : base(inner.Message, inner) { }
		public InvalidOperation(Exception inner, string message) : base(message, inner) { }
		public InvalidOperation(Exception inner, string message, params object[] args)
			: base(string.Format(Value.Culture, message, args), inner) { }

		protected InvalidOperation(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

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
					lineNumber = Code?.FindLine(CodeAt-1) ?? -1;
					if (lineNumber < 0)
						lineNumber = 0;
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
		public int FindLine(int at)
		{
			if (LineMap == null || LineMap.Length == 0)
				return -1;
			int it = Array.BinarySearch(LineMap, at);
			if (it < 0)
			{
				it = ~it;
				if (it > 0)
					it--;
			}
			return it;
		}

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

	#region Arguments and related

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
	public enum CallFlags
	{
		None = 0,
		Create = 1<<0,
		Autocall = 1<<1,
		Implicit = 1<<2,
		Explicit = 1<<3,
		Convert = Implicit|Explicit
	}
	public readonly struct Arguments : IEnumerable<Value>
	{
		readonly ArgumentList list;
		public readonly int argc;
		public readonly CallFlags flags;
		public Processor Processor => list?.Processor;
		public Core Core => list?.Core;
		public int Length => argc;
		public int Count => argc;
		public int Size => argc;
		public bool Create => (flags & CallFlags.Create) != 0;
		public bool Autocall => (flags & CallFlags.Autocall) != 0;
		public bool Implicit => (flags & CallFlags.Implicit) != 0;
		public bool Explicit => (flags & CallFlags.Explicit) != 0;

		[DebuggerStepThrough]
		public Arguments(ArgumentList list, int argc, CallFlags flags)
		{
			this.list = list;
			this.argc = argc;
			this.flags = flags;
		}
		[DebuggerStepThrough]
		public Arguments(Arguments args, int argc, CallFlags flags)
		{
			this.list = args.list;
			this.argc = Math.Min(argc, args.argc);
			this.flags = flags;
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

	#region Interfaces for reflection

	/// <summary>
	/// Callable object (function which can also have properties).
	/// </summary>
	public interface ICallable
	{
		/// <summary>
		/// Try to call the object.
		/// Should not throw exceptions (return false instead,
		/// may be part of method group and different overload may be attempted).
		/// </summary>
		/// <param name="result">Both input (the callable) and output (the result)</param>
		/// <param name="self">Value of `this` (null for function/static call)</param>
		/// <param name="args">Arguments and <see cref="CallFlags"/></param>
		bool Call(ref Value result, object self, in Arguments args);
	}
	/// <summary>
	/// Object with custom operator implementation.
	/// </summary>
	public interface IOperators
	{
		/// <summary>
		/// Implements unary operators.
		/// Execution engine will directly only use <see cref="OpKind.Unary"/>
		/// but <see cref="Set(Core, ref Value, OpCode, ref Value)"/> may redirect
		/// <see cref="OpKind.PreOrPost"/> here (even from different descriptor).
		/// See <see cref="Descriptor.Unary(ref Value, OpCode)"/>
		/// </summary>
		/// <param name="self">Both input (<see cref="Value.IsReference"/>) and output</param>
		/// <param name="op">The operation (<see cref="OpKind.Unary"/> or <see cref="OpKind.PreOrPost"/>)</param>
		bool Unary(ref Value self, OpCode op);
		/// <summary>
		/// Implements binary operators (<see cref="OpKind.Binary"/> and <see cref="OpKind.Logic"/>
		/// except <see cref="OpCode.As"/> and higher).
		/// Should not throw exceptions (return false instead,
		/// the descriptor of second argument may handle it).
		/// See <see cref="Descriptor.Binary(ref Value, OpCode, ref Value)"/>.
		/// </summary>
		/// <param name="lhs">Both first argument and output</param>
		/// <param name="op"></param>
		/// <param name="rhs">Second argument (input only, do not modify)</param>
		bool Binary(ref Value lhs, OpCode op, ref Value rhs);
	}
	/// <summary>
	/// Object that is able to convert itself into different type.
	/// </summary>
	public interface IConvert
	{
		/// <summary>
		/// Try to convert the value to new type (<paramref name="to"/> could be <c>this</c>).
		/// Should not throw exceptions (return false instead).
		/// See <see cref="Descriptor.Convert(ref Value, Descriptor, bool)"/>
		/// </summary>
		/// <param name="self">The value to be converted</param>
		/// <param name="to">Target descriptor</param>
		/// <param name="flags">Conversion flags (currently only <see cref="CallFlags.Explicit"/> is relevant)</param>
		/// <returns>True if converted, false if not</returns>
		bool Convert(ref Value self, Descriptor to, CallFlags flags = CallFlags.Convert);
	}

	#endregion
}
