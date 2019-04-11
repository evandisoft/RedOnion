using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public class ReflectedAction : Descriptor
		{
			public ReflectedAction(string name) : base(name, typeof(Action)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create)
					throw InvalidOperation(Name + " cannot be used with 'new'");
				if (args.Length != 0)
					throw InvalidOperation(Name + " does not take arguments");
				((Action)result.obj)();
				result = Value.Void;
				return true;
			}
		}
		public class ReflectedMethod<T> : Descriptor
		{
			public ReflectedMethod(string name) : base(name, typeof(Action<T>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create)
					throw InvalidOperation(Name + " cannot be used with 'new'");
				if (args.Length != 0)
					throw InvalidOperation(Name + " does not take arguments");
				((Action<T>)result.obj)((T)self);
				result = Value.Void;
				return true;
			}
		}
	}
}
