using System;
using System.Collections.Generic;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS.Objects
{
	public class Globals : UserObject
	{
		public Globals() : base("Globals")
		{
			Add("print", new Value(Print.Instance));
			Add("object", new Value(new UserObject()));
			Add("list", new Value(typeof(List<Value>)));
			Lock();
		}
	}
}
