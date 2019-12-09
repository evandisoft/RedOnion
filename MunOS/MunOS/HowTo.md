Extend EngineProcess and EngineThread with ROSProcess and ROSThread.
You can look at my implementation of KerbaluaProcess and KerbaluaThread.

ROSProcess will implement many aspects of ReplEvaluator the same way: completion related functionality, as well as GetImportString, and Extension.

It will also implement `ExecuteSourceInThread`, which will create a new ROSThread
to execute the given source with whatever `ExecPriority` you wish.

In your implementation of `ExecuteSourceInThread`, you should call MunProcess' `ExecuteThread`,
which will take a thread you created, and run it. It will add it to the list of `runningThreads`,
and it will register a callback for when the thread is finished.

When the thread is finished, the callback will remove the thread from the list of runningThreads, print any errors, and
then call the empty virtual function `ThreadExecutionComplete`, which you can override. It may pass an exception,
or the exception may be null, indicating the process was killed or finished normally.
(In the future, I should probably pass an enum saying how the thread finished).

When terminate is called, it will kill all running threads, which will just remove them from being executed in
CoreExecMgr, and each time a thread is killed, it will remove the thread from the list of runningThreads and also
call `ThreadExecutionComplete`.

MunProcess has a public field called outputBuffer, which is the buffer to which you will print
whatever you wish to print. It has the same features of the OutputArea, like AddError() to print a new line
starting with "e> ", or AddOutput to print a new line starting with "o> ". This outputBuffer belongs to the
process itself, and LiveRepl fills the OutputArea with the contents of the outputBuffer from the current process
every update.

ROSThread will implement `ProtectedExecute`, which is called by MunThread.Execute, which is executed by the
CoreExecMgr with a given ticklimit.

MunThread handles setting the current thread to itself before it calls `ProtectedExecute` and setting it to null
after `ProtectedExecute` completes.

ROSThread will also implement IsSleeping, but you can just return false and the thread will just be selected to run
every update.

Note: There is a Process ID, a Thread ID, and an ExecInfo ID. ExecInfo ID's are only for running threads and only
used by the CoreExecMgr for killing threads. The ID in the runningThreads dictionary is the ExecInfo ID.

I've decided that the Process and Threads will handle stats, so those are the ID's that will show up in any process
manager window.

Processes and Threads have an optional name which will be used for display.

When you have a ROSThread and ROSProcess implementation, add a new ROSProcess in ScriptWindow.Evaluation's InitEvaluation function
at the beginning as I did with KerbaluaProcess.

I added two different instances of the lua engine for demonstration/testing.

You have to add the process both into the engineProcesses dict so it will show up in LiveRepl, and also
`ProcessManager.Instance.Processes`, so that its FixedUpdate will be called. (Though currently this does nothing).