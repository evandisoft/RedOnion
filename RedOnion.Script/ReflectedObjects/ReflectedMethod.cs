using System;
using System.Reflection;

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedMethod : BasicObjects.SimpleObject
	{
		/// <summary>
		/// Method name
		/// </summary>
		public string Name { get; }

		public ReflectedMethod(Engine engine, string name)
			: base(engine, null)
			=> Name = name;
	}
}
