using Kerbalua.Parsing;
using NUnit.Framework;
using System;
namespace KerbaluaNUnit
{
	[TestFixture()]
	public class MLUA_ImplicitReturnTests
	{
		[Test()]
		public void MLUA_TestCase_1()
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
		public void MLUA_TestCase_2()
		{
			var q = IncompleteLuaParsing.IsImplicitReturn(
			@"
a=2
			");
			Assert.IsFalse(q);
		}

		[Test()]
		public void MLUA_TestCase_3()
		{
			var q = IncompleteLuaParsing.IsImplicitReturn(
			@"
2
			");
			Assert.IsTrue(q);
		}
	}
}
