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
		public override Type Type => _type;
		private Type _type;

		public override ObjectFeatures Features
			=> ObjectFeatures.Function;

		/// <summary>
		/// Method name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Discovered methods with same name
		/// </summary>
		protected internal MethodInfo[] Methods { get; }

		public ReflectedMethod(
			Engine engine, ReflectedType creator,
			string name, params MethodInfo[] methods)
			: base(engine, null)
		{
			Creator = creator;
			_type = creator?.Type;
			Name = name;
			Methods = methods;
		}

		public override Value Call(IObject self, int argc)
		{
			var result = new Value();
			if (self == null || (self.Features & ObjectFeatures.Proxy) == 0)
			{
				if (!Engine.HasOption(Engine.Option.Silent))
					throw new InvalidOperationException("Called "
						+ (Type == null ? Name : Type.Name + "." + Name)
						+ " without native self");
				return result;
			}
			var target = self.Target;
			if (target == null || (Type != null && !Type.IsAssignableFrom(target.GetType())))
			{
				if (!Engine.HasOption(Engine.Option.Silent))
					throw new InvalidOperationException("Called "
						+ (Type == null ? Name : Type.Name + "." + Name)
						+ " without proper self");
				return result;
			}
			foreach (MethodInfo method in Methods)
				if (TryCall(Engine, method, target, argc, ref result))
					return result;
			if (!Engine.HasOption(Engine.Option.Silent))
				throw new InvalidOperationException("Could not call "
					+ (Type == null ? Name : Type.Name + "." + Name)
					+ ", " + Methods.Length + " candidates");
			return result;
		}

		protected static bool TryCall(
			Engine engine, MethodInfo method,
			object self, int argc, ref Value result)
			=> ReflectedFunction.TryCall(
				engine, method, self, argc, ref result);
	}
}
