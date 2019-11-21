using System;
using System.Collections.Generic;
using Kerbalua.Other;

namespace Kerbnlua
{
	public class KerbnluaReplEvaluator:ReplEvaluator
	{
		KerbnluaScript script;
		public KerbnluaReplEvaluator()
		{
			script=new KerbnluaScript();
			script.state["print"]=new Action<object>((obj) => PrintAction.Invoke(obj.ToString()));
			script.state["os"]=null;
			script.state["debug"]=null;
			script.state["io"]=null;
		}

		public override string Extension => ".nlua";

		public override bool Evaluate(out string result)
		{
			var status=script.Evaluate(out object[] retvals);
			result="";
			foreach (var retval in retvals)
			{
				result+=", "+retval;
			}
			if (result!="")
			{
				result=result.Substring(2);
			}

			if (status==KeraLua.LuaStatus.OK)
			{
				return true;
			}
			if (status!=KeraLua.LuaStatus.Yield)
			{
				PrintErrorAction?.Invoke(result);
				result="";
				return true;
			}

			return false;
		}

		public override void FixedUpdate()
		{
			//throw new NotImplementedException();
		}

		public override IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
		{
			replaceEnd=replaceStart=cursorPos;
			return new List<string>(); //throw new NotImplementedException();
		}

		public override IList<string> GetDisplayableCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
		{
			replaceEnd=replaceStart=cursorPos;
			return new List<string>();
		}

		public override string GetImportString(string scriptname)
		{
			return "";//throw new NotImplementedException();
		}

		public override void Terminate()
		{
			script.Terminate();//throw new NotImplementedException();
		}

		protected override void ProtectedSetSource(string source, string path)
		{
			script.SetSource(source);
		}
	}
}
