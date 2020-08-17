using System;
using MunOS;
using NUnit.Framework;
using RedOnion.Debugging;
using RedOnion.KSP.API;
using RedOnion.KSP.ROS;
using RedOnion.ROS;

namespace RedOnion.KSP.Tests
{
	[TestFixture]
	public class API_MiscTests : ApiTestsBase
	{
		[Test]
		public void API_Misc01_Subscribe()
		{
			Lines(
				"global.counter = 0",
				"var sub = science.situationChanged def",
				"  counter++",
				"return null");
			foreach (var action in Science.situationChanged)
				action();
			MunCore.Default.FixedUpdate();
			Test(1, "counter");
			Test("sub = null");
			GC.Collect();
			GC.WaitForPendingFinalizers();
			UI.CollectorQueue.Collect();
			Assert.AreEqual(0, Science.situationChanged.count);
		}
	}
}
