Our [Lua engine](https://www.lua.org/manual/5.2/) uses [MoonSharp](http://www.moonsharp.org/) to create an in-game Lua scripting environment for KSP. 

Since MoonSharp is a lua implementation that can interact with Mono/C# objects, and since KSP is implemented in C# Mono, our engine can interact with any aspect of the the [KSP api](https://kerbalspaceprogram.com/api/annotated.html) ingame. We can provide in in-game scripting nearly any functionality a modmaker would normally have to access with C#.

We provide an API for both Lua and ROS that is documented [here](https://github.com/evandisoft/RedOnion/blob/master/CommonScriptApi.md)

Here is an example Lua script that can be [executed](https://github.com/evandisoft/RedOnion/blob/master/TroubleShooting.md#how-do-i-run-a-script) while you are in flight mode (make sure you have [the "Lua" engine selected](https://github.com/evandisoft/RedOnion/blob/master/TroubleShooting.md#script-wont-work)):
```
vessel=Ksp.FlightGlobals.ActiveVessel

for i=0,20*60 do
    coroutine.yield()
end

for j=1,5 do
    for i=0,Ksp.Random.Range(0,20*60) do
        coroutine.yield()
    end
    local num=Ksp.Random.Range(0,vessel.parts.Count)
    print(vessel.parts[num].ToString().."is malfunctioning!")
    vessel.parts[num].explode()
end
print("Done!")
```

A Lua script being ran in the editor/repl will have to return control to KSP at various points in order to allow KSP to execute its code. You can voluntarily temporarily yield control back to KSP by calling coroutine.yield(). This will effectively make the script wait a short time. So it can be used as a crude timing mechanism. If you do not call coroutine.yield() within a certain amount of time, your script will be automatically yielded, and then later automatically resumed at the point where it was automatically yielded.

This script is a script that waits a short time, and then explodes 5 random parts. It can be a fun piloting challenge because
it can make your ship very difficult to recover from. A video demonstration of my poor piloting skills being put to the test by this script is [here](https://www.youtube.com/watch?v=xzAghlB2NLw)

First thing this script does, is use our [CommonScriptApi](https://github.com/evandisoft/RedOnion/blob/master/CommonScriptApi.md) to
get access to a reference to the current vessel.

```
vessel=Ksp.FlightGlobals.ActiveVessel
```
And stores the current vessel in a global variable called "vessel"

Then goes into a normal lua for loop for 20*60=1200 iterations. This effectively causes the script to delay a while.

After that is done, it goes into another for loop that will run 5 times.
```
for j=1,5 do
```

Each iteration of the loop is going to first wait a random amount of time by yielding over and over again in this little loop:
```
for i=0,Ksp.Random.Range(0,20*60) do
    coroutine.yield()
end
```

Then it will select a random part index.
```
local num=Ksp.Random.Range(0,vessel.parts.Count)
```

Display which part will be exploding:
```
print(vessel.parts[num].ToString().."is malfunctioning!")
```

And finally call the part's explode functionality:
```
vessel.parts[num].explode()
```

If you have any questions, problems, or requests for new functionality please feel free to make an issue on this repository, or respond in our [KSP forum thread](https://forum.kerbalspaceprogram.com/index.php?/topic/183050-wipalpha-release-020-redonion-unrestricted-in-game-scripting-with-repl-and-live-editing-with-intellisense-lua-and-a-custom-jsruby-like-language-implemented/)
