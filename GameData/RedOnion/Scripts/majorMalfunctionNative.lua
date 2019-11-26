Random=native.UnityEngine.Random

sleep(10)

for j=1,8 do
    sleep(Random.Range(0,10))
    
    local num=Random.Range(0,ship.native.parts.Count)
    print(ship.native.parts[num].ToString().."is malfunctioning!")
    ship.native.parts[num].explode()
end
print("Done!")