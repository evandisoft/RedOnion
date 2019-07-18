using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using RedOnion.KSP.API;
using System.ComponentModel;
using MoonSharp.Interpreter;
using KSP.UI.Screens;

namespace RedOnion.KSP.Parts
{
	public interface IPartSet : ICollection<PartBase>
	{
		bool Dirty { get; }
		void SetDirty();
		event Action Refresh;
	}
	public class PartSet<Part>
		: ReadOnlyList<Part>
		, IPartSet
		where Part : PartBase
	{
		protected Dictionary<global::Part, Part> cache = new Dictionary<global::Part, Part>();
		public ResourceList Resources => resources ?? (resources = new ResourceList(this));
		protected ResourceList resources;

		protected internal override void SetDirty()
		{
			base.SetDirty();
			if (resources != null)
				resources.SetDirty();
		}

		protected internal PartSet() { }
		protected internal PartSet(Action refresh) : base(refresh) { }

		bool IPartSet.Dirty => Dirty;
		bool ICollection<PartBase>.IsReadOnly => true;
		void IPartSet.SetDirty() => SetDirty();
		event Action IPartSet.Refresh
		{
			add => Refresh += value;
			remove => Refresh -= value;
		}
		IEnumerator<PartBase> IEnumerable<PartBase>.GetEnumerator()
		{
			foreach (var part in this)
				yield return part;
		}
		void ICollection<PartBase>.Add(PartBase item) => throw new NotImplementedException();
		void ICollection<PartBase>.Clear() => throw new NotImplementedException();
		bool ICollection<PartBase>.Remove(PartBase item) => throw new NotImplementedException();
		bool ICollection<PartBase>.Contains(PartBase item)
		{
			if (Dirty) DoRefresh();
			return cache.ContainsKey(item.native);
		}
		void ICollection<PartBase>.CopyTo(PartBase[] array, int index)
		{
			foreach (var part in this)
				array[index++] = part;
		}

		public override bool Contains(Part item)
		{
			if (Dirty) DoRefresh();
			return cache.ContainsKey(item.native);
		}

		protected internal override void Clear()
		{
			list.Clear();
			cache.Clear();
		}
		protected internal override bool Add(Part item)
		{
			if (cache.ContainsKey(item.native))
				return false;
			list.Add(item);
			cache[item.native] = item;
			return true;
		}

		public Part this[global::Part part]
		{
			get
			{
				if (Dirty) DoRefresh();
				return cache.TryGetValue(part, out var it) ? it : null;
			}
		}
	}
	public class ShipPartSet : PartSet<PartBase>, IDisposable
	{
		[Description("Ship (vessel/vehicle) this list of parts belongs to.")]
		public Ship Ship { get; protected set; }

		protected PartBase root;
		[Description("Root part.")]
		public PartBase Root
		{
			get
			{
				if (Dirty) DoRefresh();
				return root;
			}
		}

		protected Decoupler nextDecoupler;
		[Description("One of the decouplers that will get activated by nearest stage. (Same as `Parts.NextDecoupler`.)")]
		public Decoupler NextDecoupler
		{
			get
			{
				if (Dirty) DoRefresh();
				return nextDecoupler;
			}
		}
		[Description("Stage number of the nearest decoupler or -1. (`NextDecoupler?.Stage ?? -1`)")]
		public int nextDecouplerStage => NextDecoupler?.stage ?? -1;

		[Description("List of all decouplers, separators, launch clamps and docks with staging enabled."
			+ " (Docking ports without staging enabled not included.)")]
		public ReadOnlyList<Decoupler> decouplers { get; protected set; }
		[Description("List of all docking ports (regardless of staging).")]
		public ReadOnlyList<DockingPort> dockingports { get; protected set; }
		[Description("All engines (regardless of state).")]
		public EngineSet engines { get; protected set; }
		[Description("All sensors.")]
		public ReadOnlyList<Sensor> sensors { get; protected set; }

