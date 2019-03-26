vessel=Ksp.FlightGlobals.ActiveVessel
part=vessel.parts[8]
m=part.Modules[0]

function printModules(part)
    for module in part.Modules do
        print(module.moduleName)
    end
end 

for part in vessel.parts do
    printModules(part)
end