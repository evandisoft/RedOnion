vessel=import.FlightGlobals.ActiveVessel
ctrl=import.RedOnion.KSP.Autopilot.FlightControl.GetInstance()
Vec=import.RedOnion.KSP.MathUtil.Vec
UE=import.UnityEngine

local previousTime=os.time()
local ctrlTable={roll=0,pitch=0,yaw=0}

while true do
    print(vessel.ctrlState.GetPYR())

    local blah=0
    blah,torque=ctrl.GetAllTorque(vessel)

    local angularVelocity=Vec.Div(vessel.angularMomentum,vessel.MOI)
    local maxAccel=Vec.Div(torque,vessel.MOI)
    local input=Vec.Div(angularVelocity,maxAccel*UE.Time.deltaTime)
    
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