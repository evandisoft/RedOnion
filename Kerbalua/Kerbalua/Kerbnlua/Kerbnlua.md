We load a string into the stack by doing 

state.LoadString(string)

then you state.

coroutines:
    if a function yields, I believe its 
    
https://stackoverflow.com/questions/26347022/meaning-of-the-from-parameter-of-lua-resume

state.Resume

Seems that the state.State is a thread and if you mess it up you cannot resume.
Maybe you can use pcall for that.

Cannot find a way to use the nicer state interface (pop() -> object) with a non-main thread.
Making a new thread returns a Keralua.Lua state.
(Found one way is using `xmove` to move things from the other thread's stack to the main thread)

ToCFunction doesn't work.
Popping a function off the stack and then putting it on again doesn't work.

Execution system is:
You have main thread in your `Lua state=new Lua();`
Then you create a thread.
`KeraLua.Lua thread=state.State.NewThread();`
This thread will share an environment, including globals, with the state, which represents the main thread.
Now to set some source for execution you say `thread.LoadString(source)`.
This compiles the code and creates a function to run it in. And that function is placed on the top 
of the stack.
Then you call `thread.resume(null,0)`. This calls the function on top of the stack, which is
the function that is executing the source.

