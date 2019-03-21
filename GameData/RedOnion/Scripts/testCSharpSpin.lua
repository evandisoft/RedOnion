vessel=Ksp.FlightGlobals.ActiveVessel
ctrl=Ksp.FlightControl
ctrl.TargetDir=Ksp.Vec.New(0,0,1)
while true do
    print(vessel.transform.up)
    coroutine.yield()
end