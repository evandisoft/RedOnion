require("basiclib")

if ksp.HighLogic.LoadedScene==ksp.GameScenes.FLIGHT then
    require("flightgui")
    package.loaded["flightgui"]=nil
end