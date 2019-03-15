using NUnit.Framework;
using Kerbalua.Completion;
using System;
namespace KerbaluaNUnit {
	[TestFixture()]
	public class LuaIntellisenseParserTests {
		[Test()]
		public void TestCase_1()
		{
			var listener = LuaIntellisenseParser.Parse("blah");


			Assert.AreEqual("blah", listener.PartialCompletion);
			Assert.AreEqual("", listener.StartSymbol);
			Assert.AreEqual(false, listener.HasStartSymbol);
		}

		[Test()]
		public void TestCase_2()
		{
			var listener = LuaIntellisenseParser.Parse("blah.");


			Assert.AreEqual("", listener.PartialCompletion);
			Assert.AreEqual("blah", listener.StartSymbol);
			Assert.AreEqual(true, listener.HasStartSymbol);
		}

		[Test()]
		public void TestCase_3()
		{
			var listener = LuaIntellisenseParser.Parse("asdf.qwer");


			Assert.AreEqual("qwer", listener.PartialCompletion);
			Assert.AreEqual("asdf", listener.StartSymbol);
			Assert.AreEqual(true, listener.HasStartSymbol);
		}

		[Test()]
		public void TestCase_4()
		{
			var listener = LuaIntellisenseParser.Parse("blah.asdf.");


			Assert.AreEqual("", listener.PartialCompletion);
			Assert.AreEqual("blah", listener.StartSymbol);
			Assert.AreEqual(true, listener.HasStartSymbol);
		}
		[Test()]
		public void TestCase_5()
		{
			var listener = LuaIntellisenseParser.Parse("blah.asdf[l;jalff9d8*43].tyui");


			Assert.AreEqual("tyui", listener.PartialCompletion);
			Assert.AreEqual("blah", listener.StartSymbol);
			Assert.AreEqual(true, listener.HasStartSymbol);
		}
		[Test()]
		public void TestCase_6()
		{
			var listener = LuaIntellisenseParser.Parse("lak;sdf\"blah.asdf.");


			Assert.AreEqual("", listener.PartialCompletion);
			Assert.AreEqual("blah", listener.StartSymbol);
			Assert.AreEqual(true, listener.HasStartSymbol);
		}
		[Test()]
		public void TestCase_7()
		{
			var listener = LuaIntellisenseParser.Parse("blah.tyui[](1..354.5)[].asdf.k5");

			Assert.AreEqual("k5", listener.PartialCompletion);
			Assert.AreEqual("blah.asdf.", listener.ToString());
			Assert.AreEqual("blah", listener.StartSymbol);
			Assert.AreEqual(true, listener.HasStartSymbol);
		}
		[Test()]
		public void TestCase_8()
		{
			var listener = LuaIntellisenseParser.Parse("blah.asdf.");


			Assert.AreEqual("blah.asdf.", listener.ToString());
			Assert.AreEqual(true, listener.HasStartSymbol);
		}
	}
}
