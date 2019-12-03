- The design for the core is that it only knows how to execute IExecutables.

- Its sole purpose is to divide up execution time and tell IExecutables how much
time to execute for, removing IExecutables when they are finished, terminated with Kill, or
when calling IExecutable.Execute throws an exception.

- This allows the code for the Core to be potentially very simple and independant, so
it is easy to understand and relatively easy to test.

- More functionality is going to be added, but in different `layers`, which will
use the Core for the specific task of managing multiple things running at once.

- An additional plus of this is that anything that wants to can run something on the
core directly without going through additional layers.