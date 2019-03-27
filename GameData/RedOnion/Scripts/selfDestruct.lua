vessel=Ksp.FlightGlobals.ActiveVessel

function selfDestruct(part)
    local childParts=part.children
    for i=childParts.count-1,0,-1 do
        selfDestruct(childParts[i])
    end
    part.explode()
    for i=0,Ksp.Random.Range(1,60*10) do
        coroutine.yield()
    end
end

selfDestruct(vessel.parts[0])

