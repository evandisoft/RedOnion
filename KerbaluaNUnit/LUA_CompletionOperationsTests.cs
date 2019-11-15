using NUnit.Framework;
using Kerbalua.Completion;
using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using Kerbalua.Other;
using RedOnion.KSP.MoonSharp;
using Kerbalua.MoonSharp;
//using UnityEngine;
using System.Reflection;
using RedOnion.KSP.API;
using static KerbaluaNUnit.TestingUtil;

namespace KerbaluaNUnit {
	[TestFixture()]
	public class LUA_CompletionOperationsTests
	{


		//public CompletionObject GetCompletionObject(Table globals, string source)
		//{
		//	var processed = LuaIntellisense.Parse(source);

		//	return new CompletionObject(globals, processed.Segments);
		//}

		Script script = new Script(CoreModules.Preset_Complete);
		public LUA_CompletionOperationsTests()
		{
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
		}

		public class Adf
		{
			public Adf asdf()
			{
				return this;
			}
			public string asd = "badsf";
			public delegate Adf blah2(int i);
			public blah2 blah;
			public int testy = 2;
			public List<blah2> adfs;
			public static int asdfg = 4;
		}

		[Test()]
		public void LUA_TestCase_Static()
		{
			var operations = new CompletionOperations(
				LuaIntellisense.Parse(
				@"Adf.asdfg."
					).Segments);
			var globals = script.Globals;
			globals["Adf"]=UserData.CreateStatic(typeof(Adf));
			object currentObject = script.Globals;
			IList<string> completions=new List<string>();
			//Console.WriteLine(operations);
			completions = OperationsProcessor.StaticGetPossibleCompletions(currentObject);

			Assert.IsTrue(OperationsProcessor.TryProcessOperation(currentObject, operations, out currentObject));
			//Console.WriteLine(operations);

			completions = OperationsProcessor.StaticGetPossibleCompletions(currentObject);

			//foreach (var c in OperationsProcessor.StaticGetPossibleCompletions(currentObject,operations))
			//{
			//	Console.WriteLine(c);
			//}
			Assert.IsTrue(OperationsProcessor.TryProcessOperation(currentObject, operations, out currentObject));
			//Console.WriteLine(operations);
			completions = OperationsProcessor.StaticGetPossibleCompletions(currentObject);
			//PrintAll(completions);
			Assert.AreEqual(11, completions.Count);
			//Console.WriteLine(operations);
		}

		[Test()]
		public void LUA_TestCase_Instance()
		{
			var operations = new CompletionOperations(
				LuaIntellisense.Parse(
				@"Adf.asd."
					).Segments);
			var globals = script.Globals;
			globals["Adf"] = new Adf();
			object currentObject = script.Globals;
			IList<string> completions = new List<string>();
			//Console.WriteLine(operations);
			Assert.IsTrue(OperationsProcessor.TryProcessOperation(currentObject, operations, out currentObject));
			//Console.WriteLine(operations);
			completions = OperationsProcessor.StaticGetPossibleCompletions(currentObject);
			//foreach (var c in OperationsProcessor.StaticGetPossibleCompletions(currentObject,operations))
			//{
			//	Console.WriteLine(c);
			//}
			Assert.IsTrue(OperationsProcessor.TryProcessOperation(currentObject, operations, out currentObject));
			//Console.WriteLine(operations);
			completions = OperationsProcessor.StaticGetPossibleCompletions(currentObject);
			//PrintAll(completions);

			//Console.WriteLine(operations);
		}



		//[Test()]
		//public void LUA_TestCase_2()
		//{
		//	script = new Script(CoreModules.Preset_Complete);
		//	script.Globals["ADF"] = new Adf();

		//	var completion = GetCompletionObject(script.Globals,
		//		@"ADF."
		//		);
		//	var completions = completion.GetCurrentCompletions();
		//	Assert.AreEqual(3, completions.Count);
		//	Assert.True(completion.ProcessNextSegment());
		//	completions = completion.GetCurrentCompletions();
		//	foreach (var c in completions)
		//	{
		//		//Console.WriteLine(c);
		//	}
		//	Assert.AreEqual(10, completions.Count);
		//}

