//#define DEBUG_PARTS_REFRESH

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
using System.Text;

namespace RedOnion.KSP.Parts
{
	public interface IPartSet : ICollection<PartBase>, IReadOnlyCollection<PartBase>
	{
		bool Dirty { get; }
		void SetDirty();
		void Update();
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

		protected internal override void SetDirty(bool value)
		{
			if (value) _resources?.SetDirty(value);
			base.SetDirty(value);
		}

		[Description("Ship (vessel/vehicle) this list of parts belongs to.")]
		public Ship ship
		{
			get
			{
				Update();
				return _ship;
			}
			protected internal set => _ship = value;
		}
		protected Ship _ship;

		protected internal PartSet(Ship ship) => this.ship = ship;
		protected internal PartSet(Ship ship, Action refresh) : base(refresh) => this.ship = ship;

		int IReadOnlyCollection<PartBase>.Count => count;
		int ICollection<PartBase>.Count => count;
		bool ICollection<PartBase>.IsReadOnly => true;
		bool IPartSet.Dirty => Dirty;
		void IPartSet.SetDirty() => Dirty = true;
		void IPartSet.Update() => Update();
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
			Update();
			return cache.ContainsKey(item.native);
		}
		void ICollection<PartBase>.CopyTo(PartBase[] array, int index)
		{
			foreach (var part in this)
				array[index++] = part;
		}

		public override bool Contains(Part item)
		{
			Update();
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
				Update();
				if (cache.TryGetValue(part, out var it))
					return it;
				ApiLogger.Log($"Could not find part {part.persistentId}/{part.name} in ship {ship.id}/{ship.persistentId}/{ship.name}");
				return null;
			}
		}

		public override string ToString()
		{
			Update();
			bool first = true;
			var sb = new StringBuilder();
			sb.Append("[");
			foreach (var part in list)
			{
				if (!first) sb.Append(", ");
				first = false;
				sb.Append(part.name);
			}
			sb.Append("]");
			return sb.ToString();
		}
	}
	[Description("Collection of all the parts in one ship/vessel.")]
	public class ShipPartSet : PartSet<PartBase>, IDisposable
	{
		protected Dictionary<Part, PartBase> prevCache = new Dictionary<Part, PartBase>();

		protected PartBase _root;
		[Description("Root part.")]
		public PartBase root
		{
			get
			{
				Update();
				return _root;
			}
		}

		[Description("Controlling part.")]
		public PartBase control
		{
			get
			{
				var ctl = ship.native.GetReferenceTransformPart();
				return ctl == null ? root : this[ctl];
			}
		}

		protected LinkPart _nextDecoupler;
		[Description("One of the decouplers that will get activated by nearest stage.")]
		public LinkPart nextDecoupler
		{
			get
			{
				Update();
				return _nextDecoupler;
			}
		}
		[Description("Stage number of the nearest decoupler or -1. (`nextDecoupler?.stage ?? -1`)")]
		public int nextDecouplerStage => nextDecoupler?.stage ?? -1;

		[Description("List of all decouplers, separators, launch clamps and docks with staging enabled."
			+ " (Docking ports without staging enabled not included.)")]
		public ReadOnlyList<LinkPart> decouplers { get; protected set; }
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
			decouplers = new ReadOnlyList<LinkPart>(DoRefresh);
			dockingports = new ReadOnlyList<DockingPort>(DoRefresh);
			engines = new EngineSet(ship, DoRefresh);
			sensors = new ReadOnlyList<Sensor>(DoRefresh);
			stages = new Stages(ship, DoRefresh);
		}
		protected internal override void SetDirty(bool value)
		{
#if DEBUG && DEBUG_PARTS_REFRESH
			Value.DebugLog($"Ship Parts Dirty = {value}");
#endif
			if (value)
			{
				GameEvents.onVesselWasModified.Remove(VesselModified);
				if (_ship == Ship.Active)
					Stage.SetDirty("PartSet Dirty");
			}
			decouplers.Dirty = value;
			dockingports.Dirty = value;
			engines.Dirty = value;
			sensors.Dirty = value;
			stages.Dirty = value;
			base.SetDirty(value);
		}
		protected internal override void Clear()
		{
			var prev = prevCache;
			prevCache = cache;
			cache = prev;
			//not needed, cleared as part of SetDirty
			//decouplers.Clear();
			//dockingports.Clear();
			//engines.Clear();
			//sensors.Clear();
			//stages.Clear();
			base.Clear();
		}
		void VesselModified(Vessel vessel)
		{
#if DEBUG && DEBUG_PARTS_REFRESH
			if (_ship == null)
			{
				Value.DebugLog("VesselModified: ship not assigned");
				return;
			}
#endif
			if (vessel == _ship.native)
				Dirty = true;
		}

		~ShipPartSet() => Dispose(false);
		[Browsable(false), MoonSharpHidden]
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
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
			stages.Clear();
			decouplers = null;
			dockingports = null;
			engines = null;
			sensors = null;
			stages = null;
			prevCache = null;
		}

