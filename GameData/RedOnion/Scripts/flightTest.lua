vessel=ksp.flightGlobals.ActiveVessel
ctrl=ksp.flightControl
ctrl.Reset()
-- ctrl.SetWithTable{roll=1}
ctrl.Enable()
local previousTime=os.time()
local ctrlTable={roll=0,pitch=0,yaw=0,mainThrottle=1}

while true do
    torque=ctrl.GetAvailableTorque()
    local angularSpeed=ksp.vec.Div(vessel.angularMomentum,vessel.MOI)
    local accel=ksp.vec.Div(torque,vessel.MOI)

    local neededInput=ksp.vec.Div(ksp.vec.Abs(angularSpeed),accel*ksp.time.deltaTime);
    ctrlTable.roll=neededInput.y
    ctrlTable.yaw=neededInput.z
    ctrlTable.pitch=neededInput.x
    
    ctrl.SetWithTable(ctrlTable)
    
    --print(vessel.ctrlState.roll)
    --print(vessel.ctrlState.yaw)
    --print(vessel.ctrlState.pitch)
    
    coroutine.yield()
end