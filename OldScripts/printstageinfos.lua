vessel=import.FlightGlobals.ActiveVessel
vdv=vessel.VesselDeltaV

currentStageNum=vessel.currentStage
function printStageMass(stageNum)
    local sdv=vdv.GetStage(stageNum)
    
    if sdv!=nil then
        print("Stage "..stageNum)
        print("Start Mass:"..sdv.startMass)
        print("End Mass:  "..sdv.endMass)
        print("Dry Mass:  "..sdv.dryMass)
        print("Fuel Mass: "..sdv.fuelMass)
        print("Total Mass:"..sdv.stageMass)
    end
end

for i=currentStageNum,3,-1 do
    printStageMass(i)
end