		protected internal ShipPartSet(Ship ship)
		{
			Ship = ship;
			decouplers = new ReadOnlyList<Decoupler>(DoRefresh);
			dockingports = new ReadOnlyList<DockingPort>(DoRefresh);
			engines = new EngineSet(DoRefresh);
			sensors = new ReadOnlyList<Sensor>(DoRefresh);
		}
		protected internal override void SetDirty()
		{
			if (Dirty) return;
			RedOnion.ROS.Value.DebugLog("Ship Parts Dirty");
			GameEvents.onVesselWasModified.Remove(VesselModified);
			base.SetDirty();
			decouplers.SetDirty();
			dockingports.SetDirty();
			engines.SetDirty();
			sensors.SetDirty();
			if (Ship == Ship.Active)
				Stage.SetDirty();
		}
		void VesselModified(Vessel vessel)
		{
			if (vessel == Ship.native)
				SetDirty();
		}

		~ShipPartSet() => Dispose(false);
		[Browsable(false), MoonSharpHidden]
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (cache == null)
				return;
			GameEvents.onVesselWasModified.Remove(VesselModified);
			cache = null;
			Ship = null;
			root = null;
			nextDecoupler = null;
			if (disposing)
			{
				list.Clear();
				decouplers.Clear();
				dockingports.Clear();
				engines.Clear();
				sensors.Clear();
			}
			decouplers = null;
			dockingports = null;
			engines = null;
			sensors = null;
		}

		//TODO: reuse wrappers
		protected override void DoRefresh()
		{
			root = null;
			nextDecoupler = null;
			decouplers.Clear();
			dockingports.Clear();
			engines.Clear();
			sensors.Clear();
			Construct(Ship.native.rootPart, null, null);
			base.DoRefresh();
			decouplers.Dirty = false;
			dockingports.Dirty = false;
			engines.Dirty = false;
			GameEvents.onVesselWasModified.Add(VesselModified);
			RedOnion.ROS.Value.DebugLog("Ship Parts Refreshed (Parts: {0}, Engines: {1})", list.Count, engines.Count);
		}
		protected void Construct(Part part, PartBase parent, Decoupler decoupler)
		{
			if (part.State == PartStates.DEAD || part.transform == null)
				return;

			PartBase self = null;
			foreach (var module in part.Modules)
			{
				if (module is IEngineStatus)
				{
					var engine = new Engine(Ship, part, parent, decoupler);
					engines.Add(engine);
					self = engine;
					break;
				}
				if (module is IStageSeparator)
				{
					var dock = module as ModuleDockingNode;
					if (dock != null)
					{
						var port = new DockingPort(Ship, part, parent, decoupler, dock);
						dockingports.Add(port);
						self = port;
						if (!module.StagingEnabled())
							break;
						decoupler = port;
					}
					else
					{
						// ignore anything with staging disabled and continue the search
						// this can e.g. be heat shield or some sensor with integrated decoupler
						if (!module.StagingEnabled())
							continue;
						if (module is global::LaunchClamp)
						{
							var clamp = new LaunchClamp(Ship, part, parent, decoupler);
							self = clamp;
							decoupler = clamp;
						}
						else if (module is ModuleDecouple || module is ModuleAnchoredDecoupler)
						{
							var separator = new Decoupler(Ship, part, parent, decoupler);
							self = separator;
							decoupler = separator;
						}
						else // ModuleServiceModule ?
							continue; // rather continue the search
					}
					decouplers.Add(decoupler);
					// ignore leftover decouplers
					if (decoupler.native.inverseStage >= StageManager.CurrentStage)
						break;
					// check if we just created closer decoupler (see StageValues.CreatePartSet)
					if (nextDecoupler == null || decoupler.native.inverseStage > nextDecoupler.native.inverseStage)
						nextDecoupler = decoupler;
					break;
				}
				var sensor = module as ModuleEnviroSensor;
				if (sensor != null)
				{
					var sense = new Sensor(Ship, part, parent, decoupler, sensor);
					sensors.Add(sense);
					self = sense;
					break;
				}
			}
			if (self == null)
				self = new PartBase(Ship, part, parent, decoupler);
			if (root == null)
				root = self;
			if (cache == null)
				cache = new Dictionary<Part, PartBase>();
			cache[part] = self;
			list.Add(self);
			foreach (var child in part.children)
				Construct(child, self, decoupler);
		}
	}
}
