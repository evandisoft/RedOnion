using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunOS.Repl
{
	public abstract class ScriptManager
	{
		public abstract string Extension { get; }

		public abstract MunProcess CreateProcess(MunCore core);
		public abstract MunThread CreateThread(MunProcess process, string source, string path, string[] includes = null);
	}
}
