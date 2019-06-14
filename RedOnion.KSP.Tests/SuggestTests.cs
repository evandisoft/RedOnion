using System;
using System.Collections.Generic;
using NUnit.Framework;
using RedOnion.KSP.API;
using RedOnion.KSP.ROS;
using RedOnion.ROS;

namespace RedOnion.KSP.Tests
{
	public class ROS_Suggest : RosSuggest
	{
		public ROS_Suggest() : base(new RosCore()) { }

		protected void AssertContains(string what, IList<string> list)
		{
			foreach (var e in list)
			{
				if (e == what)
					return;
			}
			Assert.Fail("List does not contain '{0}'", what);
		}

		[Test]
		public void ROS_Suggest01_Empty()
		{
			var list = GetCompletions("", 0, out var at, out var to);
			AssertContains("print", list);
		}
	}
}
