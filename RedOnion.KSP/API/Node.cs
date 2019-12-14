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
				var ship = Ship.Active;
				if (ship == null)
					return null;
				var nodes = ship.native.patchedConicSolver?.maneuverNodes;
				if (nodes == null || nodes.Count == 0)
					return null;
				var mnode = nodes[0];
				if (cachedNext == null || cachedNext.native != mnode)
					cachedNext = new Node(mnode, ship);
				return cachedNext;
			}
		}

		[Unsafe, Description("KSP API.")]
		public ManeuverNode native { get; }
		[Description("Ship the node belongs to.")]
		public Ship ship { get; }

		protected internal Node(ManeuverNode native, Ship ship)
		{
			this.native = native;
			this.ship = ship;
		}

		[Description("Planned time for the maneuver.")]
		public double time => native.UT;
		[Description("Seconds until the maneuver.")]
		public double eta => native.UT - Time.now;

		[Description("Direction and amount of velocity change needed.")]
		public Vector deltav => new Vector(native.GetBurnVector(ship.orbit));
		[Description("Amount of velocity change in prograde direction.")]
		public double prograde => native.DeltaV.z;
		[Description("Amount of velocity change in normal direction.")]
		public double normal => native.DeltaV.y;
		[Description("Amount of velocity change in radial-out direction.")]
		public double radial => native.DeltaV.x;
	}
}