		//[Test()]
		//public void LUA_TestCase_3()
		//{
		//	script = new Script(CoreModules.Preset_Complete);
		//	script.Globals["ADF"] = new Adf();

		//	var completion = GetCompletionObject(script.Globals,
		//		@"ADF."
		//		);
		//	var completions = completion.GetCurrentCompletions();
		//	foreach (var c in completions)
		//	{
		//		//Console.WriteLine(c);
		//	}
		//	Assert.AreEqual(3, completions.Count);
		//	Assert.True(completion.ProcessNextSegment());
		//	completions = completion.GetCurrentCompletions();
		//	foreach (var c in completions)
		//	{
		//		//Console.WriteLine(c);
		//	}
		//	Assert.AreEqual(10, completions.Count);
		//}


		//[Test()]
		//public void LUA_TestCase_4()
		//{
		//	script = new Script(CoreModules.Preset_Complete);
		//	script.Globals["ADF"] = new Adf();

		//	var completion = GetCompletionObject(script.Globals,
		//		@"ADF."
		//		);
		//	var completions = completion.GetCurrentCompletions();
		//	foreach (var c in completions)
		//	{
		//		//Console.WriteLine(c);
		//	}
		//	Assert.AreEqual(3, completions.Count);
		//	Assert.True(completion.ProcessNextSegment());
		//	completions = completion.GetCurrentCompletions();
		//	foreach (var c in completions)
		//	{
		//		//Console.WriteLine(c);
		//	}
		//	Assert.AreEqual(10, completions.Count);
		//}
		//		[Test()]
		//		public void LUA_TestCase_3_Interop()
		//		{
		//			script = new KerbaluaScript();
		//			//script.Globals["ship"] = new Adf();

		//			var completion = GetCompletionObject(script.Globals,
		//@"
		//	a=ship.
		//"
		//				);
		//			var completions = completion.GetCurrentCompletions();
		//			foreach (var c in completions)
		//			{
		//				Console.WriteLine(c);
		//			}
		//			Assert.AreEqual(1, completions.Count);
		//			//Assert.Throws<LuaIntellisenseException>(() => completion.ProcessNextSegment());
		//		}

		//		[Test()]
		//		public void LUA_TestCase_4_Interop_2()
		//		{
		//			script = new KerbaluaScript();
		//			//script.Globals["ship"] = new Adf();

		//			// I think this is not quite right, but I'm done for now.
		//			var completion = GetCompletionObject(script.Globals,
		//@"
		//	a=Vector.abs
		//"
		//				);
		//			var completions = completion.GetCurrentCompletions();
		//			foreach (var c in completions)
		//			{
		//				Console.WriteLine(c);
		//			}
		//			Assert.AreEqual(1, completions.Count);
		//			completion.ProcessNextSegment();
		//			completions = completion.GetCurrentCompletions();
		//			foreach (var c in completions)
		//			{
		//				Console.WriteLine(c);
		//			}
		//			Assert.AreEqual(1, completions.Count);
		//		}

		//		[Test()]
		//		public void LUA_TestCase_5_Static()
		//		{
		//			script = new KerbaluaScript();
		//			//script.Globals["ship"] = new Adf();

		//			// I think this is not quite right, but I'm done for now.
		//			var completion = GetCompletionObject(script.Globals,
		//@"
		//	a=AssemblyStatic.
		//"
		//		);
		//	var completions = completion.GetCurrentCompletions();
		//	foreach (var c in completions)
		//	{
		//		Console.WriteLine(c);
		//	}
		//	Assert.AreEqual(1, completions.Count);
		//	completion.ProcessNextSegment();
		//	completions = completion.GetCurrentCompletions();
		//	foreach (var c in completions)
		//	{
		//		Console.WriteLine(c);
		//	}
		//	Assembly a = Assembly.GetAssembly(typeof(System.Linq.Enumerable));
		//	Console.WriteLine(a);
		//	Assert.AreEqual(1, completions.Count);
		//}


	}
}
