## Interruptibility: 
We need an auto-interrupt that is global to all coroutines in a certain context. Can be stored with the script associated with a coroutine. Coroutines created in that script share the same script. Might not be hard to implement.

### Side effects:
- Need to create a new implementation of sleep.

### Benefits:
- Users can use coroutines.
- They may even be able to use autoyield.