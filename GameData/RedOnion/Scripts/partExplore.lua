vessel=Ksp.FlightGlobals.ActiveVessel
part=vessel.parts[8]
m=part.Modules[0]

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
        end
    end)
end