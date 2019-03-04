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
	delegate IObject CreateObj(Engine engine);

	/// <summary>
	/// Property interface (single property with custom access methods)
	/// </summary>
	/// <remarks>Can only be hosted in read-only properties (not in dynamic)</remarks>
	public interface IProperty
	{
		/// <summary>
		/// Get value of this property
		/// </summary>
		Value Get(IObject iObject);

		/// <summary>
		/// Set value of this property
		/// </summary>
		/// <returns>False if not set (e.g. read-only)</returns>
		bool Set(IObject iObject, Value value);
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
		/// Delete the specified property
		/// </summary>
		public bool Set(string name, IProperty prop)
		{
			this[name] = new Value(prop);
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
		/// Create with one property
		/// </summary>
		public Properties(string name, Value value)
			: base(StringComparer.OrdinalIgnoreCase)
			=> Set(name, value);

		/// <summary>
		/// Create with one object-reference property (usually "prototype")
		/// </summary>
		public Properties(string name, IObject obj)
			: base(StringComparer.OrdinalIgnoreCase)
			=> Set(name, new Value(obj));
	}

	public interface IObject : IProperties
	{
		/// <summary>
		/// Engine this object belongs to
		/// </summary>
		Engine Engine { get; }
		/// <summary>
		/// Base class (to search properties in this object next)
		/// </summary>
		IObject BaseClass { get; }
		/// <summary>
		/// Basic properties - not enumerable, not writable unless iProp with set returning true
		/// </summary>
		IProperties BaseProps { get; }
		/// <summary>
		/// Added properties - enumerable and writable (unless same exist in baseProps)
		/// </summary>
		IProperties MoreProps { get; }
		/// <summary>
		/// Contained value (if any)
		/// </summary>
		Value Value { get; }
		/// <summary>
		/// Find the object containing the property
		/// </summary>
		IObject Which(string name);

		/// <summary>
		/// Execute regular function call
		/// </summary>
		/// <param name="self">The object to call it on (as method if not null)</param>
		/// <param name="argc">number of arguments (pass to Arg method)</param>
		/// <returns>The result of the function</returns>
		Value Call(IObject self, int argc);

		/// <summary>
		/// Execute constructor ('new' used)
		/// </summary>
		/// <param name="argc">Number of arguments (pass to Arg method)</param>
		/// <returns>The new object (or null if not supported)</returns>
		IObject Create(int argc);

		/// <summary>
		/// Get propertu/value (reference) at the indexes
		/// </summary>
		/// <remarks>
		/// Default implementation treats x[y, z] as x[y][z],
		/// but redirecting to Call may be valid as well
		/// </remarks>
		Value Index(IObject self, int argc);
	}
}
