require("basiclib")

if ksp.HighLogic.LoadedScene==ksp.GameScenes.FLIGHT then
    require("flightgui")
    package.loaded["flightgui"]=nil
end

if ksp.HighLogic.LoadedScene==ksp.GameScenes.EDITOR then
    --window=new(ui.Window,"upgrade engines/control")
end