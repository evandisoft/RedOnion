- The design for the core is that it only knows how to execute IExecutables.

- Its sole purpose is to divide up execution time and tell IExecutables how much
time to execute for, removing IExecutables when they are finished, terminated with Kill, or
when calling IExecutable.Execute throws an exception.

- This allows the code for the Core to be potentially very simple and independant, so
it is easy to understand and relatively easy to test.

- More functionality is going to be added, but in different `layers`, which will
use the Core for the specific task of managing multiple things running at once.

- An additional plus of this is that anything that wants to run something on the
core directly, without going through additional layers, can do that.

- One feature that may be added to the Core later, is some way to make it so that there are multiple
`views` into the CoreExecMgr and each view gets an equal slice of the execution time in any given priority.
And the executables a view contains have to execute using the overall time given to that view. (But it
would give a view more time in this update if its executables didn't all yield and other view's executables 
did all yield or finish early)