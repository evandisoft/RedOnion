targetAltitude = 80000000000
karmanAltitude = 75000
ascentHeading = 90

vessel=import.FlightGlobals.ActiveVessel
vdv=vessel.VesselDeltaV
-- s=vdv.GetStage(6)
-- es=s.enginesActiveInStage[0]
stager=import.KSP.UI.Screens.StageManager
stage=stager.ActivateNextStage

function deprived()
    local s=vdv.GetStage(vessel.currentStage)
    for e in s.enginesInStage do
        if e.GetThrustActual()==0 then
            print(e,"is deprived")
            return true
        end
    end
    return false
end

function checkStaging()
    local s=vdv.GetStage(vessel.currentStage)
    local snext=vdv.GetStage(vessel.currentStage-1)
    
    if not stager.CanSeparate then
        -- print("couldn't separate")
        return
    end
    
    if s==nil then
        print("s was nil")
        stage()
        return
    end
    
    if s.enginesInStage.Count == 0 then
        stage()
        return
    end
    
    if s.stageMass < snext.stageMass then
        print("current stage mass less than next stage mass")
        stage()
        return
    end
    
    -- if s.enginesActiveInStage.Count < s.enginesInStage.Count or 
    if deprived() then
        print("active",s.enginesActiveInStage.Count,"total",s.enginesInStage.Count)
        stage()
        return
    end
end

print("Target apoapsis: ",targetAltitude)
ctrl = import.RedOnion.KSP.Autopilot.FlightControl.Instance
while vessel.orbit.ApA < targetAltitude do
  checkStaging()
  ctrl.setRel(ascentHeading, 90)
  coroutine.yield()
end

print("Apoapsis reached")