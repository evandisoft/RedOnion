[MajorMalfunction](MajorMalfunction.md) demonstrated a simple script using our our [CommonAPI](../RedOnion.KSP/API/Globals.md).

However, in v0.4.1, the `PartSet` returned by `ship.parts` didn't work properly when a part is exploded. When there is a bug like this in the safe classes, or the safe classes just don't provide a feature available in the [KSP API](https://kerbalspaceprogram.com/api/annotated.html), you can fall back to the native version of things. (This bug was fixed in v0.4.2)

`majorMalfunction.lua` was originally written using only native functionality and no "safe" classes. A version of `majorMalfunction.lua` that falls back to the native functionality is as follows:

```
Random=native.UnityEngine.Random

sleep(10)

for j=1,8 do
    sleep(Random.Range(0,10))
    
    local num=Random.Range(0,ship.native.parts.Count)
    print(ship.native.parts[num].ToString().."is malfunctioning!")
    ship.native.parts[num].explode()
end
print("Done!")
```

All that was changed was to use a "native" version of "ship" and capitalize "Count". Ship is a safe object that acts in place of [KSP's Vessel class](https://kerbalspaceprogram.com/api/class_vessel.html). Safe versions may have some member names changed to lowercase, to match the feel of lua's preference for lowercase. Safe versions of native objects may also have additional useful functionality not seen in the native version.

Here we are using our global `ship` to get access to the safe Ship object, but then accessing its "unsafe" native field that, in this case, has a working feature to replace a non-working one, but in general native versions will have some functionality that the "safe" version doesn't have, including stuff that is useful, stuff that is not well documented, and stuff that may cause problems.

In any case, ship.native returns the current active Vessel. You can also get access to this KSP object with
`native.FlightGlobals.ActiveVessel` when you are in flight mode. When you are not in flight mode `ActiveVessel` is null, as is the `ship` global.

`FlightGlobals` is a ksp object that provides access to many useful live objects, including the "ActiveVessel". Also has a list of the planetary bodies in the system in its `Bodies` field.

Anyway, back to the code. All the code does is fall back to native functionality. Of note, in the native version of ship.parts (`ship.native.parts`) `count` is capitalized. In the native version, Vessel.parts is just a CLR List<Part>, and the `Count` property of a CLR generic List<T> is capitalized.

All the code works the same as in the non-native [example](MajorMalfunction.md), just using the native parts, (List\<Part\>) and native Part instead of the safe versions.

If an object in the api exposes a "native" field, you can return that native value and see which type it is (the repl will show you), and then you can look up that type in the [KSP API](https://kerbalspaceprogram.com/api/index.html) by typing the classname in the little search box on the right.