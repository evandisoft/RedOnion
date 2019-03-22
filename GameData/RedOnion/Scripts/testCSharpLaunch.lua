vessel=Ksp.FlightGlobals.ActiveVessel
ctrl=Ksp.FlightControl
ctrl.TargetDir=Ksp.Vec.New(0,-1,0)
while true do
    print(vessel.transform.up)
    print(vessel.ctrlState.GetPYR())
    --coroutine.yield()
end
