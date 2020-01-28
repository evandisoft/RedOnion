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
		[Description("Decoupler, separator, engine plate, launch clamp or docking port.")]
		public static readonly Type link = typeof(LinkPart);
		[Description("Decoupler, separator or engine plate.")]
		public static readonly Type decoupler = typeof(Decoupler);
		[Description("Docking port.")]
		public static readonly Type dock = typeof(DockingPort);
		[Description("Docking port.")]
		public static readonly Type port = typeof(DockingPort);
		[Description("Launch Clamp.")]
		public static readonly Type clamp = typeof(LaunchClamp);
		[Description("Engine.")]
		public static readonly Type engine = typeof(Engine);
		[Description("Sensor.")]
		public static readonly Type sensor = typeof(Sensor);
	}
}
