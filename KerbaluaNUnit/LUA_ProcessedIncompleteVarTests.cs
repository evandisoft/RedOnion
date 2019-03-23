using NUnit.Framework;
using Kerbalua.Completion;
using System;
namespace KerbaluaNUnit {
	[TestFixture()]
	public class LUA_ProcessedIncompleteVarTests {
		public ProcessedIncompleteVar Parse(string source)
		{
			return LuaIntellisense.Parse(source);
		}

		[Test()]
		public void LUA_TestCase_1()
		{
			var parsed = Parse(
				@"
				function a() blah1[a](b).blah2(blah3.d
				"
				);

			Assert.AreEqual(2, parsed.Segments.Count);
			Assert.AreEqual("blah3", parsed.Segments[0].Name);
			Assert.AreEqual("d", parsed.Segments[1].Name);
		}

		[Test()]
		public void LUA_TestCase_2()
		{
			var parsed = Parse(
			@"
				function a() blah1[a](b).blah2().baa
			");

			Assert.AreEqual(3, parsed.Segments.Count);
			Assert.AreEqual(2, parsed.Segments[0].Parts.Count);
			Assert.IsInstanceOf(typeof(ArrayPart), parsed.Segments[0].Parts[0]);
			Assert.IsInstanceOf(typeof(CallPart), parsed.Segments[0].Parts[1]);
			Assert.AreEqual("baa", parsed.Segments[2].Name);
		}

		[Test()]
		public void LUA_TestCase_3()
		{
			var parsed = Parse("return asdf");

			Assert.AreEqual(1, parsed.Segments.Count);
			Assert.AreEqual("asdf", parsed.Segments[0].Name);

			//Assert.AreEqual("alpha", listener.IncompleteName);
		}

		[Test()]
		public void LUA_TestCase_4()
		{
			var parsed = LuaIntellisense.Parse("return asdf.qwer[1]().");

			Assert.AreEqual(3, parsed.Segments.Count);
			Assert.AreEqual("", parsed.Segments[2].Name);
		}

		[Test()]
		public void LUA_TestCase_5()
		{
			var parsed = Parse(
			@"
				function a() 
					return 3 
				end

				function b()
					return blah1[a](b).blah2().baa
			");

			Assert.AreEqual(3, parsed.Segments.Count);
			Assert.AreEqual("baa", parsed.Segments[2].Name);
			Assert.AreEqual("blah2", parsed.Segments[1].Name);
			Assert.AreEqual(2, parsed.Segments[0].Parts.Count);
		}

		[Test()]
		public void LUA_TestCase_6_IncompleteVarlist()
		{
			var parsed = Parse(
			@"
				a,blah1.b
			");

			Assert.AreEqual(2, parsed.Segments.Count);
			Assert.AreEqual("blah1", parsed.Segments[0].Name);
			Assert.AreEqual(0, parsed.Segments[0].Parts.Count);
		}

		[Test()]
		public void LUA_TestCase_7_IncompleteExplist()
		{
			var parsed = Parse(
			@"
				a,b=blah1.b,blah2.c
			");

			Assert.AreEqual(2, parsed.Segments.Count);
			Assert.AreEqual("blah2", parsed.Segments[0].Name);
			Assert.AreEqual(0, parsed.Segments[0].Parts.Count);
		}

		[Test()]
		public void LUA_TestCase_8_KeywordTest()
		{
			var parsed = Parse(
			@"
				a,b=blah1.b,blah2.while
			");

			Assert.AreEqual(2, parsed.Segments.Count);
			Assert.AreEqual("while", parsed.Segments[1].Name);
			Assert.AreEqual(0, parsed.Segments[0].Parts.Count);
		}
	}
}
