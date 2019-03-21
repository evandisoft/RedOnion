local module={}

function module.stopSpin()
    local vessel=ksp.flightGlobals.ActiveVessel
    local ctrl=ksp.flightControl
    local ctrlTable={roll=0,pitch=0,yaw=0}
    
    while true do
        local torque=ctrl.GetAvailableTorque()
        
        local yTorque=torque.y
        local xTorque=torque.x
        local zTorque=torque.z
        
        moi=vessel.MOI
        local yInertia=moi.y
        local yMomentum=vessel.angularMomentum.y
        local yAngularSpeed=yMomentum/yInertia
        local yAccel=yTorque/yInertia
        ctrlTable.roll=yAngularSpeed/(yAccel*ksp.time.deltaTime)
        
        local xInertia=moi.x
        local xMomentum=vessel.angularMomentum.x
        local xAngularSpeed=xMomentum/xInertia
        local xAccel=xTorque/xInertia
        ctrlTable.pitch=xAngularSpeed/(xAccel*ksp.time.deltaTime)
        
        local zInertia=moi.z
        local zMomentum=vessel.angularMomentum.z
        local zAngularSpeed=zMomentum/zInertia
        local zAccel=zTorque/zInertia
        ctrlTable.yaw=zAngularSpeed/(zAccel*ksp.time.deltaTime)
        
        coroutine.yield()
    end
end

return module