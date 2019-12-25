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
		public ROS_Suggest() : base(new ApiTestsBase()) { }

		protected void AssertContains(string what, IList<string> list)
		{
			foreach (var e in list)
			{
				if (what.Equals(e, StringComparison.OrdinalIgnoreCase))
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

		[Test]
		public void ROS_Suggest02_Property()
		{
			Processor.Execute("var a = [1,2]");
			var list = GetCompletions("a.", 2, out var at, out var to);
			AssertContains("length", list);

			list = GetCompletions("vector.", 7, out at, out to);
			AssertContains("zero", list);
		}
	}
}
