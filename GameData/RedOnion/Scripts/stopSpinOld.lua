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
    ctrlTable.roll=yAngularSpeed/(yAccel*ksp.time.deltaTime)
    
    xInertia=moi.x
    xMomentum=vessel.angularMomentum.x
    xAngularSpeed=xMomentum/xInertia
    xAccel=xTorque/xInertia
    ctrlTable.pitch=xAngularSpeed/(xAccel*ksp.time.deltaTime)
    
    zInertia=moi.z
    zMomentum=vessel.angularMomentum.z
    zAngularSpeed=zMomentum/zInertia
    zAccel=zTorque/zInertia
    ctrlTable.yaw=zAngularSpeed/(zAccel*ksp.time.deltaTime)

    ctrl.SetWithTable(ctrlTable)
    
    if(math.abs(zAngularSpeed)<0.001 and math.abs(xAngularSpeed)<0.001 and math.abs(yAngularSpeed)<0.001) then
        ctrl.Init()
        break
    end
    
    coroutine.yield()
end