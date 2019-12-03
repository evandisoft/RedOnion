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