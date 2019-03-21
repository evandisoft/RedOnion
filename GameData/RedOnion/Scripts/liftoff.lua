vessel=ksp.flightGlobals.ActiveVessel
ctrl=ksp.flightControl
ctrl.Reset()
-- ctrl.SetWithTable{roll=1}
ctrl.Enable()
local previousTime=os.time()
local ctrlTable={roll=0,pitch=0,yaw=0}
torque=ctrl.GetAvailableTorque()
yTorque=torque.y
xTorque=torque.x
zTorque=torque.z


while true do
    moi=vessel.MOI
    yInertia=moi.y
    yMomentum=vessel.angularMomentum.y
    yAngularSpeed=yMomentum/yInertia
    yAccel=yTorque/yInertia
    if (yAccel*ksp.time.deltaTime<math.abs(yAngularSpeed)) then
        ctrlTable.roll=1*ksp.mathf.Sign(yMomentum)
    else
        ctrlTable.roll=math.abs(yAngularSpeed)/(yAccel*ksp.time.deltaTime)*ksp.mathf.Sign(yMomentum)
    end
    
    xInertia=moi.x
    xMomentum=vessel.angularMomentum.x
    xAngularSpeed=xMomentum/xInertia
    xAccel=xTorque/xInertia
    if (xAccel*ksp.time.deltaTime<math.abs(xAngularSpeed)) then
        ctrlTable.pitch=1*ksp.mathf.Sign(xMomentum)
    else
        ctrlTable.pitch=math.abs(xAngularSpeed)/(xAccel*ksp.time.deltaTime)*ksp.mathf.Sign(xMomentum)
    end
    
    zInertia=moi.z
    zMomentum=vessel.angularMomentum.z
    zAngularSpeed=zMomentum/zInertia
    zAccel=zTorque/zInertia
    if (zAccel*ksp.time.deltaTime<math.abs(zAngularSpeed)) then
        ctrlTable.yaw=1*ksp.mathf.Sign(zMomentum)
    else
        ctrlTable.yaw=math.abs(zAngularSpeed)/(zAccel*ksp.time.deltaTime)*ksp.mathf.Sign(zMomentum)
    end
    
    ctrl.SetWithTable(ctrlTable)
    
    if((vessel.angularMomentum/vessel.MOI).magnitude<0.01) then
        ctrl.Reset()
        ctrl.Disable()
        break
    end
    
    coroutine.yield()
end