The code for this example is:

```
Random=native.UnityEngine.Random

function selfDestruct(part)
    local childParts=part.children
    for i=childParts.count-1,0,-1 do
        selfDestruct(childParts[i])
    end
    part.explode()
    sleep(Random.Range(0,10))
end

selfDestruct(ship.parts[0])
```

This example explodes all the parts of the ship, starting from the the [leaf parts](https://en.wikipedia.org/wiki/Tree_structure).

First we use `native` to get UnityEngine's `Random` class.
```
Random=native.UnityEngine.Random
```

In KSP, a Vessel has a collection of parts arranged in a tree-like fashion. The list of parts can also be viewed as a list, rather than a tree, which is what Vessel.parts, or ship.parts returns. But they are also connected to each other in a tree-structure, with each part having a list of children.

To take advantage of this structure, we define a [recursive function](https://www.youtube.com/watch?v=KEEKn7Me-ms) that will navigate the tree of parts and destroy leaf parts (parts that have no children) first. Recursion is very convenient for tree structures.

```
function selfDestruct(part)
    local childParts=part.children
    for i=childParts.count-1,0,-1 do
        selfDestruct(childParts[i])
    end
    part.explode()
    sleep(Random.Range(0,10))
end
```



The zeroth element of the parts list is the root of the parts tree. 

So
```
selfDestruct(ship.parts[0])
```
calls the recursive function on the root part.

Then we go through a loop calling `selfDestruct` recursively on all the children of that part. We're treating the children as each being a subtree. If a part has no children, `childParts.count-1` will be -1, and this loop never runs because it would be a loop from i=-1 to zero going backwards but -1 is already less than zero.

```
for i=childParts.count-1,0,-1 do
    selfDestruct(childParts[i])
end
```

After the loop, this part is guaranteed to have had all it's children and their children (etc) exploded.

So we can call explode on this part itself.

```
part.explode()
```

And then go to sleep a little bit to put a delay before exploding the next part.

```
sleep(Random.Range(0,10))
```

This may be confusing, but the first time we get to this point in the recursive function is actually after selfDestruct was called with a part that has no children. At that point the parent of the current part is waiting in a loop for it's children (including this part) to be destroyed. So this delay happens before the next child in that loop or the parent part explodes.