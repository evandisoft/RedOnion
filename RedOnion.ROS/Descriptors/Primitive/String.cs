using System;
using System.Collections.Generic;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		internal class OfString : Simple
		{
			public OfString()
				: base("string", typeof(string), ExCode.String, TypeCode.String, props) { }
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.obj.ToString();

			public override bool Call(ref Value result, object self, Arguments args, bool create = false)
			{
				if (args.Length != 1 || (result.obj != null && result.obj != this))
					return false;
				result = args[0].ToStr();
				return true;
			}

			public override bool Convert(ref Value self, Descriptor to)
			{
				var str = self.obj.ToString();
				switch (to.Primitive)
				{
				case ExCode.String:
					self = str;
					return true;
				case ExCode.Char:
				case ExCode.WideChar:
					self = str.Length == 0 ? '\0' : str[0];
					return true;
				case ExCode.Byte:
					self = byte.Parse(str, Value.Culture);
					return true;
				case ExCode.UShort:
					self = ushort.Parse(str, Value.Culture);
					return true;
				case ExCode.UInt:
					self = uint.Parse(str, Value.Culture);
					return true;
				case ExCode.ULong:
					self = ulong.Parse(str, Value.Culture);
					return true;
				case ExCode.SByte:
					self = sbyte.Parse(str, Value.Culture);
					return true;
				case ExCode.Short:
					self = short.Parse(str, Value.Culture);
					return true;
				case ExCode.Int:
					self = int.Parse(str, Value.Culture);
					return true;
				case ExCode.Long:
					self = long.Parse(str, Value.Culture);
					return true;
				case ExCode.Float:
					self = float.Parse(str, Value.Culture);
					return true;
				case ExCode.Number:
				case ExCode.Double:
					self = double.Parse(str, Value.Culture);
					return true;
				case ExCode.Bool:
					self = bool.Parse(str);
					return true;
				}
				return false;
			}

			public override bool Binary(ref Value lhs, OpCode op, ref Value rhs)
			{
				if (lhs.desc.Primitive != ExCode.String && !lhs.desc.Convert(ref lhs, this))
					return false;
				if (rhs.desc.Primitive != ExCode.String && !rhs.desc.Convert(ref rhs, this))
					return false;
				switch (op)
				{
				case OpCode.Add:
					lhs.obj = lhs.obj.ToString() + rhs.obj.ToString();
					return true;

				case OpCode.Equals:
					lhs = string.Compare(lhs.obj.ToString(), rhs.obj.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
					return true;
				case OpCode.Differ:
					lhs = string.Compare(lhs.obj.ToString(), rhs.obj.ToString(), StringComparison.OrdinalIgnoreCase) != 0;
					return true;

				case OpCode.Less:
					lhs = string.Compare(lhs.obj.ToString(), rhs.obj.ToString(), StringComparison.OrdinalIgnoreCase) < 0;
					return true;
				case OpCode.More:
					lhs = string.Compare(lhs.obj.ToString(), rhs.obj.ToString(), StringComparison.OrdinalIgnoreCase) > 0;
					return true;
				case OpCode.LessEq:
					lhs = string.Compare(lhs.obj.ToString(), rhs.obj.ToString(), StringComparison.OrdinalIgnoreCase) <= 0;
					return true;
				case OpCode.MoreEq:
					lhs = string.Compare(lhs.obj.ToString(), rhs.obj.ToString(), StringComparison.OrdinalIgnoreCase) >= 0;
					return true;
				}
				return true;
			}

			static readonly Props props = new StringProps();
			class StringProps : Props
			{
				static Prop M(string name, Func<string, Value, Value> fn)
					=> new Prop(name, new Value(new Method1<string>(name), fn));
				public StringProps() : base(new Prop[]
				{
					new Prop("length", new Value(0)),
					new Prop("substring", new Value(new Substring())),
					M("compare", (it, arg) => it.CompareTo(arg.ToStr())),
					M("equals", (it, arg) => it.Equals(arg.ToStr())),
					M("contains", (it, arg) => it.Contains(arg.ToStr()))
				}, new Prop[] {
					new Prop("format", new Value(new Format()))
				})
				{
					Alias("count", 0);
					Alias("substr", 1);
					Alias("compareTo", 2);
				}
			};

			public override bool Get(ref Value self, int at)
			{
				if (self.obj != this)
				{
					if (at == 0)
					{
						self = self.obj.ToString().Length;
						return true;
					}
					if (at >= 0x100)
					{
						self = self.obj.ToString()[at-0x100];
						return true;
					}
				}
				return base.Get(ref self, at);
			}

			public override int IndexFind(ref Value self, Arguments args)
			{
				if (args.Length == 0)
					return -1;
				var index = args[0];
				int at;
				if (index.IsNumberOrChar)
				{
					at = index.ToInt();
					if (at < 0 || at >= self.obj.ToString().Length)
						throw new IndexOutOfRangeException();
					return at + 0x100;
				}
				if (!index.desc.Convert(ref index, String))
					return -1;
				var name = index.obj.ToString();
				at = Find(self.obj, name, true);
				if (at < 0 || args.Length == 1)
					return at;
				if (!Get(ref self, at))
					return -1;
				return self.desc.IndexFind(ref self, new Arguments(args, args.Length-1));
			}
			public override IEnumerable<Value> Enumerate(object self)
			{
				foreach (var c in self.ToString())
					yield return new Value(c);
			}

			class Substring : Descriptor
			{
				public override bool Call(ref Value result, object self, Arguments args, bool create = false)
				{
					var str = self.ToString();
					if (args.Length == 0)
					{
						result = str;
						return true;
					}
					var arg = args[0].ToInt();
					if (args.Length == 1)
					{
						result = str.Substring(arg);
						return true;
					}
					var arg2 = args[1].ToInt();
					result = str.Substring(arg, arg2);
					return true;
				}
			}
			class Format : Descriptor
			{
				public override bool Call(ref Value result, object self, Arguments args, bool create = false)
				{
					if (self == Descriptor.String)
					{
						if (args.Length == 0)
							return false;
						var msg = args[0].ToStr();
						if (args.Length == 1)
						{
							result = msg;
							return true;
						}
						var call = new object[args.Length-1];
						for (int i = 1; i < args.Length; i++)
							call[i-1] = args[i].Box();
						msg = string.Format(Value.Culture, msg, call);
						result = msg;
						return true;
					}
					else
					{
						var msg = self.ToString();
						if (args.Length == 0)
						{
							result = msg;
							return true;
						}
						var call = new object[args.Length];
						for (int i = 0; i < args.Length; i++)
							call[i] = args[i].Box();
						msg = string.Format(Value.Culture, msg, call);
						result = msg;
						return true;
					}
				}
			}
		}
	}
}
