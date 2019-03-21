vessel=ksp.flightGlobals.ActiveVessel
while true do
    print(vessel.ctrlState.roll)
    print(vessel.ctrlState.yaw)
    print(vessel.ctrlState.pitch)
    coroutine.yield()
end

