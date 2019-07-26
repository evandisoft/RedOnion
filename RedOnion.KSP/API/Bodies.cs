using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Completion;
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
	public class Bodies : IEnumerable<SpaceBody>, ISelfDescribing, IUserDataType, ICompletable
	{
		static protected readonly Dictionary<CelestialBody, SpaceBody> map = new Dictionary<CelestialBody, SpaceBody>();
		static protected readonly Dictionary<string, SpaceBody> names = new Dictionary<string, SpaceBody>(StringComparer.OrdinalIgnoreCase);
		static protected readonly ListCore<KeyValuePair<string, SpaceBody>> propList;
		static protected readonly Dictionary<string, int> propDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
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
				var name = sb.ToString();
				propDict[name] = propList.size;
				propList.Add(new KeyValuePair<string, SpaceBody>(name, it));
				sb.Length = 0;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		[Browsable(false), MoonSharpHidden]
		public IEnumerator<SpaceBody> GetEnumerator() => map.Values.GetEnumerator();

		[Browsable(false), MoonSharpHidden]
		public SpaceBody this[CelestialBody key]
			=> key == null ? null : map.TryGetValue(key, out var it) ? it : null;
		[Description("Get the celestial body by name.")]
		public SpaceBody this[string name]
		{
			get
			{
				if (names.TryGetValue(name, out var body))
					return body;
				if (propDict.TryGetValue(name, out var index))
					return propList[index].Value;
				return null;
			}
		}

		IList<string> ICompletable.PossibleCompletions => propDict.Keys.ToList();
		bool ICompletable.TryGetCompletion(string completionName, out object completion)
		{
			if (propDict.TryGetValue(completionName, out var at))
			{
				completion = propList[at].Value;
				return true;
			}
			completion = null;
			return false;
		}
		DynValue IUserDataType.Index(Script script, DynValue index, bool isDirectIndexing)
		{
			var name = index.String;
			if (!isDirectIndexing && names.TryGetValue(name, out var body))
				return DynValue.FromObject(script, body);
			if (propDict.TryGetValue(name, out var at))
				return DynValue.FromObject(script, propList[at].Value);
			return DynValue.Nil;
		}
		bool IUserDataType.SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
			=> false;
		DynValue IUserDataType.MetaIndex(Script script, string metaname)
			=> null;

		Descriptor ISelfDescribing.Descriptor => SelfDescriptor.Instance;
		protected class SelfDescriptor : Descriptor
		{
			public static SelfDescriptor Instance { get; } = new SelfDescriptor();
			protected SelfDescriptor() { }
			public override int Find(object self, string name, bool add = false)
				=> propDict.TryGetValue(name, out var at) ? at : -1;
			public override string NameOf(object self, int at)
				=> at < 0 || at >= propList.size ? "[?]" : propList[at].Key;
			public override bool Get(ref Value self, int at)
			{
				self = new Value(propList[at].Value);
				return true;
			}
			public override int IndexFind(ref Value self, Arguments args)
			{
				if (args.Length != 1)
					return -1;
				var index = args[0];
				if (!index.desc.Convert(ref index, String))
					return -1;
				return propDict.TryGetValue(index.obj.ToString(), out var at) ? at : -1;
			}
			public override IEnumerable<string> EnumerateProperties(object self)
			{
				foreach (var pair in propList)
					yield return pair.Key;
			}
			public override IEnumerable<Value> Enumerate(object self)
			{
				foreach (var pair in propList)
					yield return new Value(pair.Value);
			}
		}
	}
	public class SpaceBody : ISpaceObject
	{
		[Unsafe, Description("KSP API. Native `CelestialBody`.")]
		public CelestialBody native { get; private set; }
		protected internal SpaceBody(CelestialBody body) => native = body;

		[Description("Position of the body (relative to active ship)")]
		public Vector3d position => native.position;
		[Description("Celestial body this body is orbiting.")]
		public ISpaceObject body => Bodies.Instance[native.referenceBody];
	}
}
