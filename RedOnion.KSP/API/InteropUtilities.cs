using MoonSharp.Interpreter;
using RedOnion.Script;
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
				var obj = dyn.UserData.Object;
				if (obj is IObject ros)
					return new Value(ros);
				return Value.AsNative(obj);
			}
			return new Value();
		}
		public static DynValue ToLua(this Value value)
		{
			if (value.IsNumber)
				return DynValue.NewNumber(value.Number);
			switch (value.Kind)
			{
			case ValueKind.Object:
				var obj = value.RefObj;
				return obj == null ? DynValue.Nil : UserData.Create(obj);
			case ValueKind.Native:
				return DynValue.FromObject(null, value.Native);
			case ValueKind.Void:
				return DynValue.Void;
			case ValueKind.String:
				return DynValue.NewString(value.String);
			}
			return DynValue.Nil;
		}
		public static IObject ToRos(this CallbackArguments from, out Arguments to)
		{
			IObject self = null;
			if (from.Count > 0)
			{
				var zero = from[0];
				if (zero.Type == DataType.UserData)
					self = zero.UserData.Object as IObject;
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
