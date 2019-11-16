
   https://stackoverflow.com/questions/26347022/meaning-of-the-from-parameter-of-lua-resume

Seems that the state.State is a thread and if you mess it up you cannot resume.
I think you can with pcallk.

Don't know of a way to pop objects safely off a non main thread except by using otherThread.xmove(mainThread,n) to move
n objects from the non-main thread's stack to the main thread's stack and then using mainThread.Pop() to pop them off.

Popping a function off the stack with ToCFunction+Pop(1) and then putting it on again doesn't work.

Execution system is:
You have main thread in your `Lua state=new Lua();`
Then you create a thread: `KeraLua.Lua thread=state.State.NewThread();`
This thread will share an environment, including globals, with the main thread, which represented by the Lua class.
Now to set some `source` code for execution you say `thread.LoadString(source)`.
That compiles the code in `source` places the resulting executable code in a function on top of the stack.
Then you call `thread.Resume(null,0)` to start that executable code. `thread.Resume(null,0)` calls the function on top of the stack, which is the function we just placed on it. Check lua docs for lua_resume to find out more. We could have also used the function `thread.Call` to call the code, but using `thread.Resume` allows the code to be interrupted and then resumed.

I believe that when you call `thread.Resume` with a function on the stack, that thread becomes dedicated to executing that code
until it is finished. So I'm creating new `threads` each time we need to start a new execution (in case the code errors out, or never ends).

It can also be interrupted with a hook and later continued. It can be interrupted automatically by setting up a `hook` function.

`state.State.SetHook(AutoyieldHook, KeraLua.LuaHookMask.Count, 1000);`

```C#
private void AutoyieldHook(IntPtr luaState, IntPtr ar)
{
    var keraluaState = KeraLua.Lua.FromIntPtr(luaState);
    
    if (keraluaState.IsYieldable)
    {
        keraluaState.Yield(0);
    }
}
```

The hook function, `AutoyieldHook` has been set up to be automatically called every 1000 instructions.
Normally this hook functionality is used for debugging, and can also be set to execute every line, every function call, or every return.

However, as far as I know, only one hook can be active at once, so debugging features will have to be done thoughtfully.
Users can also set the hook in lua code (Example: `debug.sethook(hookfunction,'c',1000)`), but that hook overrides the one
Autoyield hook that I need in order to prevent infinite loops from freezing KSP.