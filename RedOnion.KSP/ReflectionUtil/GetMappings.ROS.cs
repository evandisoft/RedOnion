using System;
using System.Collections.Generic;
using RedOnion.Common.Completion;
using RedOnion.KSP.ROS;
using RedOnion.ROS;

namespace RedOnion.KSP.ReflectionUtil
{
	public partial class GetMappings : ISelfDescribing
	{
		Descriptor ISelfDescribing.Descriptor => GetMappingsDescriptor.Instance;
		class GetMappingsDescriptor : Descriptor
		{
			public static GetMappingsDescriptor Instance { get; } = new GetMappingsDescriptor();
			public GetMappingsDescriptor() : base("mappings", typeof(GetMappings)) { }
			public override int Find(object self, string name, bool add)
				=> ((GetMappings)self).RosFind(name);
			public override bool Get(ref Value self, int at)
				=> ((GetMappings)self.obj).RosGet(ref self, at);
			public override IEnumerable<string> EnumerateProperties(object self)
				=> ((ICompletable)self).PossibleCompletions;
		}
		RosProps ros;
		internal int RosFind(string name)
		{
			if (ros.dict != null && ros.dict.TryGetValue(name, out var idx))
				return idx;
			if (TryGetAssemblyNamespaceInstance(name, out var ns))
			{
				if (ros.dict == null)
					ros.dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				idx = ros.prop.size;
				ref var prop = ref ros.prop.Add();
				prop.name = name;
				prop.value = new Value(ns);
				ros.dict[name] = idx;
				return idx;
			}
			return -1;
		}
		internal bool RosGet(ref Value self, int at)
		{
			if (at < 0 || at >= ros.prop.size)
				return false;
			self = ros.prop.items[at].value;
			return true;
		}
		internal string RosNameOf(int at)
			=> ros.prop.items[at].name;
	}
}
