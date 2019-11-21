vessel=import.FlightGlobals.ActiveVessel

Random=import.UnityEngine.Random

sleep(10)

for j=1,8 do
    
    local num=Random.Range(0,vessel.parts.Count)
    print(vessel.parts[num].ToString().."is malfunctioning!")
    vessel.parts[num].explode()
end
print("Done!")