#if DEBUG && DEBUG_PARTS_REFRESH
		bool refreshing;
#endif
		protected override void DoRefresh()
		{
#if DEBUG && DEBUG_PARTS_REFRESH
			Value.DebugLog($"Refreshing Ship Parts, Guid: {_ship.id}");
			if (refreshing)
				throw new InvalidOperationException("Already refreshing");
			if (!Dirty)
				throw new InvalidOperationException("Not dirty");
			refreshing = true;
#endif
			Dirty = false;
			_root = null;
			_nextDecoupler = null;
			while (stages.list.Count <= _ship.currentStage)
				stages.list.Add(new StagePartSet(_ship, DoRefresh));
			stages.list.Count = _ship.currentStage + 1;

			Construct(_ship.native.rootPart, null, null);

			prevCache.Clear();
			Refresh?.Invoke();
			GameEvents.onVesselWasModified.Add(VesselModified);
			ApiLogger.Log($"Ship Parts Refreshed: {list.Count}, Engines: {engines.count}, Guid: {_ship.id}");

#if DEBUG && DEBUG_PARTS_REFRESH
			Value.DebugLog($"Ship Parts Refreshed: {list.Count}/{cache.Count}, Engines: {engines.count}, Guid: {_ship.id}");
			refreshing = false;
#endif
		}
		protected void Construct(Part part, PartBase parent, LinkPart decoupler)
		{
			if (part.State == PartStates.DEAD || part.transform == null)
				return;

			//TODO: consider change of StagingEnabled - some parts may get improperly categorized
			Engine engine = null;
			if (prevCache.TryGetValue(part, out var self))
			{
#if DEBUG && DEBUG_PARTS_REFRESH
				Value.DebugLog($"Wrapper for part {part} taken from cache, decoupler: {decoupler}/{decoupler?.stage??-1}");
#endif
				self.decoupler = decoupler;
				if (self is Engine eng)
					engine = eng;
				else if (self is LinkPart dec)
				{
					if (dec is DockingPort port)
						dockingports.Add(port);
					if (dec.staged)
					{
						decoupler = dec;
						decouplers.Add(dec);
#if DEBUG && DEBUG_PARTS_REFRESH
						Value.DebugLog($"Part {part} is decoupler");
#endif

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
#if DEBUG && DEBUG_PARTS_REFRESH
				Value.DebugLog($"Creating wrapper for part {part}, decoupler: {decoupler}/{decoupler?.stage??-1}");
#endif
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
#if DEBUG && DEBUG_PARTS_REFRESH
						Value.DebugLog($"Part {part} is decoupler");
#endif
						decouplers.Add(decoupler);
						// ignore leftover decouplers
						if (decoupler.stage >= _ship.currentStage)
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
				int upto = engine.ignited ? ship.currentStage : engine.stage;
				for (int i = decoupledin + 1; i >= upto; i--)
					stages.list[i].activeEngines.Add(engine);
			}
			foreach (var child in part.children)
				Construct(child, self, decoupler);
		}
	}
}
