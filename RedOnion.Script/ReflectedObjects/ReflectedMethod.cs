using System;
using System.Reflection;

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedMethod : BasicObjects.SimpleObject
	{
		/// <summary>
		/// Reflected type this method belongs to (may be null)
		/// </summary>
		public ReflectedType Creator { get; }

		/// <summary>
		/// Class/type this function belongs to (may be null)
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Method name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Discovered methods with same name
		/// </summary>
		protected MethodInfo[] Methods { get; }

		public ReflectedMethod(
			Engine engine, ReflectedType creator,
			string name, MethodInfo[] methods)
			: base(engine, null)
		{
			Creator = creator;
			Type = creator?.Type;
			Name = name;
			Methods = methods;
		}

		public override Value Call(IObject self, int argc)
		{
			var result = new Value();
			if (self is IObjectProxy proxy)
			{
				var target = proxy.Target;
				if (target == null)
					goto finish;
				if (Type != null && !Type.IsAssignableFrom(target.GetType()))
					goto finish;
				foreach (MethodInfo method in Methods)
					if (TryCall(Engine, method, target, argc, ref result))
						return result;
			}
		finish:
			return result;
		}

		protected static bool TryCall(
			Engine engine, MethodInfo method,
			object self, int argc, ref Value result)
			=> ReflectedFunction.TryCall(
				engine, method, self, argc, ref result);
	}
}
