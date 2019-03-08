using System;
using System.Reflection;

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedFunction : BasicObjects.SimpleObject
	{
		/// <summary>
		/// Class this function belongs to
		/// </summary>
		public Type Class { get; }

		/// <summary>
		/// Function name (static method)
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Reflected type this function belongs to (may be null)
		/// </summary>
		public ReflectedType Type { get; }

		public ReflectedFunction(Engine engine, Type type, string name)
			: base(engine, null)
		{
			Class = type;
			Name = name;
		}

		public ReflectedFunction(Engine engine, ReflectedType type, string name)
			: base(engine, null)
		{
			Class = type.Class;
			Name = name;
			Type = type;
		}
	}
}
