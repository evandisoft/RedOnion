vessel=Ksp.FlightGlobals.ActiveVessel
part=vessel.parts[8]
m=part.Modules[0]
ctrl=Ksp.FlightControl

function each(lst,action)
    for i in lst do
        action(i)
    end
end

ctrlSurface=nil
for part in vessel.parts do
    --printModules(part)
    each(part.Modules,function(m)
        print(m)
        if (m.GUIName=="Control Surface") then
            ctrlSurface=m
            return
        end
    end)
end

while true do
    --print(vessel.transform.up.Cross(ctrlSurface.liftForce,vessel.localCoM-ctrlSurface.transform.position))
    --print(www)
    print(ctrlSurface.transform.up,ctrlSurface.part.transform.up)
end