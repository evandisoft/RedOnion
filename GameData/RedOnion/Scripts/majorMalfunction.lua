vessel=Ksp.FlightGlobals.ActiveVessel

for i=0,20 do
    coroutine.yield()
end

for j=1,8 do
    for i=0,Ksp.Random.Range(0,20) do
        coroutine.yield()
    end
    local num=Ksp.Random.Range(0,vessel.parts.Count)
    print(vessel.parts[num].ToString().."is malfunctioning!")
    vessel.parts[num].explode()
end
print("Done!")