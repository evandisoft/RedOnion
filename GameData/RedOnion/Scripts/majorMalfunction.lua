Random=native.UnityEngine.Random

sleep(10)

for j=1,8 do
    sleep(Random.Range(0,10))
    
    local num=Random.Range(0,ship.parts.count)
    print(ship.parts[num].ToString().."is malfunctioning!")
    ship.parts[num].native.explode()
end
print("Done!")