vessel=Ksp.FlightGlobals.ActiveVessel
part=vessel.parts[0]

function printModules(part)
    for module in part.Modules do
        print(module)
    end
end 

for part in vessel.parts do
    printModules(part)
    print(part.persistentId)
end

module=part.Modules[0]