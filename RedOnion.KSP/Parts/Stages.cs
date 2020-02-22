using System;
using System.Collections;
using System.Collections.Generic;
using RedOnion.KSP.API;
using System.ComponentModel;
using RedOnion.Attributes;

namespace RedOnion.KSP.Parts
{
	[WorkInProgress, Description("Parts per stage (by `decoupledin+1`).")]
	public class Stages : ReadOnlyList<StagePartSet>
	{
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

		protected internal Stages(Ship ship, Action refresh) : base(refresh) => _ship = ship;
		protected internal override void SetDirty(bool value)
		{
			base.SetDirty(value);
			foreach (var stage in list)
				stage.Dirty = value;
		}
		protected internal override void Clear()
		{
			foreach (var stage in list)
				stage.Clear();
			//base.Clear(); - don't clear the list here, items reused
		}
	}
	[WorkInProgress, Description("Parts with same value of `decoupledin`.")]
	public class StagePartSet : PartSet<PartBase>
	{
		[Description("List of engines active in this stage.")]
		public readonly EngineSet activeEngines;

		protected internal StagePartSet(Ship ship, Action refresh) : base(ship, refresh)
			=> activeEngines = new EngineSet(ship, refresh);
		protected internal override void SetDirty(bool value)
		{
			activeEngines.SetDirty(value);
			base.SetDirty(value);
		}
		protected internal override void Clear()
		{
			base.Clear();
			activeEngines.Clear();
		}
	}
}
