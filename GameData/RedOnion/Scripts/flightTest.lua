vessel=Ksp.FlightGlobals.ActiveVessel
ctrl=Ksp.FlightControl
ctrl.Reset()
-- ctrl.SetWithTable{roll=1}
ctrl.Enable()
local previousTime=os.time()
local ctrlTable={roll=0,pitch=0,yaw=0,mainThrottle=1}
local Vec=Ksp.Vec

while true do
    local torque=ctrl.GetAvailableTorque()
    local angularSpeed=Vec.Div(vessel.angularMomentum,vessel.MOI)
    local accel=Vec.Div(torque,vessel.MOI)

    local v=Vec.Div(angularSpeed,accel*Ksp.Time.deltaTime);
    ctrlTable.roll=v.y
    ctrlTable.yaw=v.z
    ctrlTable.pitch=v.x
    
    ctrl.SetWithTable(ctrlTable)
    
    --print(vessel.ctrlState.roll)
    --print(vessel.ctrlState.yaw)
    --print(vessel.ctrlState.pitch)
    
    coroutine.yield()
end