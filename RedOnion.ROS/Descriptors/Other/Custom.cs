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
			public delegate void Read(ref Value self);
			public delegate void Write(ref Value self, ref Value value);
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
					read = (ref Value self) => self = value;
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
			public override bool Has(Core core, ref Value self, string name)
			{
				var index = this.index;
				if (self.obj == this)
					index = sindex;
				return index != null && index.ContainsKey(name);
			}
			public override void Get(Core core, ref Value self)
			{
				if (!(self.idx is string name))
					goto fail;
				var props = this.props;
				var index = this.index;
				if (self.obj == this)
				{
					props = sprops;
					index = sindex;
				}
				int at = index != null && index.TryGetValue(name, out var found) ? found : -1;
				if (at < 0)
					goto fail;
				var read = sprops[at].read;
				if (read != null)
				{
					read(ref self);
					return;
				}
			fail:
				GetError(ref self);
			}
			public override void Set(Core core, ref Value self, OpCode op, ref Value value)
			{
				if (!(self.idx is string name))
					goto fail;
				var props = this.props;
				var index = this.index;
				if (self.obj == this)
				{
					props = sprops;
					index = sindex;
				}
				int at = index != null && index.TryGetValue(name, out var found) ? found : -1;
				if (at < 0)
					goto fail;
				ref var prop = ref props[at];
				var write = prop.write;
				if (write == null)
					goto fail;
				if (op == OpCode.Assign)
				{
					write(ref self, ref value);
					return;
				}
				var read = prop.read;
				if (read == null)
					goto fail;
				var it = self;
				read(ref it);
				if (op.Kind() == OpKind.Assign)
				{
					if (!it.desc.Binary(ref it, op + 0x10, ref value)
						&& !value.desc.Binary(ref it, op + 0x10, ref value))
						goto fail;
					write(ref self, ref it);
					return;
				}
				if (op.Kind() != OpKind.PreOrPost)
					goto fail;
				if (op >= OpCode.Inc)
				{
					it.desc.Unary(ref it, op);
					write(ref self, ref it);
					return;
				}
				var tmp = it;
				it.desc.Unary(ref it, op + 0x08);
				write(ref self, ref it);
				self = tmp;
				return;
			fail:
				GetError(ref self);
			}
		}
	}
}
