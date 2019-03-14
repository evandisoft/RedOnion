using System;
using System.Collections.Generic;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.ReflectedObjects;
using UnityEngine;
using Kerbalua.AutoPilot;
using KSP.UI.Screens;

namespace Kerbalua.Other {
	public class RedOnionReplEvaluator:ReplEvaluator {
		Engine engine;

		public RedOnionReplEvaluator()
		{
			//temporarily commenting this out
			//engine = new Engine(engine => new EngineRoot(engine));
			engine = new Engine();
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
				AddType(typeof(StageManager));
				AddType(typeof(AutoPilotAPI));
			}
		}

		protected override bool ProtectedEvaluate(string source,out string output)
		{
			output = "";
			try {
				//Debug.Log("Running statement with Execution Countdown at " + engine.ExecutionCountdown);
				engine.ExecutionCountdown = 10000;
				engine.Execute(source);
				Value result = engine.Result;
				output +=result.ToString();
			}
			catch(Exception e) {
				Debug.Log(e);
			}

			// TODO: This needs to be replaced when engine can fail to complete in one update
			bool isComplete = true;

			return isComplete;
		}

		/// <summary>
		/// TODO: NOT CURRENTLY IMPLEMENTED
		/// </summary>
		/// <returns>The completions.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		public override IList<string> GetCompletions(string source, int cursorPos,out int replaceStart,out int replaceEnd)
		{
			List<string> NOT_IMPLEMENTED_COMPLETIONS = new List<string>();
			NOT_IMPLEMENTED_COMPLETIONS.Add("RedOnion");
			NOT_IMPLEMENTED_COMPLETIONS.Add("intellisense");
			NOT_IMPLEMENTED_COMPLETIONS.Add("is");
			NOT_IMPLEMENTED_COMPLETIONS.Add("not");
			NOT_IMPLEMENTED_COMPLETIONS.Add("currently");
			NOT_IMPLEMENTED_COMPLETIONS.Add("implemented");
			for (int i = 0;i < 100;i++) {
				NOT_IMPLEMENTED_COMPLETIONS.Add("test-string #" + i);
			}
			replaceStart = replaceEnd = cursorPos;
			return NOT_IMPLEMENTED_COMPLETIONS;
		}


		public override void ResetEngine()
		{
			engine.Reset();
		}

		public override void Terminate()
		{
			throw new NotImplementedException();
		}
	}
}
