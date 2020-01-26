using System;
using System.Collections;
using System.Collections.Generic;
using RedOnion.ROS;
using RedOnion.KSP.API;
using System.ComponentModel;
using MoonSharp.Interpreter;
using KSP.UI.Screens;
using static RedOnion.Debugging.QueueLogger;
using RedOnion.Attributes;

namespace RedOnion.KSP.Parts
{
	public interface IPartSet : ICollection<PartBase>, IReadOnlyCollection<PartBase>
	{
		bool Dirty { get; }
		void SetDirty();
		event Action Refresh;
	}
	[Description("Collection of ship-parts.")]
	public class PartSet<Part>
		: ReadOnlyList<Part>
		, IPartSet
		where Part : PartBase
	{
		protected Dictionary<global::Part, Part> cache = new Dictionary<global::Part, Part>();

		[Description("Resource collection of all the parts in this set.")]
		public ResourceList resources => _resources ?? (_resources = new ResourceList(this));
		protected ResourceList _resources;

		protected internal override void SetDirty()
		{
			base.SetDirty();
			if (_resources != null)
				_resources.SetDirty();
		}

		[Description("Ship (vessel/vehicle) this list of parts belongs to.")]
		public Ship ship
		{
			get
			{
				if (Dirty) DoRefresh();
				return _ship;
			}
			protected internal set => _ship = value;
		}
		protected Ship _ship;

		protected internal PartSet(Ship ship) => this.ship = ship;
		protected internal PartSet(Ship ship, Action refresh) : base(refresh) => this.ship = ship;

		bool IPartSet.Dirty => Dirty;
		int IReadOnlyCollection<PartBase>.Count => count;
		int ICollection<PartBase>.Count => count;
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
				if (cache.TryGetValue(part, out var it))
					return it;
				ApiLogger.Log($"Could not find part {part.persistentId}/{part.name} in ship {ship.id}/{ship.persistentId}/{ship.name}");
				return null;
			}
		}
	}
	[Description("Collection of all the parts in one ship/vessel.")]
	public class ShipPartSet : PartSet<PartBase>, IDisposable
	{
		protected Dictionary<Part, PartBase> prevCache;

		protected PartBase _root;
		[Description("Root part.")]
		public PartBase root
		{
			get
			{
				if (Dirty) DoRefresh();
				return _root;
			}
		}

		protected DecouplerBase _nextDecoupler;
		[Description("One of the decouplers that will get activated by nearest stage.")]
		public DecouplerBase nextDecoupler
		{
			get
			{
				if (Dirty) DoRefresh();
				return _nextDecoupler;
			}
		}
		[Description("Stage number of the nearest decoupler or -1. (`nextDecoupler?.stage ?? -1`)")]
		public int nextDecouplerStage => nextDecoupler?.stage ?? -1;

		[Description("List of all decouplers, separators, launch clamps and docks with staging enabled."
			+ " (Docking ports without staging enabled not included.)")]
		public ReadOnlyList<DecouplerBase> decouplers { get; protected set; }
		[Description("List of all docking ports (regardless of staging).")]
		public ReadOnlyList<DockingPort> dockingports { get; protected set; }
		[Description("All engines (regardless of state).")]
		public EngineSet engines { get; protected set; }
		[Description("All sensors.")]
		public ReadOnlyList<Sensor> sensors { get; protected set; }
		[WorkInProgress, Description("Parts per stage (by `decoupledin+1`).")]
		public Stages stages { get; protected set; }

		protected internal ShipPartSet(Ship ship) : base(ship)
		{
			decouplers = new ReadOnlyList<DecouplerBase>(DoRefresh);
			dockingports = new ReadOnlyList<DockingPort>(DoRefresh);
			engines = new EngineSet(ship, DoRefresh);
			sensors = new ReadOnlyList<Sensor>(DoRefresh);
			stages = new Stages(ship, DoRefresh);
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
			stages.SetDirty();
			if (_ship == Ship.Active)
				Stage.SetDirty();
		}
		void VesselModified(Vessel vessel)
		{
#if DEBUG
			if (_ship == null)
			{
				Value.DebugLog("VesselModified: ship not assigned");
				return;
			}
#endif
			if (vessel == _ship.native)
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
			if (!disposing)
			{
				UI.Collector.Add(this);
				return;
			}
			GameEvents.onVesselWasModified.Remove(VesselModified);
			cache = null;
			_ship = null;
			_root = null;
			_nextDecoupler = null;
			list.Clear();
			decouplers.Clear();
			dockingports.Clear();
			engines.Clear();
			sensors.Clear();
			decouplers = null;
			dockingports = null;
			engines = null;
			sensors = null;
			stages = null;
			prevCache = null;
		}

/*#if DEBUG
		bool refreshing;
#endif*/
		protected override void DoRefresh()
		{
/*#if DEBUG
			Value.DebugLog($"Refreshing Ship Parts, Guid: {_ship.id}");
			if (refreshing)
				throw new InvalidOperationException("Already refreshing");
			refreshing = true;
#endif*/
			_root = null;
			_nextDecoupler = null;
			decouplers.Clear();
			dockingports.Clear();
			engines.Clear();
			sensors.Clear();
			list.Clear();
			var prev = prevCache;
			prevCache = cache;
			cache = prev ?? new Dictionary<Part, PartBase>();
			cache.Clear();
			decouplers.Dirty = false;
			dockingports.Dirty = false;
			engines.Dirty = false;
			stages.Dirty = false;
			foreach (var stage in stages.list)
			{
				stage.Clear();
				stage.Dirty = false;
			}
			while (stages.list.Count <= _ship.currentStage)
				stages.list.Add(new StagePartSet(_ship, DoRefresh));
			Construct(_ship.native.rootPart, null, null);
			base.DoRefresh();
			prevCache.Clear();
			GameEvents.onVesselWasModified.Add(VesselModified);
			ApiLogger.Log($"Ship Parts Refreshed: {list.Count}, Engines: {engines.count}, Guid: {_ship.id}");
/*#if DEBUG
			Value.DebugLog($"Ship Parts Refreshed: {list.Count}, Engines: {engines.count}, Guid: {_ship.id}");
			refreshing = false;
#endif*/
		}
		protected void Construct(Part part, PartBase parent, DecouplerBase decoupler)
		{
			if (part.State == PartStates.DEAD || part.transform == null)
				return;

			//TODO: consider change of StagingEnabled - some parts may get improperly categorized
			Engine engine = null;
			if (prevCache.TryGetValue(part, out var self))
			{
				Value.DebugLog($"Wrapper for part {part} taken from cache");
				self.decoupler = decoupler;
				if (self is Engine eng)
					engine = eng;
				else if (self is DecouplerBase dec)
				{
					if (dec is DockingPort port)
						dockingports.Add(port);
					if (dec.staged)
					{
						decoupler = dec;
						decouplers.Add(dec);

						if ((_nextDecoupler == null || decoupler.stage > _nextDecoupler.stage)
							&& decoupler.stage < _ship.native.currentStage)
							_nextDecoupler = decoupler;
					}
				}
				else if (self is Sensor sensor)
					sensors.Add(sensor);
			}
			else
			{
				Value.DebugLog($"Creating wrapper for part {part}");
				foreach (var module in part.Modules)
				{
					if (module is IEngineStatus)
					{
						engine = new Engine(_ship, part, parent, decoupler);
						self = engine;
						break;
					}
					if (module is IStageSeparator)
					{
						var dock = module as ModuleDockingNode;
						if (dock != null)
						{
							var port = new DockingPort(_ship, part, parent, decoupler, dock);
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
							if (module is global::LaunchClamp calmpModule)
							{
								var clamp = new LaunchClamp(_ship, part, parent, decoupler, calmpModule);
								self = clamp;
								decoupler = clamp;
							}
							else if (module is ModuleDecouplerBase decouplerBase)
							{
								var separator = new Decoupler(_ship, part, parent, decoupler, decouplerBase);
								self = separator;
								decoupler = separator;
							}
							else // ModuleServiceModule ?
								continue; // rather continue the search
						}
						decouplers.Add(decoupler);
						// ignore leftover decouplers
						if (decoupler.native.inverseStage >= _ship.native.currentStage)
							break;
						// check if we just created closer decoupler
						if (_nextDecoupler == null || decoupler.stage > _nextDecoupler.stage)
							_nextDecoupler = decoupler;
						break;
					}
					var sensor = module as ModuleEnviroSensor;
					if (sensor != null)
					{
						var sense = new Sensor(_ship, part, parent, decoupler, sensor);
						sensors.Add(sense);
						self = sense;
						break;
					}
				}
				if (self == null)
					self = new PartBase(PartType.Unknown, _ship, part, parent, decoupler);
			}
			if (_root == null)
				_root = self;
			cache[part] = self;
			list.Add(self);
			var decoupledin = self.decoupledin;
			stages.list[decoupledin + 1].Add(self);
			if (engine != null)
			{
				engines.Add(engine);
				int upto = engine.ignited ? ship.currentStage : engine.staged ? engine.stage : 0;
				for (int i = decoupledin + 1; i >= upto; i--)
					stages.list[i].activeEngines.Add(engine);
			}
			foreach (var child in part.children)
				Construct(child, self, decoupler);
		}
	}
}
