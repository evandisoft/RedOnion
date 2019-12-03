1. make it so that processes can start other processes, and go into a sleep mode (or return a ExecStatus.FINISHED to the
OS to remove itself OS consideration), until they get a notification from the other processes.

1. Use this to implement LiveRepl evaluate. Make LiveRepl create a process which waits for the executing process.

1. Make a general interface for queueing processes up, so that they will be ran in turn.

1. Potentially make one's run be conditional on a previous ones final status.

1. Process/Thread distinction at this layer. Destroying a proces ends all its threads.
1. Implement kill many for the OS to efficiently kill all a processes' threads in one iteration.
1. Perhaps only threads will run directly on MunOS.