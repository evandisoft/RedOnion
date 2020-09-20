using RedOnion.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public class Simple : Descriptor
		{
			public delegate bool Read(ref Value self);
			public delegate bool Write(ref Value self, ref Value value);
			[DebuggerDisplay("{name}")]
			public readonly struct Prop
			{
				public readonly string name;
				public readonly Value value;
				public override string ToString()
					=> name;
				public Prop(string name, Value value)
				{
					this.name = name;
					this.value = value;
				}
				public static Prop Method<T>(string name, Func<T, Value, Value> fn)
					=> new Prop(name, new Value(new Method1<string>(name), fn));
			}
			public class Props
			{
				internal Prop[] items, sitems;
				internal Dictionary<string, int> index, sindex;
				public Props(Prop[] items, Prop[] sitems = null)
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
			public Simple(string name, Props props)
				: this(name, typeof(Custom), props) { }
			public Simple(string name, Type type, Props props)
				: base(name, type)
			{
				this.props = props.items;
				index = props.index;
				sprops = props.sitems;
				sindex = props.sindex;
			}
			internal Simple(string name, Type type, ExCode primitive, TypeCode typeCode, Props props)
				: base(name, type, primitive, typeCode)
			{
				this.props = props.items;
				index = props.index;
				sprops = props.sitems;
				sindex = props.sindex;
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
				self = props[at].value;
				return;
			fail:
				GetError(ref self);
			}
		}
	}
}
