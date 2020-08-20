using MunSharp.Interpreter.DataStructs;
using MunSharp.Interpreter.Execution.VM;

namespace MunSharp.Interpreter.Execution
{
	interface ILoop
	{
		void CompileBreak(ByteCode bc);
		bool IsBoundary();
	}


	internal class LoopTracker
	{
		public FastStack<ILoop> Loops = new FastStack<ILoop>(16384);
	}
}
