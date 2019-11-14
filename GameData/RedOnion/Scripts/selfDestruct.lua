vessel=import.FlightGlobals.ActiveVessel
Random=import.UnityEngine.Random

function selfDestruct(part)
    local childParts=part.children
    for i=childParts.count-1,0,-1 do
        selfDestruct(childParts[i])
    end
    part.explode()
    for i=0, Random.Range(20*1,20*2) do
        coroutine.yield()
    end
end

selfDestruct(vessel.parts[0])