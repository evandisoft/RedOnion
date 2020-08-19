using System;
using MunSharp.Interpreter;
using NUnit.Framework;

namespace MunSharp.Interpreter.Tests
{
	[TestFixture()]
	public class MunSharpTests
	{
		string source="";
		Script script=null;
		Coroutine coroutine=null;
		DynValue result=null;

		string innerCoroutineRet=@"
function l()
	return 1 
end
c=coroutine.create(l)
return coroutine.resume(c)
		";

		string innerCoroutineLoop=@"
function l()
	while true do end
end
c=coroutine.create(l)
return coroutine.resume(c)
		";

		string innerCoroutineFiniteLoop=@"
function l()
	for i=1,1000 do a=1+1 end
	return 1
end
c=coroutine.create(l)
return coroutine.resume(c)
		";

		[SetUp]
		public void Setup()
		{
			script=new Script();
		}

		public Coroutine CreateCoroutine(string source)
		{
			DynValue mainFunction = script.DoString(@"return function ()"+source+"\n end");
			return script.CreateCoroutine(mainFunction).Coroutine;
		}

		[Test]
		public void MunSharp_01_ForcedYield()
		{
			source="return 1";

			// normal - no forced yield
			coroutine=CreateCoroutine(source);
			result=coroutine.Resume();
			Assert.IsFalse(script.ForceYield);
			Assert.AreEqual(CoroutineState.Dead, coroutine.State);
			Assert.AreEqual(DataType.Number, result.Type);

			// a forced yield
			coroutine=CreateCoroutine(source);
			script.ForceYield=true;
			result=coroutine.Resume();
			Assert.IsFalse(script.ForceYield);
			Assert.AreEqual(CoroutineState.ForceSuspended, coroutine.State);

			// continuing after the forced yield
			result=coroutine.Resume();
			Assert.AreEqual(CoroutineState.Dead, coroutine.State);
			Assert.AreEqual(DataType.Number, result.Type);
			Assert.AreEqual(1, result.Number);
		}

		[Test]
		public void MunSharp_02_TimedInterrupt()
		{
			source = "while true do end";
			coroutine=CreateCoroutine(source);

			// yield timed for 1000 ticks
			script.SetTimedInterrupt(1000);
			result=coroutine.Resume();
			Assert.AreEqual(CoroutineState.ForceSuspended, coroutine.State);
			Assert.IsFalse(script.TimedInterruptActive);

			// continuing on for another yield timed for 0 ticks
			script.SetTimedInterrupt(0);
			result=coroutine.Resume();
			Assert.AreEqual(CoroutineState.ForceSuspended, coroutine.State);
			Assert.IsFalse(script.TimedInterruptActive);
		}

		[Test]
		public void MunSharp_03_AutoYieldCounter()
		{
			source = "while true do end";
			coroutine=CreateCoroutine(source);
			coroutine.AutoYieldCounter=100;

			// yield after 100 instructions
			result=coroutine.Resume();
			Assert.AreEqual(CoroutineState.ForceSuspended, coroutine.State);

			// continuing on for another yield
			result=coroutine.Resume();
			Assert.AreEqual(CoroutineState.ForceSuspended, coroutine.State);
		}

		[Test] //EVANTODO: this test doesn't actually test anything.
		public void MunSharp_04_InnerCoroutine_ForcedYield()
		{
			source=innerCoroutineRet;

			// normal - no forced yield
			coroutine=CreateCoroutine(source);
			result=coroutine.Resume();
			Assert.IsFalse(script.ForceYield);
			Assert.AreEqual(CoroutineState.Dead, coroutine.State);
			Assert.AreEqual(DataType.Tuple, result.Type);
			Assert.AreEqual(DataType.Number, result.Tuple[1].Type);
			Assert.AreEqual(1, result.Tuple[1].Number);

			// a forced yield
			coroutine=CreateCoroutine(source);
			script.ForceYield=true;
			result=coroutine.Resume();
			Assert.IsFalse(script.ForceYield);
			Assert.AreEqual(CoroutineState.ForceSuspended, coroutine.State);

			// continuing after the forced yield
			result=coroutine.Resume();
			Assert.AreEqual(CoroutineState.Dead, coroutine.State);
			Assert.AreEqual(DataType.Tuple, result.Type);
			Assert.AreEqual(DataType.Number, result.Tuple[1].Type);
			Assert.AreEqual(1, result.Tuple[1].Number);
		}

		[Test] //EVANTODO: this test doesn't actually test anything.
		public void MunSharp_05_InnerCoroutine_TimedInterrupt()
		{
			source = innerCoroutineLoop;
			coroutine=CreateCoroutine(source);

			// yield timed for 1000 ticks
			script.SetTimedInterrupt(1000);
			Assert.IsTrue(script.TimedInterruptActive);
			result=coroutine.Resume();
			Assert.AreEqual(CoroutineState.ForceSuspended, coroutine.State);
			Assert.IsFalse(script.TimedInterruptActive);

			// continuing on for another yield timed for 0 ticks
			script.SetTimedInterrupt(0);
			result=coroutine.Resume();
			Assert.AreEqual(CoroutineState.ForceSuspended, coroutine.State);
			Assert.IsFalse(script.TimedInterruptActive);
		}

		[Test]
		public void MunSharp_06_InnerFiniteLoop_AutoYieldCounter()
		{
			source = innerCoroutineFiniteLoop;
			coroutine = CreateCoroutine(source);
			coroutine.AutoYieldCounter=100;

			// The AutoYieldCounter of the top coroutine does not effect
			// the inner coroutine. So the inner coroutine using more than
			// 100 instructions does not trigger an AutoYieldCounter
			// interrupt. Instead of being interrupted, the inner 
			// coroutine completes and returns normally.
			result=coroutine.Resume();
			Assert.AreEqual(CoroutineState.Dead, coroutine.State);
			Assert.AreEqual(DataType.Tuple, result.Type);
			Assert.AreEqual(DataType.Number, result.Tuple[1].Type);
			Assert.AreEqual(1, result.Tuple[1].Number);
		}
	}
}
