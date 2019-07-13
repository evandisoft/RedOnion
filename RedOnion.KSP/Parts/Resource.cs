using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.Parts
{
	public class ResourceList : ReadOnlyList<Resource>
	{
		protected PartBase part;
		protected IPartSet partSet;
		protected readonly Dictionary<string, Resource> cache = new Dictionary<string, Resource>();
		protected internal double stamp;

		protected internal ResourceList(PartBase part) => this.part = part;
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
					res = new Resource(this, name);
					Add(res);
					cache[name] = res;
				}
				res.amount += native.amount;
				res.maxAmount += native.maxAmount;
			}
		}
		public Resource this[string name]
		{
			get
			{
				if (Dirty) DoRefresh();
				return cache.TryGetValue(name, out var it) ? it : null;
			}
		}
		public double GetAmountOf(string name)
			=> this[name]?.Amount ?? 0.0;
		public double GetMaxAmountOf(string name)
			=> this[name]?.MaxAmount ?? 0.0;
	}
	public class Resource
	{
		public ResourceList List { get; }
		public string Name { get; }
		public int ID { get; }

		internal double amount, maxAmount;
		public double Amount
		{
			get
			{
				double now = Planetarium.GetUniversalTime();
				if (now != List.stamp) List.Update(now);
				return amount;
			}
		}
		public double MaxAmount
		{
			get
			{
				double now = Planetarium.GetUniversalTime();
				if (now != List.stamp) List.Update(now);
				return maxAmount;
			}
		}

		protected internal Resource(ResourceList list, string name)
		{
			List = list;
			Name = name;
			ID = name.GetHashCode();
		}
	}
}
