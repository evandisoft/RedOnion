local module={}

function module.setSpin(ctrlTable)
    local vessel=ksp.flightGlobals.ActiveVessel
    local ctrl=ksp.flightControl
    local targetSpinx
    local targetSpiny
    local targetSpinz
    
    targetSpinx,targetSpiny,targetSpinz=coroutine.yield(false)
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
        ctrlTable.roll=(yAngularSpeed-targetSpiny)/(yAccel*ksp.time.deltaTime)
        
        local xInertia=moi.x
        local xMomentum=vessel.angularMomentum.x
        local xAngularSpeed=xMomentum/xInertia
        local xAccel=xTorque/xInertia
        ctrlTable.pitch=(xAngularSpeed-targetSpinx)/(xAccel*ksp.time.deltaTime)
        
        local zInertia=moi.z
        local zMomentum=vessel.angularMomentum.z
        local zAngularSpeed=zMomentum/zInertia
        local zAccel=zTorque/zInertia
        ctrlTable.yaw=(zAngularSpeed-targetSpinz)/(zAccel*ksp.time.deltaTime)
        
        targetSpinx,targetSpiny,targetSpinz=coroutine.yield(false)
    end
end

function module.atTargetSpin(targetSpin,threshold)
    local moi=vessel.MOI
    local yInertia=moi.y
    local yMomentum=vessel.angularMomentum.y
    local yAngularSpeed=yMomentum/yInertia
    
    local xInertia=moi.x
    local xMomentum=vessel.angularMomentum.x
    local xAngularSpeed=xMomentum/xInertia

    local zInertia=moi.z
    local zMomentum=vessel.angularMomentum.z
    local zAngularSpeed=zMomentum/zInertia
    
    return math.abs(zAngularSpeed-targetSpin.z)<threshold 
        and math.abs(xAngularSpeed-targetSpin.x)<threshold
        and math.abs(yAngularSpeed-targetSpin.y)<threshold
end
    
return module