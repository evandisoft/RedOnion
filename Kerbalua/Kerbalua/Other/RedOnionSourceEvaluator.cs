using System;
using System.Collections.Generic;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.ReflectedObjects;
using UnityEngine;

namespace Kerbalua.Other {
	public class RedOnionReplEvaluator:ReplEvaluator {
		Engine engine;

		public RedOnionReplEvaluator()
		{
			engine = new Engine(engine => new EngineRoot(engine));
		}

		class EngineRoot : Root {
			public EngineRoot(Engine engine) : base(engine) { }

			protected override void Fill()
			{
				AddType(typeof(Debug));
				AddType(typeof(Color));
				AddType(typeof(Rect));
				AddType(typeof(Vector2));
				AddType(typeof(Vector3));
				AddType(typeof(GUI));
				AddType(typeof(GUISkin));
				AddType(typeof(GUIStyle));
				AddType(typeof(GUIStyleState));
				AddType(typeof(GUIContent));
				AddType(typeof(GUIElement));
				AddType(typeof(GUILayer));
				AddType(typeof(GUILayout));
				AddType(typeof(GUIText));
				AddType(typeof(GUIUtility));
				AddType(typeof(FlightCtrlState));
				AddType(typeof(FlightGlobals));
				AddType(typeof(Vessel));
			}
		}

		public override string Evaluate(string source)
		{
			string output = "";
			try {
				//Debug.Log("Running statement with Execution Countdown at " + engine.ExecutionCountdown);
				engine.ExecutionCountdown = 10000;
				engine.Execute(source);
				Value result = engine.Result;
				output = "\n";
				output +=result.ToString();
			}
			catch(Exception e) {
				Debug.Log(e);
			}

			return output;
		}


		public override List<string> GetCompletions(string source, int cursorPos)
		{
			throw new NotImplementedException();
		}

		public override string GetPartialCompletion(string source, int cursorPos)
		{
			throw new NotImplementedException();
		}

		public override void ResetEngine()
		{
			engine.Reset();
		}
	}
}
