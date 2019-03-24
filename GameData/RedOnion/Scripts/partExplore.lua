vessel=Ksp.FlightGlobals.ActiveVessel
part=vessel.parts[0]

function printModules(part)
    for module in part.Modules do
        print(module)
    end
end 