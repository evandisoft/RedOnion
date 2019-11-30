using System;
using System.Collections.Generic;

namespace MunOS.Repl
{
	public interface IReplEngine
	{
		string EngineName();
		void SetReplProcess(ReplProcess replProcess);
		bool IsWaitingForInput();
		bool IsIdle();
		void Interrupt();
		void ReceiveInput(string str);
		void EvaluateSource(string source);
		void FixedUpdate(float timeAllotted);
		void Reset();
		IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd);
		IList<string> GetDisplayableCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd);
	}
}
