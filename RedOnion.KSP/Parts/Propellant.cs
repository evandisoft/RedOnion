using RedOnion.KSP.API;
using RedOnion.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using RedOnion.Attributes;
using System.Text;
using RedOnion.ROS;

namespace RedOnion.KSP.Parts
{
	[WorkInProgress, Description("List/set of propellants of single engine or list/set of engines.")]
	public class PropellantList : ReadOnlyList<Propellant>
	{
		protected Dictionary<string, Propellant> dict = new Dictionary<string, Propellant>();
		protected ModuleEngines engine;
		protected ReadOnlyList<Engine> engines;
		protected internal PropellantList(ModuleEngines engine)
			=> this.engine = engine;
		protected internal PropellantList(ReadOnlyList<Engine> engines)
			=> this.engines = engines;

		protected override void DoRefresh()
		{
			list.Clear();
			dict.Clear();
			if (engine != null)
			{
				foreach (var p in engine.propellants)
				{
					var it = new Propellant(p);
					dict[p.name] = it;
					Add(it);
				}
			}
			else
			{
				foreach (var e in engines)
				{
					foreach (var p in e.activeModule.propellants)
					{
						if (dict.TryGetValue(p.name, out var it))
						{
							it.list.Add(p);
							it.minimal = Math.Min(it.minimal, p.minResToLeave);
						}
						else
						{
							it = new Propellant(p);
							dict[p.name] = it;
							Add(it);
						}
					}
				}
			}
			base.DoRefresh();
		}

		[Description("Get propellant by name.")]
		public Propellant this[string name]
		{
			get
			{
				if (Dirty) DoRefresh();
				return dict.TryGetValue(name, out var it) ? it : null;
			}
		}

		// this turned out not to be what I was searching for
		public double getMinimalOf(string name)
			=> this[name]?.minimal ?? 0.0;

		public override string ToString()
		{
			if (Dirty) DoRefresh();
			bool first = true;
			var sb = new StringBuilder();
			sb.Append("[");
			foreach (var prop in list)
			{
				if (!first) sb.Append(", ");
				first = false;
				sb.Append(prop.name);
			}
			sb.Append("]");
			return sb.ToString();
		}

		public IEnumerable<string> names
		{
			get
			{
				foreach (var prop in this)
					yield return prop.name;
			}
		}
		public IEnumerable<Propellant> solid
		{
			get
			{
				foreach (var prop in this)
					if (prop.solid)
						yield return prop;
			}
		}
		public IEnumerable<Propellant> liquid
		{
			get
			{
				foreach (var prop in this)
					if (prop.liquid)
						yield return prop;
			}
		}
		public IEnumerable<string> namesOfSolid
		{
			get
			{
				foreach (var prop in this)
					if (prop.solid)
						yield return prop.name;
			}
		}
		public IEnumerable<string> namesOfLiquid
		{
			get
			{
				foreach (var prop in this)
					if (prop.liquid)
						yield return prop.name;
			}
		}
	}
	[WorkInProgress, Description("Propellant consumed by engine.")]
	public class Propellant
	{
		protected internal ListCore<global::Propellant> list;

		protected internal Propellant(global::Propellant p)
		{
			list.Add(p);
			minimal = p.minResToLeave;
		}

		[Description("Name of the propellant (like `LiquidFuel').")]
		public string name => list[0].name;
		// not what I was searching for, we need Engine.ignitionThreshold or something like that
		public double minimal { get; protected internal set; }

		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_propellant.html). First in the list if aggregate.")]
		public global::Propellant native => list[0];
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_part_resource_definition.html). First in the list if aggregate.")]
		public PartResourceDefinition resourceDef => native.resourceDef;
		[Description("Flow mode of the propellant. First in the list if aggregate, which works for most propellants, but may be random when you combine e.g. Karbonite SRB's with normal engines.")]
		public ResourceFlowMode flowMode => native.GetFlowMode();
		[WorkInProgress, Description("No flow propellant - usually solid fuel, bound to SRB.")]
		public bool solid => flowMode == ResourceFlowMode.NO_FLOW;
		[WorkInProgress, Description("Flowing propellant. May include electricity (if consumed by the engine, e.g. Ion Drive).")]
		public bool liquid
		{
			get
			{
				var mode = flowMode;
				return mode != ResourceFlowMode.NO_FLOW
					&& mode != ResourceFlowMode.ALL_VESSEL
					&& mode != ResourceFlowMode.ALL_VESSEL_BALANCE;
			}
		}

		public override string ToString() => name;
	}
}
