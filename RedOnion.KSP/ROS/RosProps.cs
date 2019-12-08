using RedOnion.ROS;
using RedOnion.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RedOnion.KSP.ROS
{
	struct RosProps
	{
		[DebuggerDisplay("{name} = {value}")]
		public struct Prop
		{
			public string name;
			public Value value;
			public override string ToString()
				=> string.Format(Value.Culture, "{0} = {1}", name, value.ToString());
		}
		public ListCore<Prop> prop;
		public Dictionary<string, int> dict;
	}
}
