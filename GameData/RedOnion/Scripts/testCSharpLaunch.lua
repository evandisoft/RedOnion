vessel=Ksp.FlightGlobals.ActiveVessel
ctrl=Ksp.FlightControl
ctrl.TargetDir=vessel.transform.up --Ksp.Vec.New(1,0,0)
while true do
    print(vessel.transform.up)
    print(vessel.ctrlState.GetPYR())
    coroutine.yield()
end
