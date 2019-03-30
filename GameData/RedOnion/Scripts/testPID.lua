vessel=Ksp.FlightGlobals.ActiveVessel
ctrl=Ksp.FlightControl

local ctrlTable={pitch=0}
local Vec=Ksp.Vec

Kp=-5
Ki=0
Kd=0
dir=vessel.transform.up

function createPID(Kp,Ki,Kd)
    local previous_error = 0
    local integral = 0
    return function(error)
        integral = Ksp.Scalar.Clampf(integral + error * Ksp.Time.deltaTime,-1,1)
        local derivative = (error - previous_error) / Ksp.Time.deltaTime
        previous_error = error
        local output = Ksp.Scalar.Clampf(Kp * error + Ki * integral + Kd * derivative,-1,1)
        print("output is "..output)
        return output
    end
end
             
pitchPID=createPID(Kp,Ki,Kd)
yawPID=createPID(Kp,Ki,Kd)
while true do
  error=ctrl.CurrentDistanceAxis(vessel,dir)
  print(error.ToString())
  ctrlTable.pitch=pitchPID(error.x)
  ctrlTable.yaw=pitchPID(error.z)
  ctrl.SetWithTable(ctrlTable)
  coroutine.yield()
end