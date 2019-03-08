using System;
using System.Reflection;

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedObject : BasicObjects.SimpleObject
	{
		public object Target { get; }
		public ReflectedType Type { get; }

		public ReflectedObject(Engine engine, object target, IProperties properties = null)
			: base(engine, properties)
			=> Target = target;
		public ReflectedObject(Engine engine, object target, ReflectedType type, IProperties properties = null)
			: this(engine, target, properties)
			=> Type = type;
	}
}
