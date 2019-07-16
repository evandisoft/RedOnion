using RedOnion.KSP.API;
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
		protected readonly Dictionary<string, Resource> cache = new Dictionary<string, Resource>();
		protected internal double stamp;

		protected internal ResourceList(PartBase part)
			=> this.part = part;
		protected internal ResourceList(IPartSet partSet)
			=> (this.partSet = partSet).Refresh += SetDirty;

		protected override void DoRefresh()
		{
			// TODO: reuse
			list.Clear();
			cache.Clear();
			Update(Planetarium.GetUniversalTime());
			base.DoRefresh();
		}
		protected internal void Update(double now)
		{
			stamp = now;
			foreach (var res in list)
			{
				res.PartCount = 0;
				res.amount = 0;
				res.maxAmount = 0;
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
			foreach (var native in part.Native.Resources)
			{
				var name = native.resourceName;
				if (!cache.TryGetValue(name, out var res))
				{
					res = new Resource(this, name, part == null ? null : native);
					Add(res);
					cache[name] = res;
				}
				res.amount += native.amount;
				res.maxAmount += native.maxAmount;
			}
		}

		[Description("Get resource by name. Returns null for non-existent resource.")]
		public Resource this[string name]
		{
			get
			{
				if (Dirty) DoRefresh();
				return cache.TryGetValue(name, out var it) ? it : null;
			}
		}
		[Description("Get amount of resource (in part or set/list) by name. Returns zero for non-existent resources.")]
		public double GetAmountOf(string name)
			=> this[name]?.Amount ?? 0.0;
		[Description("Get maximum storable amount of resource by name. Returs zero for non-existent resources.")]
		public double GetMaxAmountOf(string name)
			=> this[name]?.MaxAmount ?? 0.0;
		[Description("Get number of parts that can store the named resource. Returns zero for non-existent resources.")]
		public int GetPartCountOf(string name)
			=> this[name]?.PartCount ?? 0;
	}
	public class Resource
	{
		// null for aggregate, assigned if List.part != null
		protected PartResource native;
		public ResourceList List { get; }
		public string Name { get; }
		public int ID { get; }

		protected int count;
		public int PartCount
		{
			get;
			protected internal set;
		}

		internal double amount, maxAmount;
		public double Amount
		{
			get
			{
				if (native != null)
					return native.amount;
				double now = Planetarium.GetUniversalTime();
				if (now != List.stamp) List.Update(now);
				return amount;
			}
		}
		public double MaxAmount
		{
			get
			{
				if (native != null)
					return native.maxAmount;
				double now = Planetarium.GetUniversalTime();
				if (now != List.stamp) List.Update(now);
				return maxAmount;
			}
		}

		protected internal Resource(ResourceList list, string name, PartResource native = null)
		{
			this.native = native;
			List = list;
			Name = name;
			ID = name.GetHashCode();
			if (native != null)
				PartCount = 1;
		}
	}
}
