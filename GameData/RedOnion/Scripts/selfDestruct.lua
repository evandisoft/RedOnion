Random=native.UnityEngine.Random

function selfDestruct(part)
    local childParts=part.children
    for i=childParts.count-1,0,-1 do
        selfDestruct(childParts[i])
    end
    part.explode()
    sleep(Random.RandomRange(0,10))
end

selfDestruct(ship.parts[0].native)