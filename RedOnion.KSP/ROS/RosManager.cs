using MunOS;
using MunOS.Repl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedOnion.KSP.ROS
{
	public class RosManager : ScriptManager
	{
		public override string Extension => throw new NotImplementedException();

		public override MunProcess CreateProcess(MunCore core)
			=> new RosProcess(core);
		public override MunThread CreateThread(MunProcess process, string source, string path, string[] includes = null)
			=> new RosThread((RosProcess)process, MunPriority.Main, source, path, includes);
	}
}
