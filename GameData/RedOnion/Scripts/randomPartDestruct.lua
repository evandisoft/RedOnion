vessel=Ksp.FlightGlobals.ActiveVessel

for j=1,5 do
    for i=0,Ksp.Random.Range(0,20*60) do
        coroutine.yield()
    end
    local num=Ksp.Random.Range(0,vessel.parts.Count)
    vessel.parts[num].explode()
end
print("Done!")