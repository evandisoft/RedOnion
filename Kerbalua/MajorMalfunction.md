[![Dynawing-Malfunction.jpg](https://i.postimg.cc/5ygzNTdn/Dynawing-Malfunction.jpg)](https://postimg.cc/WqDz594k)

**There is a bug in the current implementation of explode with the wrapper object ship.parts, but the script still somewhat works. But I suppose it's ironic that there's a bug in this given the name of it. This bug also will effect selfDestruct.lua.**

It will be fixed in 0.4.2

Here is an example Lua script that can be [executed](https://github.com/evandisoft/RedOnion/blob/master/TroubleShooting.md#how-do-i-run-a-script) while you are in flight mode (assuming you don't mind your ship exploding). 
```
Random=native.UnityEngine.Random

sleep(10)

for j=1,8 do
    sleep(Random.Range(0,10))
    
    local num=Random.Range(0,ship.parts.count)
    print(ship.parts[num].ToString().."is malfunctioning!")
    ship.parts[num].explode()
end
print("Done!")
```

This script is a script that waits a short time, by calling sleep, and then explodes 5 random parts. It can be a fun piloting challenge because
it can make your ship very difficult to recover from. A video demonstration of my poor piloting skills being put to the test by this script is [here](https://www.youtube.com/watch?v=xzAghlB2NLw)

The first thing this script does is uses `native` to access UnityEngine's [Random](https://docs.unity3d.com/ScriptReference/Random.html) class.

```
Random=native.UnityEngine.Random
```
Then it waits for ten seconds.

```
sleep(10)
```

Then it goes into a loop
```
for j=1,8 do
```

Each iteration of the loop is going to first wait between 0 and 10 seconds.
```
sleep(Random.Range(0,10))
```

Then it will select a random part index.
```
local num=Random.Range(0,ship.parts.count)
```

Display which part will be exploding:
```
print(ship.parts[num].ToString().."is malfunctioning!")
```

And finally call the part's explode functionality:
```
ship.parts[num].explode()
```
