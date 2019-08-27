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
			bodies = new ReadOnlyList<SpaceBody>(FillBodies);
		}

		[Description("Name of the body.")]
		public string name => native.bodyName;
		[Convert(typeof(Vector)), Description("Position of the body (relative to active ship).")]
		public Vector3d position => native.position;
		[Description("Celestial body this body is orbiting.")]
		public SpaceBody body => Bodies.Instance[native.referenceBody];
		ISpaceObject ISpaceObject.body => body;

		[Description("Orbiting celestial bodies.")]
		public ReadOnlyList<SpaceBody> bodies { get; }
		void FillBodies()
		{
			bodies.Clear();
			foreach (var child in native.orbitingBodies)
				bodies.Add(Bodies.Instance[child]);
		}

		[Description("Radius of the body [m].")]
		public double radius => native.Radius;
		[Description("Mass of the body [kg].")]
		public double mass => native.Mass;
		[Description("Standard gravitational parameter (μ = GM) [m³/s²]")]
		public double gravParameter => native.gravParameter;
		[Description("Standard gravitational parameter (μ = GM) [m³/s²]")]
		public double mu => native.gravParameter;

		[Description("Atmosphere parameters of the body.")]
		public Atmosphere atmosphere => new Atmosphere(native);
		[Description("Atmosphere parameters of the body. (Alias to `atmosphere`)")]
		public Atmosphere atm => new Atmosphere(native);

		public struct Atmosphere
		{
			readonly CelestialBody native;
			internal Atmosphere(CelestialBody native)
				=> this.native = native;

			[Description("Is there any atmosphere (true on Kerbin, false on Moon).")]
			public bool exists => native.atmosphere;
			[Description("Is there oxygen in the atmosphere.")]
			public bool oxygen => native.atmosphereContainsOxygen;
			[Description("Depth/height of the atmosphere.")]
			public double depth => native.atmosphereDepth;
			[Description("Depth/height of the atmosphere.")]
			public double height => native.atmosphereDepth;
		}
	}
}
