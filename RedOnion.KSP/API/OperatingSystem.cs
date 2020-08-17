using MoonSharp.Interpreter;
using MunOS;
using RedOnion.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.API
{
	public static class OperatingSystemInterface
	{
		internal static readonly Dictionary<string, Func<string, object[], MunProcess>>
			processCreators = new Dictionary<string, Func<string, object[], MunProcess>>(
				StringComparer.OrdinalIgnoreCase);
		/// <summary>
		/// Register process creator for specific file extension.
		/// </summary>
		/// <param name="fileExtension">The extension like ".ros" or ".lua".</param>
		/// <param name="creator">Function accepting path and arguments (which can be null).</param>
		public static void RegisterProcessCreator(
			string fileExtension, Func<string, object[], MunProcess> creator)
			=> processCreators.Add(fileExtension, creator);
	}

	[Description("Operating System - interface to MunOS.")]
	public static class OperatingSystem
	{
		[Unsafe, Description("Default instance of MunOS.")]
		public static MunCore core => MunCore.Default;

		[Description("Number of processes.")]
		public static int processCount => core.ProcessCount;
		[Description("Number of threads.")]
		public static int threadCount => core.ThreadCount;

		static Process[] _processes;
		[Description("List of all processes.")]
		public static Process[] processes
		{
			get
			{
				if (_processes == null || _processes.Length != processCount)
				{
					_processes = new Process[processCount];
					int i = 0;
					foreach (var it in core)
					{
						if (!Process.map.TryGetValue(it, out var process))
							process = new Process(it);
						_processes[i++] = process;
					}
				}
				return _processes;
			}
		}

		[Description("Terminate current process, but allow cleanup.")]
		public static void terminate()
			=> MunProcess.Current?.Terminate();
		[Description("Terminate current process now, stopping all threads immediately.")]
		public static void kill()
			=> MunProcess.Current?.Terminate(hard: true);

		[Description("OS Process - collection of threads.")]
		public class Process
		{
			internal static readonly Dictionary<MunProcess, Process>
				map = new Dictionary<MunProcess, Process>();

			static Process _current;
			[Description("Current process")]
			public static Process current
			{
				get
				{
					var native = MunProcess.Current;
					if (_current?.native != native)
					{
						if (native == null)
							return null;
						if (!map.TryGetValue(native, out _current))
							_current = new Process(native);
					}
					return _current;
				}
			}

			[Unsafe, Description("MunOS process.")]
			MunProcess native;
			[Description("Unique process identifier.")]
			public readonly long id;
			public override string ToString() => id.ToString();

			internal Process(MunProcess it)
			{
				native = it;
				id = it.ID;
				map.Add(it, this);
				it.Disposed += Disposed;
			}
			public static implicit operator MunProcess(Process it) => it?.native;

			[Description("Create new process from a script (given path to it).")]
			public Process(string path)
			{
				var ext = System.IO.Path.GetExtension(path);
				if (!OperatingSystemInterface.processCreators.TryGetValue(ext, out var creator))
					throw new InvalidOperationException($"File extension '{ext}' not registered");
				var it = creator(path, null);
				if (it == null)
					throw new InvalidOperationException("Registered process creator did not return a process");
				native = it;
				id = it.ID;
				map.Add(it, this);
				it.Disposed += Disposed;
			}
			public Process(string path, params object[] args)
			{
				var ext = System.IO.Path.GetExtension(path);
				if (!OperatingSystemInterface.processCreators.TryGetValue(ext, out var creator))
					throw new InvalidOperationException("File extension not registered");
				var it = creator(path, args);
				if (it == null)
					throw new InvalidOperationException("Registered process creator did not return a process");
				native = it;
				id = it.ID;
				map.Add(it, this);
				it.Disposed += Disposed;
			}

			void Disposed(MunProcess it)
			{
				map.Remove(it);
				native = null;
				_processes = null;
			}

			[Description("Terminate the process, but allow cleanup.")]
			public void terminate() => native?.Terminate();
			[Description("Terminate the process now, stopping all threads immediately.")]
			public void kill() => native?.Terminate(true);
		}
	}
}
