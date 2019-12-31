Random=native.UnityEngine.Random
sleep(Random.Range(0,3))
print("entering loop")
repeat 
    sleep()
until not somelock

somelock=true
print("doing something")
print("sleeping")
sleep(1)
print("releasing lock...")
somelock=false