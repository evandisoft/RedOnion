using System;
using MunOS;
using NUnit.Framework;
using RedOnion.KSP.ROS;
using Kerbalua.Scripting;

namespace RedOnion.KSP.Tests
{
	[TestFixture]
	public class MUN_OSTests : MunCore
	{
		class SimpleThread : MunThread
		{
			public SimpleThread(MunCore core, MunProcess process = null, MunPriority priority = MunPriority.Main, string name = null, bool start = true)
				: base(core, process, priority, name, start) { }

			int counter = 0;
			protected override MunStatus Execute(long tickLimit)
			{
				switch (Status)
				{
				case MunStatus.Incomplete:
					switch (counter++)
					{
					case 0:
						return MunStatus.Yielded;
					case 1:
						return MunStatus.Incomplete;
					default:
						return MunStatus.Finished;
					}
				case MunStatus.Yielded:
					return MunStatus.Sleeping;
				case MunStatus.Sleeping:
					return MunStatus.Incomplete;
				default:
					throw new NotImplementedException();
				}
			}

			public bool DoneCalled;
			protected override void OnDone()
			{
				DoneCalled = true;
				base.OnDone();
			}
		}
		[Test]
		public void MUN_Core01_SimpleThread()
		{
			var thread = new SimpleThread(this);
			Assert.AreEqual(MunStatus.Incomplete, thread.Status);
			Assert.AreEqual(1, main.Count);
			Assert.AreEqual(1, Count);

			FixedUpdate();
			Assert.AreEqual(MunStatus.Yielded, thread.Status);
			Assert.AreEqual(1, main.Count);

			FixedUpdate();
			Assert.AreEqual(MunStatus.Sleeping, thread.Status);
			Assert.AreEqual(1, sleeping.Count);

			FixedUpdate();
			Assert.AreEqual(MunStatus.Incomplete, thread.Status);
			Assert.AreEqual(1, main.Count);
			Assert.IsFalse(thread.DoneCalled);

			FixedUpdate();
			Assert.AreEqual(MunStatus.Finished, thread.Status);
			Assert.AreEqual(0, Count);
			Assert.IsTrue(thread.DoneCalled);
		}

		[Test]
		public void MUN_Core02_ProcessTerminate()
		{
			var pro = new MunProcess(this);
			new SimpleThread(this, pro);
			new SimpleThread(this, pro);
			Assert.AreEqual(2, Count);
			pro.Terminate();
			Assert.AreEqual(0, Count);
		}
	}
}
