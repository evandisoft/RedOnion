using RedOnion.Collections;
using System;
using System.Collections.Generic;
using static RedOnion.Debugging.QueueLogger;

namespace MunOS.Repl
{
	public abstract class ScriptManager
	{
		/// <summary>
		/// File extension used by the scripting engine (including the dot - ".lua", ".ros", ...).
		/// </summary>
		public abstract string Extension { get; }
		/// <summary>
		/// Create new process (for the threads this manager spawns).
		/// One is created using this method from <see cref="Initialize(MunCore, IList{string})"/> and assigned to <see cref="Process"/>.
		/// </summary>
		public abstract MunProcess CreateProcess();
		/// <summary>
		/// Create new thread.
		/// </summary>
		/// <param name="source">Source code (the script). Can be null (use path).</param>
		/// <param name="path">The path of the file (null for repl lines).</param>
		/// <param name="process">The process it should belong to or null (in which case <see cref="Process"/> is to be used).</param>
		/// <param name="priority">Execution priority of the thread (<see cref="MunPriority.Main"/> by default).</param>
		/// <param name="start">Whether the thread shall be started immediately or not.</param>
		public abstract MunThread CreateThread(string source, string path,
			MunProcess process = null, MunPriority priority = MunPriority.Main, bool start = true);

		/// <summary>
		/// The core this manager is associated with (set in <see cref="Initialize(MunCore, IList{string})"/>.
		/// </summary>
		public MunCore Core { get; protected set; }
		/// <summary>
		/// Main process (used by REPL).
		/// </summary>
		public MunProcess Process { get; protected set; }
		/// <summary>
		/// Indicator that <see cref="Initialize(MunCore, IList{string})"/> has been called
		/// and all init scripts finished executing.
		/// </summary>
		public bool Initialized { get; protected set; }

		public OutputBuffer OutputBuffer { get; } = new OutputBuffer();

		/// <summary>
		/// List of threads waiting for init scripts to finish before being scheduled for execution.
		/// </summary>
		protected ListCore<MunThread> waitingThreads;

		/// <summary>
		/// This method is to be called after creation and after Reset().
		/// May also be used to run chain of scripts before any other is accepted.
		/// </summary>
		/// <param name="core">MunCore to use (MunCore.Default if null).</param>
		/// <param name="startup">List of scripts (paths) to run first.</param>
		public virtual void Initialize(MunCore core = null, IList<string> startup = null)
		{
			if (Process != null)
				Reset();
			Core = core ?? MunCore.Default;
			Process = CreateProcess();
			Process.AutoRemove = false;
			Process.OutputBuffer = OutputBuffer;
			Initialized = startup == null || startup.Count == 0;
			if (!Initialized)
			{
				MunThread prev = null;
				foreach (var path in startup)
				{
					var thread = CreateThread(null, path, start: prev == null);
					if (prev != null)
						prev.ExecNext = thread;
					prev = thread;
				}
				Process.ThreadDone += InitThreadDone;
			}
		}
		private void InitThreadDone(MunThread thread)
		{
			MunLogger.DebugLog($@"Init script done: {thread}, next: {(
				thread.ExecNext?.ToString() ?? "none")}, ex: {(
				thread.Exception?.ToString() ?? "none")}");
			if (thread.ExecNext == null && thread.Exception == null)
			{
				MunLogger.DebugLog($"Waiting thread count: {waitingThreads.Count}");
				foreach (var waiting in waitingThreads)
				{
					MunLogger.DebugLog($"Scheduling thread#{waiting.ID}");
					Core.Schedule(waiting);
				}
				waitingThreads.Clear();
				Initialized = true;
				Process.ThreadDone -= InitThreadDone;
				MunLogger.DebugLog("Initialized");
			}
		}

		public virtual void Evaluate(string source, string path, bool withHistory = false)
		{
			var thread = CreateThread(source, path, start: Initialized);
			if (!Initialized)
				waitingThreads.Add(thread);
			if (withHistory)
				History.Add(source);
		}

		public virtual void Terminate()
			=> Process?.Terminate(false);

		public virtual void Reset()
		{
			Process?.Dispose();
			foreach (var thread in waitingThreads)
				thread.Terminate(hard: true);
			waitingThreads.Clear();
			Process = null;
			Core = null;
			Initialized = false;
		}

		/// <summary>
		/// Looking at the source text and an index to the current cursor
		/// position, return a sorted list of strings that are the possible completions
		/// in the current context.
		/// </summary>
		/// <returns>The completion.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		/// <param name="replaceStart">The start index of the string to be replaced by a completion.</param>
		/// <param name="replaceEnd">The end index of the string to be replaced by a completion</param>
		public abstract IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd, MunProcess process = null);

		/// <summary>
		/// Gets the displayable completions. These will be the completions that the
		/// user is to see when deciding what option to select. For example
		/// the actual completion might be to turn 'anobject.completion' 
		/// into 'anobject["completion"]'. But the user would see just 'completion'
		/// in the list.
		/// </summary>
		/// <returns>The displayable completions.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		/// <param name="replaceStart">Replace start.</param>
		/// <param name="replaceEnd">Replace end.</param>
		public virtual IList<string> GetDisplayableCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd, MunProcess process = null)
			=> GetCompletions(source, cursorPos, out replaceStart, out replaceEnd, process);


		public HistoryManager History { get; } = new HistoryManager();
		public class HistoryManager
		{
			LinkedList<string> list = new LinkedList<string>();
			LinkedListNode<string> node;
			public string Current => node?.Value ?? "";
			public int MaxSize { get; set; } = 1000; // TODO: apply the limit when changing

			public void Add(string source)
			{
				if (!(list.Count > 0 && source == list.First.Value))
				{
					list.AddFirst(source);
				}

				if (list.Count > MaxSize)
				{
					list.RemoveLast();
				}

				node = null;
			}

			public string Up()
			{
				if (list.Count==0)
				{
					return "";
				}

				if (node == null)
				{
					node = list.First;
				}
				else if (node != list.Last)
				{
					node = node.Next;
				}

				return node.Value;
			}

			public string Down()
			{
				if (node == null)
				{
					return "";
				}

				if (node != list.First)
				{
					node = node.Previous;
				}

				return node.Value;
			}
		}
	}
}
