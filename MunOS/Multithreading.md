## MunOS

Prior to version 0.5.0, one was not able to run a long-running program in Lua from a button press because the execution system for Lua could not manage running multiple long-running programs at the same time.

0.5.0 brings a new execution system, called **MunOS**, which works for both our engines, and now creates new "threads" whenever you execute some code. (These "threads" are not real OS threads and will not speed up your computations.) 

This allows you to have a list of buttons which run various programs, which you can press at any time, even while you are already running a script. You can also run a new script from LiveRepl even when you are currently running a script.

The Lua and ROS engines run in different **processes**, and now do not share an output buffer. This means that output your Lua threads have produced will not appear when you have the ROS engine selected.

Clicking the **Terminate** button will kill all the threads from the currently selected process, and also all associated windows and vecdraws.

For now there are only two processes: 1 Lua engine and 1 ROS engine, but in the future we will create both an api, and a user interface for creating/killing processes, allowing the user to create additional Lua/ROS processes and manage them.

### Managing Multithreading
Even though multiple threads in MunOS are not running simultaneously with respect to the computer you are running (they are not using multiple physical cores or multiple Linux/Windows threads), multiple MunOS threads will often be running in a particular update, with threads being interrupted by MunOS to give time to other threads for that update. (However, script code will never be interrupted during a native C# call.)

This can lead to issues when you want only one thread to use a particular resource at a time. Global variables are shared between all threads in the same `process`. To manage these possible interactions you can create your own locking mechanisms using a feature both engines provide:

**In general you are not going to be able to know when one of your "threads" will be interrupted to run another thread. But there is one exception: immediately after a `sleep` call in Lua, or a `yield` call in ROS, your thread will have at least 100 uninterruptible instructions.**

What 100 instructions will allow you to do is different for the two engines, but what it will _at least_ allow you to do is the following (in lua):
```
sleep()
if not somelock then
    somelock=true
```
Or ROS:
```
yield
if !("somelock" in globals)
    globals.somelock=false

yield
if !somelock
    somelock=true
```
(In ROS, you have to set the global `somelock` before using it. And you will want to set it only one time, hence the use of `yield`.)

Here `somelock` is a global variable. Immediately after the `sleep/yield`, you will be guaranteed to have enough instructions to test a global variable, and set it to a simple constant value like `true`. You will not be guaranteed to have enough instructions for something more expensive like
```
sleep()
if not complicatedLogic() then
    somelock=true
```

Because the function `complicatedLogic` may take longer than the amount of instructions you have left.

After you have tested `somelock` and set it to `true`, you can run anything inside the if statement and it won't be simultaneously running in any other thread that is running the same source code:

Lua:
```
sleep()
if not somelock then
    somelock=true
    ...
    somelock=false
end
```
ROS:
```
yield
if !("somelock" in globals)
    globals.somelock=false

yield
if !somelock
    somelock=true
    ...
    somelock=false
```


Don't forget to set the lock to false again before you exit the if statement.

If you have multiple threads trying to run this exact same code at the same time, they will all reach the `sleep/yield` statement, and wait. The first thread to leave that statement will be guaranteed to at least check the lock (which we will assume starts out as `false` or `nil`), and then set it before any other thread can interrupt it.

After the point where the lock was set, any code of any length can be ran in the area indicated by `...` with the assurance that only one thread will be running that code at the same time, because the first thread to leave the `sleep/yield` will set the global variable `somelock` to true and thus all other threads will not enter the if statement until somelock was again set to false.

So even though the first thread could end up being interrupted inside the `...` area, no other thread will be running that same code.

If you want all the threads to run some code for sure, but just take turns, you can do the following (in Lua):
```
repeat 
    sleep()
until not somelock

somelock=true
...
somelock=false
```
ROS:
```
yield
if !("somelock" in globals)
    globals.somelock=false

do
    yield
until !somelock
somelock=true
...
somelock=false
```

And this will have every thread wait in the loop until their turn to run, and any time a thread returns from `sleep/yield`, it will immediately check somelock, and if it was false, it will break from the loop, and immediately set the lock to true before any other thread gets to interrupt it.

Importantly, after whatever the thread was doing in `...` is completed, we set somelock to false again so that the next thread can escape the loop.

In the future, explicit locking functionality may be added to make the implementation simpler.