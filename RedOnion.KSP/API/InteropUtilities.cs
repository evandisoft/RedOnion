using MoonSharp.Interpreter;
using RedOnion.ROS;
using System;

namespace RedOnion.KSP.API
{
	public static class InteropUtilities
	{
		public static Value ToRos(this DynValue dyn)
		{
			switch (dyn.Type)
			{
			case DataType.Nil:
				return Value.Null;
			case DataType.Void:
				return Value.Void;
			case DataType.Boolean:
				return new Value(dyn.Boolean);
			case DataType.Number:
				return new Value(dyn.Number);
			case DataType.String:
				return new Value(dyn.String);
			case DataType.UserData:
				return new Value(dyn.UserData.Object);
			}
			return new Value();
		}
		public static DynValue ToLua(this Value value, Script script = null)
		{
			if (value.IsNumerOrChar)
				return DynValue.NewNumber(value.ToDouble());
			if (value.IsString)
				return DynValue.NewString(value.ToStr());
			if (value.IsVoid)
				return DynValue.Void;
			return DynValue.FromObject(script, value.obj);
		}
		public static object ToRos(this CallbackArguments from, out Arguments to)
		{
			object self = null;
			if (from.Count > 0)
			{
				var zero = from[0];
				if (zero.Type == DataType.UserData)
					self = zero.UserData.Object;
			}
			if (from.Count <= 1)
			{
				to = new Arguments(null, 0);
				return self;
			}
			var list = new ArgumentList(null);
			for (int i = 1; i < from.Count; i++)
				list.Add(ToRos(from[i]));
			to = new Arguments(list, list.Count);
			return self;
		}
	}
}
