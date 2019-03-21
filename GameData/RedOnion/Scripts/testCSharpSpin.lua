vessel=Ksp.FlightGlobals.ActiveVessel
ctrl=Ksp.FlightControl
ctrl.TargetDir=Ksp.Vec.New(vessel.transform.up)
while true do
    print(vessel.transform.up)
    coroutine.yield()
end