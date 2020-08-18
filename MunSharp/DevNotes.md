## Interruptibility: 
We need an auto-interrupt that is global to all coroutines in a certain context. Can be stored with the script associated with a coroutine. Coroutines created in that script share the same script. Might not be hard to implement.

I could put the stopwatch in MunSharp

### Requirements:
- Need to create a new implementation of sleep.

### Benefits:
- Users can use coroutines.
- They may even be able to use autoyield.

## Sleep

### Requirements:
- Sleep timer must be stored and checked in the thread.
- Calling sleep must immediately trigger a forced yield of the current coroutine so that it breaks out of the thread loop. 

## Forced Yield
A flag can be set on a script that is checked at the beginning of the processing loop. If true, it is then reset to false, and then returns a ForcedYieldReq()

### Requirements:
- Must integrate MunSharpTests before modifying MunSharp's Script.