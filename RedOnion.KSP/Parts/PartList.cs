using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedOnion.KSP.API;
using System.ComponentModel;
using MoonSharp.Interpreter;
using KSP.UI.Screens;

namespace RedOnion.KSP.Parts
{
	public class PartList<Part> : ICollection<Part> where Part : PartBase
	{
		protected ListCore<Part> parts;
		public int Count => parts.Count;
		public bool Contains(Part item) => parts.Contains(item);
		public void CopyTo(Part[] array, int index) => parts.CopyTo(array, index);

		protected internal void Clear() => parts.Clear();
		protected internal void Add(Part item) => parts.Add(item);

		IEnumerator IEnumerable.GetEnumerator() => parts.GetEnumerator();
		public IEnumerator<Part> GetEnumerator() => parts.GetEnumerator();

		bool ICollection<Part>.IsReadOnly => true;
		void ICollection<Part>.Add(Part item) => throw new NotImplementedException();
		void ICollection<Part>.Clear() => throw new NotImplementedException();
		bool ICollection<Part>.Remove(Part item) => throw new NotImplementedException();
	}
	public class PartList : ICollection<PartBase>, IDisposable
	{
		protected ListCore<PartBase> parts;
		protected Dictionary<Part, PartBase> cache;

		public Ship Ship { get; protected set; }
		public PartBase Root { get; protected set; }
		public Decoupler NextDecoupler { get; protected set; }
		public PartList<Decoupler> Decouplers { get; protected set; }
		public PartList<DockingPort> DockingPorts { get; protected set; }
		public PartList<Engine> Engines { get; protected set; }

		protected internal PartList(Ship ship)
		{
			Ship = ship;
			Decouplers = new PartList<Decoupler>();
			DockingPorts = new PartList<DockingPort>();
			Engines = new PartList<Engine>();
		}

		~PartList() => Dispose(false);
		[Browsable(false), MoonSharpHidden]
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				parts.Clear();
				cache = null;
				Ship = null;
				Root = null;
				NextDecoupler = null;
				Decouplers = null;
				DockingPorts = null;
				Engines = null;
			}
		}

		//TODO: reuse wrappers
		protected void Refresh()
		{
			Root = null;
			NextDecoupler = null;
			Decouplers.Clear();
			DockingPorts.Clear();
			Engines.Clear();
			Construct(Ship.Native.rootPart, null, null);
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
					Engines.Add(engine);
					self = engine;
					break;
				}
				if (module is IStageSeparator)
				{
					var dock = module as ModuleDockingNode;
					if (dock != null)
					{
						var port = new DockingPort(Ship, part, parent, decoupler, dock);
						DockingPorts.Add(port);
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
					Decouplers.Add(decoupler);
					// ignore leftover decouplers
					if (decoupler.Part.inverseStage >= StageManager.CurrentStage)
						break;
					// check if we just created closer decoupler (see StageValues.CreatePartSet)
					if (NextDecoupler == null || decoupler.Part.inverseStage > NextDecoupler.Part.inverseStage)
						NextDecoupler = decoupler;
					break;
				}
				var sensor = module as ModuleEnviroSensor;
				if (sensor != null)
				{
					self = new Sensor(Ship, part, parent, decoupler, sensor);
					break;
				}
			}
			if (self == null)
				self = new PartBase(Ship, part, parent, decoupler);
			if (Root == null)
				Root = self;
			if (cache == null)
				cache = new Dictionary<Part, PartBase>();
			cache[part] = self;
			parts.Add(self);
			foreach (var child in part.children)
				Construct(child, self, decoupler);
		}

		public int Count
		{
			get
			{
				if (parts.Count == 0)
					Refresh();
				return parts.Count;
			}
		}
		public bool Contains(PartBase item)
		{
			if (parts.Count == 0)
				Refresh();
			return parts.Contains(item);
		}
		public void CopyTo(PartBase[] array, int index)
		{
			if (parts.Count == 0)
				Refresh();
			parts.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<PartBase> GetEnumerator()
		{
			if (parts.Count == 0)
				Refresh();
			return parts.GetEnumerator();
		}

		bool ICollection<PartBase>.IsReadOnly => true;
		void ICollection<PartBase>.Add(PartBase item) => throw new NotImplementedException();
		void ICollection<PartBase>.Clear() => throw new NotImplementedException();
		bool ICollection<PartBase>.Remove(PartBase item) => throw new NotImplementedException();
	}
}
