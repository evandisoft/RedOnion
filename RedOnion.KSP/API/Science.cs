using KSP.Localization;
using RedOnion.Attributes;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedOnion.KSP.API
{
	[WorkInProgress, Description("Science tools.")]
	public static class Science
	{
		internal static Vessel vessel
		{
			get
			{
				var vessel = FlightGlobals.ActiveVessel;
				return vessel ? vessel.EVALadderVessel : null;
			}
		}

		[Description("Current science situation.")]
		public static ExperimentSituations situation
		{
			get
			{
				var v = vessel;
				return v ? ScienceUtil.GetExperimentSituation(v) : 0;
			}
		}
		[Description("Current biome (tag, returns subBiome if landed).")]
		public static string biome
		{
			get
			{
				var v = vessel;
				return v ? v.landedAt?.Length > 0
					? Vessel.GetLandedAtString(v.landedAt)
					: ScienceUtil.GetExperimentBiome(
					v.mainBody, v.latitude, v.longitude)
					: null;
			}
		}
		[Description("Current biome (display name).")]
		public static string biomeName
		{
			get
			{
				var v = vessel;
				if (!v) return null;
				if (v.landedAt?.Length > 0)
				{
					var name = Localizer.Format(v.displaylandedAt);
					return name?.Length > 0 ? name : v.landedAt;
				}
				else
				{
					var biome = ScienceUtil.GetExperimentBiome(
						v.mainBody, v.latitude, v.longitude);
					var name = ScienceUtil.GetBiomedisplayName(
						v.mainBody, biome);
					return name?.Length > 0 ? name : biome;
				}
			}
		}
		[Description("Current main biome (tag, ignores landed).")]
		public static string mainBiome
		{
			get
			{
				var v = vessel;
				return v ? ScienceUtil.GetExperimentBiome(
					v.mainBody, v.latitude, v.longitude)
					: null;
			}
		}
		[Description("Current sub-biome (tag, valid only if landed).")]
		public static string subBiome
		{
			get
			{
				var v = vessel;
				return v && v.landedAt?.Length > 0
					? Vessel.GetLandedAtString(v.landedAt)
					: null;
			}
		}
	}
}
