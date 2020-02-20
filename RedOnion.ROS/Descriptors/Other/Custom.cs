using RedOnion.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public class Custom : Descriptor
		{
			public delegate bool Read(ref Value self);
			public delegate bool Write(ref Value self, ref Value value);
			[DebuggerDisplay("{name}")]
			public readonly struct Prop
			{
				public readonly string name;
				public readonly Read   read;
				public readonly Write  write;
				public override string ToString()
					=> name;
				public Prop(string name, Read read, Write write = null)
				{
					this.name = name;
					this.read = read;
					this.write = write;
				}
				public Prop(string name, Value value)
				{
					this.name = name;
					read = (ref Value self) => { self = value; return true; };
					write = null;
				}
			}
			public class Props
			{
				internal Prop[] items, sitems;
				internal Dictionary<string, int> index, sindex;
				public Props(Prop[] items) : this(null, items) { }
				public Props(Prop[] items, Prop[] sitems)
				{
					if (sitems != null)
					{
						this.sitems = sitems;
						sindex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
						for (int i = 0; i < sitems.Length; i++)
							sindex[sitems[i].name] = i;
						if (items == null)
						{
							this.items = sitems;
							index = sindex;
							return;
						}
						var mix = new ListCore<Prop>(items.Length + sitems.Length);
						index = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
						for (int i = 0; i < items.Length; i++)
						{
							mix.Add(items[i]);
							index[items[i].name] = i;
						}
						foreach (var item in sitems)
						{
							if (index.ContainsKey(item.name))
								continue;
							index[item.name] = mix.size;
							mix.Add(item);
						}
						this.items = mix.ToArray();
						return;
					}
					if (items  != null)
					{
						this.items = items;
						index = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
						for (int i = 0; i < items.Length; i++)
							index[items[i].name] = i;
					}
				}
				protected void Alias(string alias, int at)
					=> index.Add(alias, at);
				protected void Alias(string alias, string name)
					=> index.Add(alias, index[name]);
			}
			readonly Prop[] props, sprops;
			readonly Dictionary<string, int> index, sindex;
			public Custom(string name, Props props, Props sprops = null)
				: this(name, typeof(Custom), props, sprops) { }
			public Custom(string name, Type type, Props props, Props sprops = null)
				: base(name, type)
			{
				this.props = props?.items;
				index = props?.index;
				this.sprops = sprops?.items;
				sindex = sprops?.index;
			}
			internal Custom(string name, Type type, ExCode primitive, TypeCode typeCode, Props props, Props sprops = null)
				: base(name, type, primitive, typeCode)
			{
				this.props = props?.items;
				index = props?.index;
				this.sprops = sprops?.items;
				sindex = sprops?.index;
			}

			public override int Find(object self, string name, bool add = false)
				=> self == this ? sindex != null && sindex.TryGetValue(name, out var it) ? it : -1
				: index.TryGetValue(name, out it) ? it : -1;
			public override IEnumerable<string> EnumerateProperties(object self)
			{
				if (self == this)
				{
					if (sprops == null)
						yield break;
					foreach (var prop in sprops)
						yield return prop.name;
					yield break;
				}
				foreach (var prop in props)
					yield return prop.name;
			}
			public override bool Get(ref Value self, int at)
			{
				var props = this.props;
				if (self.obj == this)
				{
					if (sprops == null)
						return false;
					props = sprops;
				}
				if (at < 0 || at >= props.Length)
					return false;
				var read = props[at].read;
				if (read == null)
					return false;
				return read(ref self);
			}
			public override bool Set(ref Value self, int at, OpCode op, ref Value value)
			{
				var props = this.props;
				var index = this.index;
				if (self.obj == this)
				{
					if (sprops == null)
						return false;
					props = sprops;
					index = sindex;
				}
				if (at < 0 || at >= props.Length)
					return false;
				ref var prop = ref props[at];
				var write = prop.write;
				if (write == null)
					return false;
				if (op == OpCode.Assign)
					return write(ref self, ref value);
				var read = prop.read;
				if (read == null)
					return false;
				var it = self;
				if (!read(ref it))
					return false;
				if (op.Kind() == OpKind.Assign)
				{
					if (!it.desc.Binary(ref it, op + 0x10, ref value)
						&& !value.desc.Binary(ref it, op + 0x10, ref value))
						return false;
					write(ref self, ref it);
					return true;
				}
				if (op.Kind() != OpKind.PreOrPost)
					return false;
				if (op >= OpCode.Inc)
				{
					if (!it.desc.Unary(ref it, op))
						return false;
					write(ref self, ref it);
					return true;
				}
				var tmp = it;
				if (!it.desc.Unary(ref it, op + 0x08))
					return false;
				write(ref self, ref it);
				self = tmp;
				return true;
			}
		}
	}
}
