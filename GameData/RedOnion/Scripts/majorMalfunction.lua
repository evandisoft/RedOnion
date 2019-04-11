vessel=import.FlightGlobals.ActiveVessel

Random=import.UnityEngine.Random

for i=0,20*60 do
    coroutine.yield()
end

for j=1,8 do
    for i=0,Random.Range(0,20*60) do
        coroutine.yield()
    end
    local num=Random.Range(0,vessel.parts.Count)
    print(vessel.parts[num].ToString().."is malfunctioning!")
    vessel.parts[num].explode()
end
print("Done!")