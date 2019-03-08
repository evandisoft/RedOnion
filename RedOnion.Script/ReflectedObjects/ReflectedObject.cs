using System;
using System.Reflection;

namespace RedOnion.Script.ReflectedObjects
{
	public interface IObjectProxy
	{
		object Target { get; }
	}
	public class ReflectedObject : BasicObjects.SimpleObject, IObjectProxy
	{
		public object Target { get; }
		public ReflectedType Creator { get; }

		public ReflectedObject(Engine engine, object target, IProperties properties = null)
			: base(engine, properties)
			=> Target = target;
		public ReflectedObject(Engine engine, object target, ReflectedType type, IProperties properties = null)
			: this(engine, target, properties)
		{
			Creator = type;
			BaseProps = type.TypeProps;
		}
	}
}
