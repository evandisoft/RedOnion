using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	/// <summary>
	/// Method of creating objects,
	/// usually for late/lazy creation of objects and properties.
	/// </summary>
	/// <param name="engine">The engine to associate the object with</param>
	/// <returns>The object</returns>
	public delegate IObject CreateObject(IEngine engine);

	[Flags]
	public enum EngineOption
	{
		None = 0,
		/// <summary>
		/// Variables live only inside blocks
		/// (otherwise inside script or function)
		/// </summary>
		BlockScope = 1 << 0,
		/// <summary>
		/// Make errors/exceptions produce undefined value where possible
		/// (throw exception otherwise)
		/// </summary>
		Silent = 1 << 1,
		/// <summary>
		/// Strict mode sets `this` in functions (not called as method) to null (global otherwise)
		/// </summary>
		Strict = 1 << 2,
		/// <summary>
		/// Anonymous functions (created by Function(args, body))
		/// expose their body (script code).
		/// </summary>
		FuncText = 1 << 31,
	}

	public interface IEngine
	{
		/// <summary>
		/// Engine options
		/// </summary>
		EngineOption Options { get; }
		/// <summary>
		/// Root object (global namespace)
		/// </summary>
		IEngineRoot Root { get; }

		/// <summary>
		/// Exit code (of last statement, code block or whole program)
		/// </summary>
		OpCode Exit { get; }
		/// <summary>
		/// Result of last expression (rvalue)
		/// </summary>
		Value Result { get; }

		/// <summary>
		/// Reset engine
		/// </summary>
		void Reset();
		/// <summary>
		/// Compile source to code
		/// </summary>
		CompiledCode Compile(string source);
		/// <summary>
		/// Run script
		/// </summary>
		void Execute(CompiledCode code, int at, int size);
		/// <summary>
		/// Evaluate expression
		/// </summary>
		Value Evaluate(CompiledCode code, int at);

		/// <summary>
		/// Box value (StringObj, NumberObj, ...)
		/// </summary>
		IObject Box(Value value);

		/// <summary>
		/// Argument list for function calls
		/// </summary>
		ArgumentList Arguments { get; }
		/// <summary>
		/// Create new variables holder object
		/// </summary>
		IObject CreateVars(IObject vars);
		/// <summary>
		/// Create new execution/activation context (for function call)
		/// </summary>
		IObject CreateContext(IObject self, IObject scope = null);
		/// <summary>
		/// Destroy last execution/activation context
		/// </summary>
		Value DestroyContext();

		/// <summary>
		/// Log message
		/// </summary>
		void Log(string msg);
	}

	[Flags]
	public enum ObjectFeatures
	{
		None = 0,
		/// <summary>
		/// Is function / does implement Call
		/// </summary>
		Function = 1 << 1,
		/// <summary>
		/// Is constructor / does implement Create
		/// </summary>
		Constructor = 1 << 2,
		/// <summary>
		/// Is collection (array, list, dictionary) / does implement Index
		/// </summary>
		Collection = 1 << 3,
		/// <summary>
		/// Can convert native objects into script object
		/// (e.g. BasicObjects.StringFun, usually comes with Function flag)
		/// </summary>
		Converter = 1 << 4,
		/// <summary>
		/// Represents native type (usually comes with Contstructor flag)
		/// </summary>
		TypeReference = 1 << 5,
		/// <summary>
		/// Is proxy/wrapper of native object
		/// </summary>
		Proxy = 1 << 6
	}

	/// <summary>
	/// Property interface (single property with custom access methods)
	/// </summary>
	/// <remarks>Can only be hosted in read-only properties (not in dynamic)</remarks>
	public interface IProperty
	{
		/// <summary>
		/// Get value of this property
		/// </summary>
		Value Get(IObject self);

		/// <summary>
		/// Set value of this property
		/// </summary>
		/// <returns>False if not set (e.g. read-only)</returns>
		bool Set(IObject self, Value value);

	}

	/// <summary>
	/// Enhanced property interface (single property with custom access methods)
	/// </summary>
	/// <remarks>Can only be hosted in read-only properties (not in dynamic)</remarks>
	public interface IPropertyEx: IProperty
	{
		/// <summary>
		/// Modify the value of this property (compound assignment)
		/// </summary>
		bool Modify(IObject self, OpCode op, Value value);
	}

	/// <summary>
	/// Properties interface (collection of properties)
	/// </summary>
	public interface IProperties
	{
		/// <summary>
		/// Test the existence of the property with provided name
		/// </summary>
		bool Has(string name);

		/// <summary>
		/// Get the value of specified property
		/// </summary>
		Value Get(string name);

		/// <summary>
		/// Test and get value
		/// </summary>
		bool Get(string name, out Value value);

		/// <summary>
		/// Set the value of specified property
		/// </summary>
		bool Set(string name, Value value);

		/// <summary>
		/// Delete the specified property
		/// </summary>
		bool Delete(string name);

		/// <summary>
		/// Reset (clear) the properties
		/// </summary>
		void Reset();
	}

	/// <summary>
	/// Default property collection implementation
	/// (case insensitive)
	/// </summary>
	public class Properties : Dictionary<string, Value>, IProperties
	{
		/// <summary>
		/// Test the existence of the property with provided name
		/// </summary>
		public bool Has(string name)
			=> ContainsKey(name);

		/// <summary>
		/// Get the value of specified property
		/// </summary>
		public Value Get(string name)
		{
			TryGetValue(name, out var value);
			return value;
		}

		/// <summary>
		/// Test and get value
		/// </summary>
		public bool Get(string name, out Value value)
			=> TryGetValue(name, out value);

		/// <summary>
		/// Set the value of specified property
		/// </summary>
		public bool Set(string name, Value value)
		{
			this[name] = value;
			return true;
		}

		/// <summary>
		/// Set the specified property to IProperty implementation
		/// </summary>
		public bool Set(string name, IProperty prop)
		{
			this[name] = new Value(prop);
			return true;
		}

		/// <summary>
		/// Modify the value of specified property (compound assignment)
		/// </summary>
		public bool Modify(string name, OpCode op, Value value)
		{
			if (!TryGetValue(name, out var left))
				return false;
			left.Modify(op, value);
			this[name] = left;
			return true;
		}

		/// <summary>
		/// Delete the specified property
		/// </summary>
		public bool Delete(string name)
			=> Remove(name);

		/// <summary>
		/// Reset (clear) the properties
		/// </summary>
		public void Reset()
			=> Clear();

		/// <summary>
		/// Create empty
		/// </summary>
		public Properties()
			: base(StringComparer.OrdinalIgnoreCase)
		{ }

		/// <summary>
		/// Create with one object-reference property (usually "prototype")
		/// </summary>
		public Properties(string name, IObject obj)
			: base(StringComparer.OrdinalIgnoreCase)
			=> Set(name, new Value(obj));

		/// <summary>
		/// Add object with name (for inline initialization)
		/// </summary>
		public void Add(string name, IObject obj)
			=> Add(name, Value.FromObject(obj));
	}

	public interface IObject : IProperties
	{
		/// <summary>
		/// Engine this object belongs to
		/// </summary>
		IEngine Engine { get; }
		/// <summary>
		/// Feature flags
		/// </summary>
		ObjectFeatures Features { get; }

		/// <summary>
		/// Name of the object (or full name of the type)
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Base class (to search properties in this object next)
		/// </summary>
		IObject BaseClass { get; }
		/// <summary>
		/// Basic properties - not enumerable, not writable unless IProperty with set returning true
		/// </summary>
		IProperties BaseProps { get; }
		/// <summary>
		/// Added properties - enumerable and writable (unless same exist in baseProps)
		/// </summary>
		IProperties MoreProps { get; }
		/// <summary>
		/// Find the object containing the property
		/// </summary>
		/// <remarks>
		/// This is actually used only by Engine.Context (to handle OpCode.Identifier).
		/// It may be removed in the future - we only need Has and even that is questionable.
		/// </remarks>
		IObject Which(string name);
		/// <summary>
		/// Modify the value of specified property (compound assignment)
		/// </summary>
		bool Modify(string name, OpCode op, Value value);

		/// <summary>
		/// Contained value (if any)
		/// </summary>
		Value Value { get; }
		/// <summary>
		/// Referenced type (if any). Features.TypeReference
		/// </summary>
		Type Type { get; }
		/// <summary>
		/// Referenced native object (if any). Features.Proxy
		/// </summary>
		object Target { get; }

		/// <summary>
		/// Execute regular function call. Features.Function
		/// </summary>
		/// <param name="self">The object to call it on (as method if not null)</param>
		/// <param name="argc">number of arguments (pass to Arg method)</param>
		/// <returns>The result of the function</returns>
		Value Call(IObject self, int argc);

		/// <summary>
		/// Execute constructor ('new' used). Features.Constructor
		/// </summary>
		/// <param name="argc">Number of arguments (pass to Arg method)</param>
		/// <returns>The new object (or null if not supported)</returns>
		IObject Create(int argc);

		/// <summary>
		/// Get property/value (reference) at the indexes. Features.Collection
		/// </summary>
		/// <remarks>
		/// Default implementation treats x[y, z] as x[y][z],
		/// but redirecting to Call may be valid as well
		/// </remarks>
		Value Index(IObject self, int argc);

		/// <summary>
		/// Convert native object into script object Features.Converter
		/// </summary>
		IObject Convert(object value);
	}

	public interface IEngineRoot : IObject
	{
		/// <summary>
		/// Box value (StringObj, NumberObj, ...)
		/// </summary>
		IObject Box(Value value);

		/// <summary>
		/// Create new function
		/// </summary>
		IObject Create(CompiledCode code, int codeAt, int codeSize, int typeAt, ArgumentInfo[] args, string body = null, IObject scope = null);

		/// <summary>
		/// Get type reference (StringFun, NumberFun, ...)
		/// </summary>
		IObject GetType(OpCode OpCode);

		/// <summary>
		/// Get type reference with parameter (array or generic)
		/// </summary>
		IObject GetType(OpCode OpCode, Value value);

		/// <summary>
		/// Get type reference with parameter (array or generic)
		/// </summary>
		IObject GetType(OpCode OpCode, params Value[] par);

		/// <summary>
		/// Get or set type creator (ReflectedType)
		/// </summary>
		IObject this[Type type] { get; set; }
	}

	/// <summary>
	/// Compiled code (at least string table and byte code)
	/// with possibly other references
	/// </summary>
	public class CompiledCode
	{
		public CompiledCode(string[] strings, byte[] code)
		{
			Strings = strings;
			Code = code;
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
		/// <summary>
		/// Line of source content (with position and text)
		/// </summary>
		public struct SourceLine
		{
			public int Position;
			public string Text;
			public SourceLine(int position, string text)
			{
				Position = position;
				Text = text;
			}
		}
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
	public class ArgumentList : List<Value>
	{
		public int Length => Count;
		public void Remove(int last)
			=> RemoveRange(Count - last, last);

		public Value Get(int argc, int index = 0)
		{
			var idx = Count - argc + index;
			return idx < Count ? this[idx] : new Value();
		}
	}

	/// <summary>
	/// Stack of blocks of current function/method
	/// </summary>
	public struct EngineContext : IProperties
	{
		/// <summary>
		/// Current object accessible by 'this' keyword
		/// </summary>
		public IObject Self { get; }
		/// <summary>
		/// Variables of current block (previous block/scope is in baseClass)
		/// </summary>
		public IObject Vars { get; private set; }
		/// <summary>
		/// Root (activation) object (new variables not declared with var will be created here)
		/// </summary>
		public IObject Root { get; }

		/// <summary>
		/// Root context
		/// </summary>
		public EngineContext(IEngine engine)
		{
			Vars = Root = engine.Root;
			Self = engine.HasOption(EngineOption.Strict) ? null : Root;
		}

		/// <summary>
		/// Function execution context
		/// </summary>
		public EngineContext(IEngine engine, IObject self, IObject scope)
		{
			Self = self ?? (engine.HasOption(EngineOption.Strict) ? null : engine.Root);
			Root = Vars = engine.CreateVars(engine.CreateVars(scope ?? engine.Root));
			Vars.Set("arguments", new Value(Vars.BaseClass));
		}

		public void Push(IEngine engine)
			=> Vars = engine.CreateVars(Vars);
		public void Pop()
			=> Vars = Vars.BaseClass;
		public bool Has(string name)
			=> Vars.Has(name);
		public IObject Which(string name)
			=> Vars.Which(name);
		public Value Get(string name)
			=> Vars.Get(name);
		public bool Get(string name, out Value value)
			=> Vars.Get(name, out value);
		public bool Set(string name, Value value)
			=> Vars.Set(name, value);
		public bool Delete(string name)
			=> Vars.Delete(name);
		public void Reset()
			=> Vars.Reset();
	}

	public static class EngineExtensions
	{
		public static bool HasOption(this IEngine engine, EngineOption option)
			=> (engine.Options & option) != 0;

		/// <summary>
		/// Run script in a string
		/// </summary>
		public static void Execute(this IEngine engine, string source)
			=> engine.Execute(engine.Compile(source));
		/// <summary>
		/// Run compiled script
		/// </summary>
		public static void Execute(this IEngine engine, CompiledCode code)
			=> engine.Execute(code, 0, code.Code.Length);

		/// <summary>
		/// Get argument of function call
		/// </summary>
		public static Value GetArgument(this IEngine engine, int argc, int index = 0)
			=> engine.Arguments.Get(argc, index);

		public static void Log(this IEngine engine, string msg, params object[] args)
			=> engine.Log(string.Format(msg, args));
		[Conditional("DEBUG")]
		public static void DebugLog(this IEngine engine, string msg)
			=> engine.Log(msg);
		[Conditional("DEBUG")]
		public static void DebugLog(this IEngine engine, string msg, params object[] args)
			=> engine.Log(string.Format(msg, args));
	}

	public static class EngineRootExtensions
	{
		public static void AddType(this IEngineRoot root, string name, Type type, IObject creator = null)
		{
			if (creator == null)
				creator = root[type];
			else root[type] = creator;
			root.Set(name, new Value(creator));
			root.DebugLog("{0} = {1}", name, type.FullName);
		}
		public static void AddType(this IEngineRoot root, Type type)
		{
			var creator = root[type];
			root.Set(type.Name, new Value(creator));
			root.DebugLog("{0} = {1}", type.Name, type.FullName);
		}

		public static void Log(this IEngineRoot root, string msg)
			=> root.Engine.Log(msg);
		public static void Log(this IEngineRoot root, string msg, params object[] args)
			=> root.Engine.Log(string.Format(msg, args));
		[Conditional("DEBUG")]
		public static void DebugLog(this IEngineRoot root, string msg)
			=> root.Engine.Log(msg);
		[Conditional("DEBUG")]
		public static void DebugLog(this IEngineRoot root, string msg, params object[] args)
			=> root.Engine.Log(string.Format(msg, args));
	}

	public static class ObjectExtensions
	{
		public static bool HasFeature(this IObject obj, ObjectFeatures feature)
			=> (obj.Features & feature) != 0;
	}
}
