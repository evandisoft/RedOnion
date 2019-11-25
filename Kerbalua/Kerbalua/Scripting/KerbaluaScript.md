The specific implementation of the **Kerbalua** engine is in [KerbaluaScript.cs](KerbaluaScript.cs).

MoonSharp's `Script` class executes the Lua code, and `KerbaluaScript` extends `Script`, adding an interface for interruptible execution.

`KerbaluaScript` needs to be interruptible because otherwise script code will pause the game until it finishes, and an infinite loop in script code will force the user to restart KSP.

To use `KerbaluaScript`, one first sets the source for an evaluation with `SetCoroutine`, and then calls `Evaluate`.

## SetCoroutine
The code first checks if the source being evaluated is an expression that needs to be returned. 

```
if (IncompleteLuaParsing.IsImplicitReturn(source))
```

(This uses some parsing functionality mostly provided by [ANTLR](https://www.antlr.org/) on a custom Lua Grammar I created called `IncompleteLua`.)

If so, it prepends "return".
```
source = "return " + source;
```

Normally entering a simple expresssion in lua would require one to also add that "return ", but for convenience, **Kerbalua** adds it to allow people to easily check simple expressions at the repl.

The next step is that the code needs to be a function in order for a coroutine to be created from it. So we create a string that will build and return a function from that source, and we execute that string with (`base.DoString`)

(The "\n" is to prevent a line comment in the source from commenting out the `end`)
```
DynValue mainFunction = base.DoString("return function () " + source + "\n end");
```

And then we have the coroutine.

```
coroutine = CreateCoroutine(mainFunction);
```

## Evaluation
`Evaluate` runs the previously set coroutine for a short time before pausing execution, either having completed execution of the source or having been interrupted with the `AutoYieldCounter`, which pauses computation after a certain number of instructions.

`Evaluate` then returns true if the execution of the source has finished, otherwise it returns false.

`Evaluate` can be called again and again without setting new source code until it finishes execution of the previously set source code.