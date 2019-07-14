using System;
using RedOnion.ROS;
using KSP.UI.Screens;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.ComponentModel;
using RedOnion.KSP.Parts;

namespace RedOnion.KSP.API
{
	[Description("Active vessel")]
	public class Ship : IDisposable
	{
		static Ship active;
		[Browsable(false), MoonSharpHidden]
		public static Ship Active
		{
			get
			{
				if (!HighLogic.LoadedSceneIsFlight)
				{
					if (active != null)
						ClearActive();
					return null;
				}
				var vessel = FlightGlobals.ActiveVessel;
				if (active?.Native != vessel)
				{
					ClearActive();
					if (vessel != null)
					{
						active = new Ship(vessel);
						Stage.SetDirty();
						GameEvents.onVesselChange.Add(active.VesselChange);
					}
				}
				return active;
			}
		}
		static void ClearActive(bool disposing = false)
		{
			if (active == null)
				return;
			GameEvents.onVesselChange.Remove(active.VesselChange);
			Stage.SetDirty();
			if (!disposing)
				active.Dispose();
			active = null;
		}
		// this would be static if EventData<T>.EvtDelegate
		// would not try to access evt.Target.GetType().Name
		// (evt.Target is null for static functions)
		void VesselChange(Vessel vessel)
			=> ClearActive();

		protected Ship(Vessel vessel)
		{
			Native = vessel;
			Parts = new ShipPartSet(this);
			GameEvents.onGameSceneLoadRequested.Add(SceneChange);
		}

		~Ship() => Dispose(false);
		[Browsable(false), MoonSharpHidden]
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (ReferenceEquals(active, this))
				ClearActive(disposing: true);
			if (Parts != null)
			{
				Parts.Dispose();
				Parts = null;
			}
			if (Native != null)
			{
				GameEvents.onGameSceneLoadRequested.Remove(SceneChange);
				Native = null;
			}
		}
		void SceneChange(GameScenes scene)
		{
			if (scene != GameScenes.FLIGHT)
				Dispose();
		}

		public Vessel Native { get; private set; }
		public ShipPartSet Parts { get; private set; }
		public PartBase Root => Parts.Root;
		public Decoupler NextDecoupler => Parts.NextDecoupler;
		public int NextDecouplerStage => Parts.NextDecouplerStage;
		public ReadOnlyList<Decoupler> Decouplers => Parts.Decouplers;
		public ReadOnlyList<DockingPort> DockingPorts => Parts.DockingPorts;
		public ReadOnlyList<Engine> Engines => Parts.Engines;

		public Guid ID => Native.id;
		public uint PersistentID => Native.persistentId;
		public VesselType VesselType => Native.vesselType;
		public float Mass => Native.GetTotalMass();
		public bool Packed => Native.packed;
		public bool Landed => Native.Landed;
		public bool Splashed => Native.Splashed;
		public double Longitude => Native.longitude;
		public double Latitude => Native.latitude;
		public double Altitude => Native.altitude;
		public double RadarAltitude => Native.radarAltitude;

		public Orbit Orbit => Native.orbit;
		public CelestialBody Body => Native.orbit.referenceBody;
		public double Eccentricity => Native.orbit.eccentricity;
		public double SemiMajorAxis => Native.orbit.semiMajorAxis;
		public double SemiMinorAxis => Native.orbit.semiMinorAxis;
		public double Apoapsis => Native.orbit.ApA;
		public double Periapsis => Native.orbit.PeA;
		public double Apocenter => Native.orbit.ApR;
		public double Pericenter => Native.orbit.PeR;
		public double TimeToAp => Native.orbit.timeToAp;
		public double TimeToPe => Native.orbit.timeToPe;
		public double Period => Native.orbit.period;
		public double TrueAnomaly => Native.orbit.trueAnomaly;
		public double MeanAnomaly => Native.orbit.meanAnomaly;

		[Convert(typeof(Vector))]
		public Vector3d Position => Native.orbit.pos;
		[Convert(typeof(Vector))]
		public Vector3d Velocity => Native.orbit.vel;
		[return: Convert(typeof(Vector))]
		public Vector3d PositionAt(double time) => Native.orbit.getPositionAtUT(time);
		[return: Convert(typeof(Vector))]
		public Vector3d VelocityAt(double time) => Native.orbit.getOrbitalVelocityAtUT(time);
	}
}
