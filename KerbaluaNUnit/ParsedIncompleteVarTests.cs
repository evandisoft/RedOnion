using NUnit.Framework;
using Kerbalua.Completion;
using System;
namespace KerbaluaNUnit {
	[TestFixture()]
	public class ParsedIncompleteVarTests {
		public ParsedIncompleteVar Parse(string source)
		{
			return LuaIntellisense.Parse(source);
		}

		[Test()]
		public void TestCase_1()
		{
			var parsed = Parse(
				@"
				function a() blah1[a](b).blah2(blah3.d
				"
				);

			Assert.AreEqual(2, parsed.Segments.Count);
			Assert.AreEqual("blah3", parsed.Segments[0].Name);
		}

		[Test()]
		public void TestCase_2()
		{
			var parsed = Parse(
			@"
				function a() blah1[a](b).blah2().baa
			");

			Assert.AreEqual(3, parsed.Segments.Count);
			Assert.AreEqual("baa", parsed.Segments[2].Name);
			Assert.AreEqual(2, parsed.Segments[0].Parts.Count);
		}

		[Test()]
		public void TestCase_3()
		{
			var parsed = Parse("return asdf");

			Assert.AreEqual(1, parsed.Segments.Count);
			Assert.AreEqual("asdf", parsed.Segments[0].Name);

			//Assert.AreEqual("alpha", listener.IncompleteName);
		}

		[Test()]
		public void TestCase_4()
		{
			var parsed = LuaIntellisense.Parse("return asdf.qwer[1]().");

			Assert.AreEqual(3, parsed.Segments.Count);
			Assert.AreEqual("", parsed.Segments[2].Name);
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
