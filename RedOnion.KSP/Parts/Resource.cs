//#define DEBUG_STAGE_NOFUEL

using RedOnion.KSP.API;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.Parts
{
	[Description("List of resources (read-only)."
		+ " Can either belong to single part or to list/set of parts.")]
	public class ResourceList : ReadOnlyList<Resource>
	{
		protected internal PartBase part;
		protected internal IPartSet partSet;
		protected Dictionary<ResourceID, Resource> cache = new Dictionary<ResourceID, Resource>();
		protected Dictionary<ResourceID, Resource> prevCache = new Dictionary<ResourceID, Resource>();
		protected internal double stamp;

		protected internal ResourceList(PartBase part)
			=> this.part = part;
		protected internal ResourceList(IPartSet partSet)
			=> this.partSet = partSet;

		protected override void Update()
		{
			partSet?.Update();
			base.Update();
		}

		protected internal override void Clear()
		{
			var prev = prevCache;
			prevCache = cache;
			cache = prev;
			cache.Clear();
			list.Clear();
		}

		protected override void DoRefresh()
		{
			Update(Time.now);
			base.DoRefresh();
#if DEBUG && DEBUG_STAGE_NOFUEL
			if (partSet == Stage.xparts)
			{
				Value.DebugLog($"Stage.xparts.resources refreshed: {list.Count}/{Stage.xparts.count}");
				foreach (var res in list)
					Value.DebugLog($"{res.name}: {res.amount}/{res.maxAmount}#{res.partCount}");
			}
#endif
		}
		protected internal void Update(double now)
		{
			stamp = now;
			foreach (var res in list)
			{
				res.partCount = 0;
				res._amount = 0;
				res._maxAmount = 0;
			}
			if (part != null)
				Process(part);
			else
			{
				foreach (var part in partSet)
					Process(part);
			}
		}
		protected void Process(PartBase part)
		{
			foreach (var native in part.native.Resources)
			{
				var name = native.resourceName;
				var id = new ResourceID(name);
				if (!cache.TryGetValue(id, out var res))
				{
					if (!prevCache.TryGetValue(id, out res))
						res = new Resource(this, name, id, this.part == null ? null : native);
					Add(res);
					cache[id] = res;
				}
				res.partCount++;
				res._amount += native.amount;
				res._maxAmount += native.maxAmount;
			}
		}

		[Description("Get resource by name. Returns null for non-existent resource.")]
		public Resource this[string name] => this[new ResourceID(name)];
		[Description("Get resource by ID (hash of the name). Returns null for non-existent resource.")]
		public Resource this[ResourceID id]
		{
			get
			{
				Update();
				return cache.TryGetValue(id, out var it) ? it : null;
			}
		}
		[Description("Get amount of resource (in part or set/list) by name. Returns zero for non-existent resources.")]
		public double getAmountOf(string name)
			=> this[name]?.amount ?? 0.0;
		[Description("Get maximum storable amount of resource by name. Returs zero for non-existent resources.")]
		public double getMaxAmountOf(string name)
			=> this[name]?.maxAmount ?? 0.0;
		[Description("Get number of parts that can store the named resource. Returns zero for non-existent resources.")]
		public int getPartCountOf(string name)
			=> this[name]?.partCount ?? 0;

		[Description("Get amount of resource (in part or set/list) by name. Returns zero for non-existent resources.")]
		public double getAmountOf(ResourceID id)
			=> this[id]?.amount ?? 0.0;
		[Description("Get maximum storable amount of resource by name. Returs zero for non-existent resources.")]
		public double getMaxAmountOf(ResourceID id)
			=> this[id]?.maxAmount ?? 0.0;
		[Description("Get number of parts that can store the named resource. Returns zero for non-existent resources.")]
		public int getPartCountOf(ResourceID id)
			=> this[id]?.partCount ?? 0;

		[Description("Get total amount of resources (in part or set/list) by list of names.")]
		public double getAmountOf(IEnumerable<string> names)
		{
			double sum = 0.0;
			foreach (var name in names)
				sum += getAmountOf(name);
			return sum;
		}
		[Description("Get total amount of resources (in part or set/list) by list of IDs.")]
		public double getAmountOf(IEnumerable<ResourceID> ids)
		{
			double sum = 0.0;
			foreach (var id in ids)
				sum += getAmountOf(id);
			return sum;
		}

		public override string ToString()
		{
			Update();
			bool first = true;
			var sb = new StringBuilder();
			sb.Append("[");
			foreach (var res in list)
			{
				if (!first) sb.Append(", ");
				first = false;
				sb.Append(res.name);
			}
			sb.Append("]");
			return sb.ToString();
		}
	}
	[Description("Resource ID - wrapped hash of resource name.")]
	public readonly struct ResourceID : IEquatable<ResourceID>, IComparable<ResourceID>, IFormattable
	{
		public readonly int id;
		public ResourceID(int id) => this.id = id;
		public ResourceID(string name) => id = name.GetHashCode();
		public static implicit operator int(ResourceID id) => id.id;

		public bool Equals(ResourceID other) => id == other.id;
		public override bool Equals(object obj) => obj is ResourceID other && Equals(other);
		public override int GetHashCode() => id;
		public int CompareTo(ResourceID other) => id.CompareTo(other.id);

		public override string ToString() => id.ToString();
		public string ToString(string format, IFormatProvider formatProvider) => id.ToString(format, formatProvider);
	}
	[Description("Resource like LiquidFuel etc.")]
	public class Resource
	{
		// null for aggregate, assigned if List.part != null
		protected PartResource native;
		[Description("The list this resource is part of.")]
		public ResourceList list { get; }
		[Description("Name of the resource (e.g. LiquidFuel).")]
		public string name { get; }
		[Description("Identificator of the resource (for resource library; hash of the name).")]
		public ResourceID id { get; }

		[Description("Number of parts (in parent list/set) able to contain this resource.")]
		public int partCount
		{
			get;
			protected internal set;
		}

		internal double _amount, _maxAmount;
		[Description("Current amount of the resource.")]
		public double amount
		{
			get
			{
				if (native != null)
					return native.amount;
				double now = Time.now;
				if (now != list.stamp) list.Update(now);
				return _amount;
			}
		}
		[Description("Maximal amount of the resource that can be stored.")]
		public double maxAmount
		{
			get
			{
				if (native != null)
					return native.maxAmount;
				double now = Time.now;
				if (now != list.stamp) list.Update(now);
				return _maxAmount;
			}
		}

		protected internal Resource(ResourceList list, string name, ResourceID id, PartResource native = null)
		{
			this.native = native;
			this.list = list;
			this.name = name;
			this.id = id;
		}

		public override string ToString() => name;
	}
}
