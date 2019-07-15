using Kerbalua.Parsing;
using NUnit.Framework;
using System;
namespace KerbaluaNUnit
{
	[TestFixture()]
	public class LUA_ImplicitReturnTests
	{
		[Test()]
		public void TestCase_1()
		{
			var q=IncompleteLuaParsing.IsImplicitReturn(
			@"
function a() 
	print(1)
	return 1
end
			");
			Assert.IsFalse(q);
		}

		[Test()]
		public void TestCase_2()
		{
			var q = IncompleteLuaParsing.IsImplicitReturn(
			@"
a=2
			");
			Assert.IsFalse(q);
		}

		[Test()]
		public void TestCase_3()
		{
			var q = IncompleteLuaParsing.IsImplicitReturn(
			@"
2
			");
			Assert.IsTrue(q);
		}
	}
}
