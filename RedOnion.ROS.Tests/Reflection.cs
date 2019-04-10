using System;
using System.Collections;
using NUnit.Framework;
using RedOnion.ROS.Objects;

namespace RedOnion.ROS.Tests
{
	[TestFixture]
	public class ROS_Reflection : CoreTests
	{
		public static class StaticTest
		{
			public static int Integer { get; set; }
		}
		[Test]
		public void ROS_Refl01_Properties()
		{
			Globals = new Globals();
			Globals.Add("test", typeof(StaticTest));
			Test(1, "test.integer = 1");
			Test(1, "test.integer");
		}
	}
}
