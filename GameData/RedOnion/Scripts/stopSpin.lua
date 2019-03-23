vessel=Ksp.FlightGlobals.ActiveVessel
ctrl=Ksp.FlightControl
local previousTime=os.time()
local ctrlTable={roll=0,pitch=0,yaw=0}

while true do
    print(vessel.ctrlState.GetPYR())

    torque=ctrl.GetAvailableTorque(vessel)

    local angularVelocity=Ksp.Vec.Div(vessel.angularMomentum,vessel.MOI)
    local maxAccel=Ksp.Vec.Div(torque,vessel.MOI)
    local input=Ksp.Vec.Div(angularVelocity,maxAccel*Ksp.Time.deltaTime)
    
    ctrlTable.pitch=input.x
    ctrlTable.roll=input.y
    ctrlTable.yaw=input.z
    
    ctrl.SetWithTable(ctrlTable)
    
    if(angularVelocity.magnitude<0.01) then
        ctrl.Shutdown()
        print("finished!")
        break
    end
    
    coroutine.yield()
end