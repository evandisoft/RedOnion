using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	[Flags]
	public enum EngineOption
	{
		None = 0,
		/// <summary>
		/// Make errors/exceptions produce undefined value where possible
		/// (throw exception otherwise).
		/// </summary>
		Silent = 1 << 0,
		/// <summary>
		/// Strict mode sets `this` in functions (not called as method) to undefined (global otherwise),
		/// prevents automatic boxing of undefined value (creates new object otherwise)
		/// and forbids function overwrite.
		/// </summary>
		Strict = 1 << 1,
		/// <summary>
		/// Variables live inside blocks
		/// (otherwise inside script or function).
		/// </summary>
		BlockScope = 1 << 2,
		/// <summary>
		/// Import properties of this-object into scope of methods.
		/// </summary>
		SelfScope = 1 << 3,
		/// <summary>
		/// Simple expression-statements (identifier or ending with dot+identifier)
		/// must result in function (which is then called with no arguments)
		/// or produce an error.
		/// </summary>
		Autocall = 1 << 4,
		/// <summary>
		/// Weaker version of Autocall
		/// (Will invoke the function but won't produce error if not a function)
		/// </summary>
		WeakAutocall = 1 << 5,

		/// <summary>
		/// Special handling for REPL
		/// </summary>
		/// <remarks>
		/// - Autocall works as WeakAutocall for single statement.
		/// - Strict allows function overwrite.
		/// </remarks>
		Repl = 1 << 31,
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
		IScope CreateVars(IObject baseClass);
		/// <summary>
		/// Create new execution/activation context (for function call)
		/// </summary>
		IObject CreateContext(IObject self, IScope scope, int argc);
		/// <summary>
		/// Destroy last execution/activation context
		/// </summary>
		Value DestroyContext();

		/// <summary>
		/// Log message
		/// </summary>
		void Log(string msg);
		/// <summary>
		/// Callback for print function
		/// </summary>
		event Action<string> Printing;
		/// <summary>
		/// Basic implementation for print function
		/// </summary>
		void Print(string msg);
		/// <summary>
		/// Print with arguments (formatting)
		/// </summary>
		void Print(string msg, params object[] args);
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
		Proxy = 1 << 6,
		/// <summary>
		/// Supports operator overloading
		/// </summary>
		Operators = 1 << 7,
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
		public Properties Set(string name, IProperty prop)
		{
			this[name] = new Value(prop);
			return this;
		}
		/// <summary>
		/// Set the specified property to ValueKind.EasyProp implementation
		/// </summary>
		public Properties Set(string name, PropertyGetter getter, PropertySetter setter = null)
		{
			this[name] = new Value(getter, setter);
			return this;
		}
		/// <summary>
		/// Set the specified property to ValueKind.EasyProp implementation
		/// </summary>
		public Properties Set<Obj>(string name,
			PropertyGetter<Obj> getter, PropertySetter<Obj> setter = null) where Obj : IObject
		{
			this[name] = Value.Property<Obj>(getter, setter);
			return this;
		}
		/// <summary>
		/// Set the specified property to ValueKind.EasyProp implementation
		/// </summary>
		public Properties Set(string name, PropertySetter setter)
		{
			this[name] = new Value((PropertyGetter)null, setter);
			return this;
		}
		/// <summary>
		/// Set the specified property to ValueKind.EasyProp implementation
		/// </summary>
		public Properties Set<Obj>(string name, PropertySetter<Obj> setter) where Obj : IObject
		{
			this[name] = Value.Property<Obj>(null, setter);
			return this;
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
		/// Create by cloning dictionary
		/// </summary>
		public Properties(IDictionary<string, Value> src)
			: base(src, StringComparer.OrdinalIgnoreCase)
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

	public interface IEnumerableObject : IObject, IEnumerable<Value> { }
	public interface IListObject : IEnumerableObject, IList<Value>
	{
		bool IsWritable { get; }
		bool IsFixedSize { get; }
	}
	public interface IObject : IProperties
	{
		/// <summary>
		/// Engine this object belongs to.
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
		/// Base class (to search properties in this object next).
		/// Used to implement inheritance (accessible as `prototype` in BasicObject).
		/// </summary>
		IObject BaseClass { get; }
		/// <summary>
		/// Basic properties - not enumerable, not writable unless IProperty with set returning true.
		/// These are usually shared by all instances of same type.
		/// </summary>
		IProperties BaseProps { get; }
		/// <summary>
		/// Added properties - enumerable and writable (unless same exist in baseProps).
		/// These are specific to the instance (and do not exist at all in SimpleObject).
		/// </summary>
		IProperties MoreProps { get; }
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
		/// Get value (RValue) at index. This can only ever be called if Index created ValueKind.IndexRef.
		/// </summary>
		/// <param name="index">Value used for ValueKind.IndexRef creation</param>
		/// <returns>The value at provided index</returns>
		Value IndexGet(Value index);
		/// <summary>
		/// Set value at index. This can only ever be called if Index created ValueKind.IndexRef.
		/// </summary>
		/// <param name="index">Value used for ValueKind.IndexRef creation</param>
		/// <param name="value">The value to set at provided index</param>
		/// <returns>True on success</returns>
		bool IndexSet(Value index, Value value);
		/// <summary>
		/// Modify the value at specified index (compound assignment)
		/// </summary>
		/// <param name="index">Value used for ValueKind.IndexRef creation</param>
		/// <param name="op">The operation (compound assignment) to perform</param>
		/// <param name="value">The value to use to modify the value at provided index</param>
		/// <returns>True on success</returns>
		bool IndexModify(Value index, OpCode op, Value value);

		/// <summary>
		/// Convert native object into script object Features.Converter
		/// </summary>
		IObject Convert(object value);
		/// <summary>
		/// Calculate result of operator. Features.Operators
		/// </summary>
		/// <param name="op">The operator</param>
		/// <param name="rhs">Right side for binary operators (undefined for unary)</param>
		/// <returns></returns>
		bool Operator(OpCode op, Value arg, bool selfRhs, out Value result);
	}

	public interface IScope : IObject
	{
		/// <summary>
		/// Add / overwrite specified property
		/// (goes to MoreProps ignoring BaseProps and BaseClass)
		/// </summary>
		/// <remarks>
		/// Used by engine to handle `var`.
		/// </remarks>
		void Add(string name, Value value);
		/// <summary>
		/// Find the object containing the property
		/// </summary>
		IObject Which(string name);
	}

	public interface IEngineRoot : IScope
	{
		/// <summary>
		/// Box value (StringObj, NumberObj, ...)
		/// </summary>
		IObject Box(Value value);

		/// <summary>
		/// Create new function
		/// </summary>
		IObject Create(string name,
			CompiledCode code, int codeAt, int codeSize, int typeAt,
			ArgumentInfo[] args, string body = null, IScope scope = null);

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
		public CompiledCode(string[] strings, byte[] code, int[] lineMap)
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

		public struct AddGuard : IDisposable
		{
			ArgumentList arglist;
			int startSize;
			public AddGuard(ArgumentList arglist)
				=> startSize = (this.arglist = arglist).Count;
			public void Dispose()
				=> arglist.RemoveRange(startSize, arglist.Count - startSize);
		}
		public AddGuard Guard()
			=> new AddGuard(this);
	}

	/// <summary>
	/// Stack of blocks of current function/method
	/// </summary>
	public struct EngineContext
	{
		/// <summary>
		/// Variables of current block (previous block/scope is in baseClass)
		/// </summary>
		public IScope Vars { get; private set; }
		/// <summary>
		/// Root (activation) object (new variables not declared with var will be created here)
		/// </summary>
		public IScope Root { get; }
		/// <summary>
		/// Current object accessible by 'this' keyword
		/// </summary>
		public IObject Self { get; }

		/// <summary>
		/// Root context
		/// </summary>
		public EngineContext(IEngine engine)
		{
			Root = Vars = engine.Root;
			Self = null;
		}

		/// <summary>
		/// Function execution context
		/// </summary>
		public EngineContext(IEngine engine, IObject self, IObject scope, int argc)
		{
			// Vars -> ... -> Root -> Arguments -> Scope
			// -> Self -> Self.BaseClass -> ... (EngineOption.SelfScope)
			// -> Globals (engine.Root)
			var args = engine.CreateVars(scope);
			Root = Vars = engine.CreateVars(args);
			Self = self;
			Vars.Add("arguments", new Value(args));
			args.Add("length", argc);
		}

		public void Push(IEngine engine)
			=> Vars = engine.CreateVars(Vars);
		public void Pop()
			=> Vars = (IScope)Vars.BaseClass;
		public bool Has(string name)
			=> Vars.Has(name)
			|| (Vars.Engine.HasOption(EngineOption.SelfScope) && Self?.Has(name) == true)
			|| Vars.Engine.Root.Has(name);
		public IObject Which(string name)
			=> Vars.Which(name)
			?? (Vars.Engine.HasOption(EngineOption.SelfScope) && Self?.Has(name) == true ? Self : null)
			?? Vars.Engine.Root.Which(name);
		public Value Get(string name)
			=> Get(name, out var value) ? value : new Value();
		public bool Get(string name, out Value value)
			=> Vars.Get(name, out value)
			|| (Vars.Engine.HasOption(EngineOption.SelfScope) && Self?.Get(name, out value) == true)
			|| Vars.Engine.Root.Get(name, out value);
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
			=> engine.Log(string.Format(Value.Culture, msg, args));
		[Conditional("DEBUG")]
		public static void DebugLog(this IEngine engine, string msg)
			=> engine.Log(msg);
		[Conditional("DEBUG")]
		public static void DebugLog(this IEngine engine, string msg, params object[] args)
			=> engine.Log(string.Format(Value.Culture, msg, args));
	}

	public static class EngineRootExtensions
	{
		public static IObject AddType(this IEngineRoot root, string name, Type type, IObject creator = null)
		{
			if (creator == null)
				creator = root[type];
			else root[type] = creator;
			root.Set(name, new Value(creator));
			root.DebugLog("{0} = {1}", name, type.FullName);
			return creator;
		}
		public static IObject AddType(this IEngineRoot root, Type type)
		{
			var creator = root[type];
			root.Set(type.Name, new Value(creator));
			root.DebugLog("{0} = {1}", type.Name, type.FullName);
			return creator;
		}

		public static void Log(this IEngineRoot root, string msg)
			=> root.Engine.Log(msg);
		public static void Log(this IEngineRoot root, string msg, params object[] args)
			=> root.Engine.Log(string.Format(Value.Culture, msg, args));
		[Conditional("DEBUG")]
		public static void DebugLog(this IEngineRoot root, string msg)
			=> root.Engine.Log(msg);
		[Conditional("DEBUG")]
		public static void DebugLog(this IEngineRoot root, string msg, params object[] args)
			=> root.Engine.Log(string.Format(Value.Culture, msg, args));
	}

	public static class ObjectExtensions
	{
		public static bool HasFeature(this IObject obj, ObjectFeatures feature)
			=> (obj.Features & feature) != 0;
	}

	public static class PropertiesExcentions
	{
		public static bool Set(this IProperties props, string name, IObject value)
			=> props.Set(name, new Value(value));
	}
}
