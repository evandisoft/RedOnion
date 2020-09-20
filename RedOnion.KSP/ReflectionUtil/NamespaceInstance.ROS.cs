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
			public override IEnumerable<string> EnumerateProperties(object self)
				=> ((NamespaceInstance)self).PossibleCompletions;
			public override void Get(Core core, ref Value self)
			{
				if (!((NamespaceInstance)self.obj).RosGet(ref self))
					GetError(ref self);
			}
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
		internal bool RosGet(ref Value self)
		{
			var name = self.idx as string;
			if (name == null && self.idx is ValueBox box
				&& box.Value.desc.Convert(ref box.Value, Descriptor.String))
			{
				name = (string)box.Value.obj;
				self.idx = name;
				ValueBox.Return(box);
			}
			if (name != null)
			{
				int at = RosFind(name);
				if (at >= 0)
				{
					self = ros.prop[at].value;
					return true;
				}
			}
			return false;
		}
	}
}
