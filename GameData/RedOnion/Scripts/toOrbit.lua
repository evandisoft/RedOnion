vessel=import.FlightGlobals.ActiveVessel
ctrl=import.RedOnion.KSP.Autopilot.FlightControl.GetInstance()

ctrl.SetRel(90,90)

while vessel.altitude<5000 do
    coroutine.yield()
end

ctrl.SetRel(90,70)

while vessel.altitude<10000 do
    coroutine.yield()
end

ctrl.SetRel(90,50)

while vessel.altitude<20000 do
    coroutine.yield()
end

ctrl.SetRel(90,30)

while vessel.altitude<30000 do
    coroutine.yield()
end

ctrl.SetRel(90,0)

while vessel.altitude<50000 do
    coroutine.yield()
end

print("done!")