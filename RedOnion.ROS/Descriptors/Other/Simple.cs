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
				if (self.obj == this)
				{
					if (sprops == null || at < 0 || at >= sprops.Length)
						return false;
					self = sprops[at].value;
					return true;
				}
				if (at < 0 || at >= props.Length)
					return false;
				self = props[at].value;
				return true;
			}
		}
	}
}
