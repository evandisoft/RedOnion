//using System;
//using System.Collections.Generic;
//using Kerbalua.Scripting;
//using Kerbnlua;
//using LiveRepl.Execution;

//namespace LiveRepl.Execution
//{
//	public class KerbnluaReplEvaluator:ReplEvaluator
//	{
//		KerbnluaScript script;
//		public KerbnluaReplEvaluator()
//		{
//			script=new KerbnluaScript();

//		}

//		public override string Extension => ".nlua";

//		public override bool Evaluate(out string result)
//		{
//			return script.Evaluate(out result);
//		}

//		public override void FixedUpdate()
//		{
//			//throw new NotImplementedException();
//		}

//		public override IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
//		{
//			replaceEnd=replaceStart=cursorPos;
//			return new List<string>(); //throw new NotImplementedException();
//		}

//		public override IList<string> GetDisplayableCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
//		{
//			replaceEnd=replaceStart=cursorPos;
//			return new List<string>();
//		}

//		public override string GetImportString(string scriptname)
//		{
//			return "";//throw new NotImplementedException();
//		}

//		public override void Terminate()
//		{
//			script.Terminate();//throw new NotImplementedException();
//		}

//		protected override void ProtectedSetSource(string source, string path)
//		{
//			script.SetSource(source);
//		}
//	}
//}
