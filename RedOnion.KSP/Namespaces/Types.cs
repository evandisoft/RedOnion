using RedOnion.Attributes;
using RedOnion.KSP.API;
using RedOnion.KSP.Parts;
using System;
using System.ComponentModel;

namespace RedOnion.KSP.Namespaces
{
	[SafeProps, DisplayName("Types"), DocBuild("RedOnion.KSP/Namespaces/Types")]
	[WorkInProgress, Description("Types to be used with ROS: `is` operator; Lua: `isa` function.")]
	public static class Types_Namespace
	{
		[Description("Ship / vessel / vehicle.")]
		public static readonly Type ship = typeof(Ship);
		[Description("Space/celestial body.")]
		public static readonly Type body = typeof(SpaceBody);
		[Description("Ship part.")]
		public static readonly Type part = typeof(PartBase);
	}
}
