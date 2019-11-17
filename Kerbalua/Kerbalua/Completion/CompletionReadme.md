The design here is that a `CompletionObject` holds a reference to the object it is completing
for and knows how to do completion for it. 
 
For methods of the form 
`TryX(CompletionOperations operations, out CompletionObject completionObject)`,
the subclass either performs the operation and moves the `operations` object forward, 
or decides to merely return some other `CompletionObject` that the system should use for the 
operation. If it performs the operation, it also retuns the next `CompletionObject` based 
on the outcome of the operation.
 
The `CompletionObject` may also consume more than one operation, as in the case with named method
calls, which consist of two operations: a `getmember` operation followed by a `call` operation.