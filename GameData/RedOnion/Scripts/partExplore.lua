vessel=Ksp.FlightGlobals.ActiveVessel
part=vessel.parts[8]
m=part.Modules[0]

function each(lst,action)
    for i in lst do
        action(i)
    end
end

for part in vessel.parts do
    --printModules(part)
    each(part.Modules,function(m)
        print(m)
    end)
end