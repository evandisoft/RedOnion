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

		/// <summary>
		/// TODO: NOT CURRENTLY IMPLEMENTED
		/// </summary>
		/// <returns>The completions.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		public override List<string> GetCompletions(string source, int cursorPos)
		{
			List<string> NOT_IMPLEMENTED_COMPLETIONS = new List<string>();
			NOT_IMPLEMENTED_COMPLETIONS.Add("RedOnion");
			NOT_IMPLEMENTED_COMPLETIONS.Add("intellisense");
			NOT_IMPLEMENTED_COMPLETIONS.Add("is");
			NOT_IMPLEMENTED_COMPLETIONS.Add("not");
			NOT_IMPLEMENTED_COMPLETIONS.Add("currently");
			NOT_IMPLEMENTED_COMPLETIONS.Add("implemented");
			return NOT_IMPLEMENTED_COMPLETIONS;
		}

		/// <summary>
		/// TODO: NOT CURRENTLY IMPLEMENTED
		/// </summary>
		/// <returns>The partial completion.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		public override string GetPartialCompletion(string source, int cursorPos)
		{
			return "";
		}

		public override void ResetEngine()
		{
			engine.Reset();
		}
	}
}
