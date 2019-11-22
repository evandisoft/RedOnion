The **Kerbalua** project is implemented using [MoonSharp](http://www.moonsharp.org/). 

The assembly name for our packaged MoonSharp dll has been renamed to `KerbaluaMoonSharp` for the reason explained [here](KerbaluaMoonSharp.md).

# Implementation

The specific implementation of the **Kerbalua** engine is in [KerbaluaScript.cs](Kerbalua/Scripting/KerbaluaScript.cs).

MoonSharp's `Script` is the basic MoonSharp defined object for executing Lua code, and `KerbaluaScript` extends `Script`, adding some functionality to make script execution interruptible.

`KerbaluaScript` needs to be interruptible because otherwise a long script code will pause the game until it finishes. An infinite loop in script code will force the user to restart KSP.

To use `KerbaluaScript`, one first sets the source for an evaluation with `SetCoroutine`, and then calls `Evaluate`.

`Evaluate` runs the previously set coroutine for a short time before pausing execution, either having completed or having been interrupted with the `AutoYieldCounter`, which pauses computation after a certain number of instructions.

`Evaluate` returns true if the execution of the source has finished, otherwise it returns false.
