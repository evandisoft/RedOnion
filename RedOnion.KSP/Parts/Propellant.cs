using RedOnion.KSP.API;
using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.Parts
{
	[Description("List/set of propellants of single engine or list/set of engines.")]
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
					foreach (var p in e.ActiveModule.propellants)
					{
						if (dict.TryGetValue(p.name, out var it))
						{
							it.list.Add(p);
							it.Minimal = Math.Min(it.Minimal, p.minResToLeave);
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
		[Description("Get minimal amount of propellant required by the engine(s).")]
		public double GetMinimalOf(string name)
			=> this[name]?.Minimal ?? 0.0;
	}
	public class Propellant
	{
		protected internal ListCore<global::Propellant> list;

		protected internal Propellant(global::Propellant p)
		{
			list.Add(p);
			Minimal = p.minResToLeave;
		}

		public string Name => list[0].name;
		public double Minimal { get; protected internal set; }
	}
}
