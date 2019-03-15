using NUnit.Framework;
using Kerbalua.Completion;
using System;
namespace KerbaluaNUnit {
	[TestFixture()]
	public class IncompleteLuaIntellisenseTests {
		public IncompleteLuaListenerImpl TestParse(string source)
		{
			return IncompleteLuaIntellisense.Parse(source);
		}

		[Test()]
		public void TestCase_1()
		{
			var listener = TestParse(
				@"
				function a() blah1[a](b).blah2(blah3.d
				"
				);

			Assert.AreEqual("d", listener.IncompleteName);
		}

		[Test()]
		public void TestCase_2()
		{
			var listener = TestParse(
			@"
				function a() blah1[a](b).blah2(blah3
			");

			Assert.AreEqual("blah3", listener.IncompleteName);
		}

		[Test()]
		public void TestCase_3()
		{
			var listener = TestParse("return asdf.qwer[1]().alpha");

			Assert.AreEqual("alpha", listener.IncompleteName);
		}

		[Test()]
		public void TestCase_4()
		{
			var listener = IncompleteLuaIntellisense.Parse("return asdf.qwer[1]().");

			Assert.AreEqual("", listener.IncompleteName);
		}

		//[Test()]
		//public void TestCase_5()
		//{
		//	var listener = IncompleteLuaIntellisense.Parse("blah.asdf[l;jalff9d8*43].tyui");


		//	Assert.AreEqual("tyui", listener.PartialCompletion);
		//	Assert.AreEqual("blah", listener.StartSymbol);
		//	Assert.AreEqual(true, listener.HasStartSymbol);
		//}
		//[Test()]
		//public void TestCase_6()
		//{
		//	var listener = IncompleteLuaIntellisense.Parse("lak;sdf\"blah.asdf.");


		//	Assert.AreEqual("", listener.PartialCompletion);
		//	Assert.AreEqual("blah", listener.StartSymbol);
		//	Assert.AreEqual(true, listener.HasStartSymbol);
		//}
		//[Test()]
		//public void TestCase_7()
		//{
		//	var listener = IncompleteLuaIntellisense.Parse("blah.tyui[](1..354.5)[].asdf.k5");

		//	Assert.AreEqual("k5", listener.PartialCompletion);
		//	Assert.AreEqual("blah.asdf.", listener.ToString());
		//	Assert.AreEqual("blah", listener.StartSymbol);
		//	Assert.AreEqual(true, listener.HasStartSymbol);
		//}
		//[Test()]
		//public void TestCase_8()
		//{
		//	var listener = IncompleteLuaIntellisense.Parse("blah.asdf.");


		//	Assert.AreEqual("blah.asdf.", listener.ToString());
		//	Assert.AreEqual(true, listener.HasStartSymbol);
		//}
	}
}
