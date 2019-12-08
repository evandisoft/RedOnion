using RedOnion.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedOnion.KSP.API
{
	[WorkInProgress, Description("Maneuver node.")]
	public class Node
	{
		static Node cachedNext;
		[Description("Next maneuver node of active ship. Null if none or in wrong scene.")]
		public static Node next
		{
			get
			{
				if (!HighLogic.LoadedSceneIsFlight)
					return null;
				var nodes = FlightGlobals.ActiveVessel?.patchedConicSolver?.maneuverNodes;
				if (nodes == null || nodes.Count == 0)
					return null;
				var mnode = nodes[0];
				if (cachedNext == null || cachedNext.native != mnode)
					cachedNext = new Node(mnode);
				return cachedNext;
			}
		}

		[Unsafe, Description("KSP API.")]
		public ManeuverNode native { get; }

		protected internal Node(ManeuverNode native)
			=> this.native = native;

		[Description("Delta-V of the node (direction and amount of velocity change needed).")]
		public Vector deltav => new Vector(native.DeltaV);

		[Description("Planned time for the maneuver.")]
		public double time => native.UT;
	}
}
