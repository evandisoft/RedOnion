using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Completion;
using RedOnion.KSP.Utilities;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.API
{
	public interface ISpaceObject
	{
		Vector3d position { get; }
		ISpaceObject body { get; }
	} 
	public class Bodies : Properties<SpaceBody>.WithMap<CelestialBody>
	{
		public static Bodies Instance { get; } = new Bodies();

		protected Bodies()
		{
			var sb = new StringBuilder();
			foreach (var body in FlightGlobals.Bodies)
			{
				var it = new SpaceBody(body);
				map[body] = it;
				strict[body.bodyName] = list.size;
				sb.Append(body.bodyName.Trim());
				if (sb.Length > 0 && char.IsUpper(sb[0]))
					sb[0] = char.ToLowerInvariant(sb[0]);
				for (int i = 0; i < sb.Length;)
				{
					if (sb[i] == ' ')
					{
						sb.Remove(i, 1);
						if (char.IsLower(sb[i]))
							sb[i] = char.ToUpper(sb[i]);
					}
					else i++;
				}
				var name = sb.ToString();
				dict[name] = list.size;
				list.Add(new KeyValuePair<string, SpaceBody>(name, it));
				sb.Length = 0;
			}
		}
	}
	public class SpaceBody : ISpaceObject
	{
		[Unsafe, Description("KSP API. Native `CelestialBody`.")]
		public CelestialBody native { get; private set; }
		protected internal SpaceBody(CelestialBody body)
		{
			native = body;
			orbiting = new ReadOnlyList<SpaceBody>(FillOrbiting);
		}

		[Description("Name of the body.")]
		public string name => native.bodyName;
		[Convert(typeof(Vector)), Description("Position of the body (relative to active ship).")]
		public Vector3d position => native.position;
		[Description("Celestial body this body is orbiting.")]
		public ISpaceObject body => Bodies.Instance[native.referenceBody];

		[Description("Orbiting celestial bodies.")]
		public ReadOnlyList<SpaceBody> orbiting { get; }
		void FillOrbiting()
		{
			orbiting.Clear();
			foreach (var child in native.orbitingBodies)
				orbiting.Add(Bodies.Instance[child]);
		}
	}
}
