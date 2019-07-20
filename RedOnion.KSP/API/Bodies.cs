using MoonSharp.Interpreter;
using RedOnion.ROS;
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
	public class Bodies : IEnumerable<SpaceBody>
	{
		static protected readonly Dictionary<CelestialBody, SpaceBody> map = new Dictionary<CelestialBody, SpaceBody>();
		static protected readonly Dictionary<string, SpaceBody> names = new Dictionary<string, SpaceBody>(StringComparer.OrdinalIgnoreCase);
		static protected readonly Dictionary<string, SpaceBody> props = new Dictionary<string, SpaceBody>(StringComparer.OrdinalIgnoreCase);
		public static Bodies Instance { get; } = new Bodies();
		protected Bodies() { }
		static Bodies()
		{
			var sb = new StringBuilder();
			foreach (var body in FlightGlobals.Bodies)
			{
				var it = new SpaceBody(body);
				map[body] = it;
				names[body.bodyName] = it;
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
				props[sb.ToString()] = it;
				sb.Length = 0;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		[Browsable(false), MoonSharpHidden]
		public IEnumerator<SpaceBody> GetEnumerator() => map.Values.GetEnumerator();
		[Browsable(false), MoonSharpHidden]
		public SpaceBody this[CelestialBody key] => map[key];
		[Description("Get the celesial body by name.")]
		public SpaceBody this[string name]
		{
			get
			{
				if (names.TryGetValue(name, out var body))
					return body;
				if (props.TryGetValue(name, out body))
					return body;
				return null;
			}
		}
	}
	public class SpaceBody : ISpaceObject
	{
		[Unsafe, Description("KSP API. Native `CelestialBody`.")]
		public CelestialBody native { get; private set; }
		protected internal SpaceBody(CelestialBody body) => native = body;

		public Vector3d position => native.position;
		public ISpaceObject body => throw new NotImplementedException();
	}
}
