using System;
using System.Collections.Generic;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS.Objects
{
	public class Globals : UserObject
	{
		public Globals() : base("Globals")
		{
			Add("object", new Value(new UserObject()));
			Lock();
		}
	}
}
