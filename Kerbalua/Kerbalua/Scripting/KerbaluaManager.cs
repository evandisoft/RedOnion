using MunOS;
using MunOS.Repl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kerbalua.Scripting
{
	public class KerbaluaManager : ScriptManager
	{
		public override string Extension => ".lua";

		public override MunProcess CreateProcess(MunCore core)
			=> new KerbaluaProcess(core);
		public override MunThread CreateThread(MunProcess process, string source, string path, string[] includes = null)
			=> new KerbaluaThread((KerbaluaProcess)process, MunPriority.Main, source, path, includes);
	}
}
