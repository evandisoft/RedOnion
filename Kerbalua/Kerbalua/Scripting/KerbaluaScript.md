The specific implementation of the **Kerbalua** engine is in [KerbaluaScript.cs](Kerbalua/Scripting/KerbaluaScript.cs).

# Evaluation
MoonSharp's `Script` is the basic MoonSharp defined object for executing Lua code, and `KerbaluaScript` extends `Script`, adding some functionality to make script execution interruptible.

`KerbaluaScript` needs to be interruptible because otherwise script code will pause the game until it finishes, and an infinite loop in script code will force the user to restart KSP.

To use `KerbaluaScript`, one first sets the source for an evaluation with `SetCoroutine`, and then calls `Evaluate`.

`Evaluate` runs the previously set coroutine for a short time before pausing execution, either having completed or having been interrupted with the `AutoYieldCounter`, which pauses computation after a certain number of instructions.

`Evaluate` returns true if the execution of the source has finished, otherwise it returns false.