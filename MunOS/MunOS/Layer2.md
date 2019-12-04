1. make it so that processes can start other processes, and go into a sleep mode (or return a ExecStatus.FINISHED to the
OS to remove itself OS consideration), until they get a notification from the other processes.

1. Use this to implement LiveRepl evaluate. Make LiveRepl create a process which waits for the executing process.

1. Make a general interface for queueing processes up, so that they will be ran in turn.

1. Potentially make one's run be conditional on a previous ones final status.

1. Maybe not this as it adds perhaps needless complexity. Perhaps a Process/Thread distinction at this layer. Destroying a proces ends all its threads.
1. Implement kill many for the OS to efficiently kill all a processes' threads in one iteration.
1. Perhaps only threads will run directly on MunOS.
1. Threads need to be able to wait on processes and/or threads, and processes need to wait on threads and or processes.

1. Implementation of IExecutable that is itself a queue of IExecutables. They run in sequence. IExecutables don't have
a return value though. So maybe we extend IExecutable. The extension has an event which can signal when it is done
with a Return value

1. Users will want to be able to run something and then later check it out. Process manager stats could be handled
in layer 2 instead of Core. Or separate exec stats from selecting finished processes. We won't want all running things
to stick around. Some things like button presses we will want to have them run and then finish.

We will also want people to be able to run functions from button presses that will stick around and can be connected
to processes. Perhaps if in 

1. Maybe process/thread distinction is important in layer2 because we want to be able to connect the process to LiveRepl
(or equivalent) in order to play around with the globals and what-not.

1. And LiveRepl's `terminate` button will destroy all threads associated with a process. Processes will not be IExecutables, but
threads will be.

1. ProcessManager will show information for processes and allow you to "switch" to a process, opening it in liverepl.