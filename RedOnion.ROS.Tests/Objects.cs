using System;
using System.Collections;
using NUnit.Framework;
using RedOnion.Attributes;
using RedOnion.ROS.Objects;

namespace RedOnion.ROS.Tests
{
	[TestFixture]
	public class ROS_Objects : CoreTests
	{
		[Test]
		public void ROS_Obj01_EnumCtor()
		{
			Globals = new Globals();
			Globals.Add("ints", new int[] { 1, 2, 3 });
			Test("var a = list ints");
			Test(2, "a[1]");
		}
	}
}
