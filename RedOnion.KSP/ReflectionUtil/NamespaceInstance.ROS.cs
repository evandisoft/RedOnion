using System;
using System.Collections.Generic;
using System.Diagnostics;
using RedOnion.KSP.ROS;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;

namespace RedOnion.KSP.ReflectionUtil
{
	public partial class NamespaceInstance : ISelfDescribing
	{
		Descriptor ISelfDescribing.Descriptor => NamespaceInstanceDescriptor.Instance;
		class NamespaceInstanceDescriptor : Descriptor
		{
			public static NamespaceInstanceDescriptor Instance { get; } = new NamespaceInstanceDescriptor();
			public NamespaceInstanceDescriptor() : base("namespace", typeof(NamespaceInstance)) { }
			public override int Find(object self, string name, bool add)
				=> ((NamespaceInstance)self).RosFind(name);
			public override bool Get(ref Value self, int at)
				=> ((NamespaceInstance)self.obj).RosGet(ref self, at);
			public override IEnumerable<string> EnumerateProperties(object self)
				=> ((NamespaceInstance)self).PossibleCompletions;
		}
		RosProps ros;
		internal int RosFind(string name)
		{
			if (ros.dict != null && ros.dict.TryGetValue(name, out var idx))
				return idx;
			if (TryGetSubNamespace(name, out var ns))
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
			if (TryGetType(name, out var type))
			{
				if (ros.dict == null)
					ros.dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				idx = ros.prop.size;
				ref var prop = ref ros.prop.Add();
				prop.name = name;
				prop.value = new Value(type);
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
