vessel=ksp.flightGlobals.ActiveVessel
ctrl=ksp.flightControl
ctrl.Reset()
-- ctrl.SetWithTable{roll=1}
ctrl.Enable()
local previousTime=os.time()
while true do
    -- print(ctrl.GetAvailableTorque())
    local newTime=os.time()
    print(newTime-previousTime)
    previousTime=newTime
    coroutine.yield()
end