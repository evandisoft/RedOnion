## MunOS

Prior to version 0.5.0, one was not able to run a long-running program in Lua from a button press because the execution system for Lua could not manage running multiple long-running programs at the same time.

0.5.0 brings a new execution system, called **MunOS**, which works for both our engines, and now creates new "threads" whenever you execute some code. (These "threads" are not real OS threads and will not speed up your computations.) 

However, this allows you to have a list of buttons which run various programs, which you can press at any time, even while you are already running a script. You can also run a new script from LiveRepl even when you are currently running a script.

The Lua and ROS engines run in different **processes**, and now do not share an output buffer. This means that output your Lua threads have produced will not appear when you have the ROS engine selected.

In the future we will give you the power to create multiple processes for the same engine.

Clicking the **Terminate** button will kill all the threads from the currently selected process, and also all associated windows and vecdraws.