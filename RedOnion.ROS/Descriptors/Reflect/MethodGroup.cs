using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public class MethodGroup : Callable
		{
			internal ListCore<Value> list;

			public MethodGroup(string name, Type type, bool method)
				: base(name, type, method)
			{ }
			public MethodGroup(string name, Type type, bool method, IEnumerable<Value> methods)
				: base(name, type, method)
				=> list.AddRange(methods);

			int argc = int.MaxValue;
			public override bool Call(ref Value result, object self, Arguments args, bool create = false)
			{
				// this is designed for Math.Abs/Max etc. to match the type
				if (argc >= 0)
				{
					if (argc == int.MaxValue)
					{
						foreach (var call in list)
						{
							var desc = call.desc as Callable;
							if (desc == null || desc.Params == null
								|| desc.MinArgs != desc.MaxArgs
								|| (argc != int.MaxValue && argc != desc.MaxArgs))
							{
								argc = -1;
								goto standard;
							}
							if (argc == int.MaxValue)
								argc = desc.MaxArgs;
						}
					}
					if (args.Length > 0 && args[0].IsNumber)
					{
						var type = args[0].desc.Type;
						foreach (var m in list)
						{
							var desc = (Callable)m.desc;
							if (desc.Params[0].ParameterType == type)
							{
								var it = m;
								if (it.desc.Call(ref it, self, args, create))
								{
									result = it;
									return true;
								}
							}
						}
					}
				}
			standard:
				foreach (var call in list)
				{
					var it = call;
					if (it.desc.Call(ref it, self, args, create))
					{
						result = it;
						return true;
					}
				}
				return false;
			}
		}
	}
}
