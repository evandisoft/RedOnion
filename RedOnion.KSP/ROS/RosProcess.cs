using MunOS;
using MunOS.Repl;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOnion.KSP.ROS
{
	public class RosProcess : MunProcess
	{
		public RosProcessor Processor { get; private set; }
		//TODO: protected internal RosThread ReplThread { get; set; }

		public RosProcess(MunCore core) : this(core, false) { }
		public RosProcess(RosManager manager) : this(manager.Core)
			=> ScriptManager = manager;
		public RosProcess(MunCore core, bool lateBind) : base(core)
		{
			if (!lateBind)
				SetProcessor(new RosProcessor(this));
		}
		public void SetProcessor(RosProcessor processor)
		{
			if (Processor != null)
				throw new InvalidOperationException("ROS Processor already set");
			Processor = processor;
			if (OutputBuffer != null)
			{
				Processor.Print += OutputBuffer.AddOutput;
				Processor.PrintError += OutputBuffer.AddError;
			}
		}

		protected override void OnSetOutputBuffer(OutputBuffer value, OutputBuffer prev)
		{
			if (prev != null)
			{
				Processor.Print -= prev.AddOutput;
				Processor.PrintError -= prev.AddError;
			}
			if (value != null)
			{
				Processor.Print += OutputBuffer.AddOutput;
				Processor.PrintError += OutputBuffer.AddError;
			}
		}
	}
}
