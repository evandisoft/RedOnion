using System;
using System.Collections.Generic;
using RedOnion.Common.Completion;
using RedOnion.KSP.ROS;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;

namespace RedOnion.KSP.ReflectionUtil
{
	public partial class GetMappings : ISelfDescribing
	{
		Descriptor ISelfDescribing.Descriptor => GetMappingsDescriptor.Instance;
		class GetMappingsDescriptor : Descriptor
		{
			public static GetMappingsDescriptor Instance { get; } = new GetMappingsDescriptor();
			public GetMappingsDescriptor() : base("mappings", typeof(GetMappings)) { }
			public override IEnumerable<string> EnumerateProperties(object self)
				=> ((ICompletable)self).PossibleCompletions;
			public override void Get(ref Value self)
			{
				if (!((GetMappings)self.obj).RosGet(ref self))
					GetError(ref self);
			}
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
