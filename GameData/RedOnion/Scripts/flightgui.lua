require("basiclib")

window=new(ui.Window,"controls")
window.X=300
window.Y=-200

window.ChildAnchors=ui.Anchors.Fill

local HashSet=native.System.Collections.Generic.HashSet

--[[
local function addbutton(text,fn)
    window.Add(new(ui.Button,text,fn))
end

addbutton("countdown",countdown)
addbutton("run experiments",runexps)
addbutton("auto parachutes",autopara)
addbutton("eva report",function() runexp("EVA Report") end)
addbutton("crew report",function() runexp("Crew Report") end)
addbutton("temperature scan",function() runexp("Temperature Scan") end)
--]]

local function addScienceButtons()
    local modules=findmodules("ModuleScienceExperiment")
    local titles=new(HashSet)
    for module in modules do
        titles.Add(module.experiment.experimentTitle)
    end
    for title in titles do
        window.AddButton(title,function() runexp(title) end)
    end
end

window.AddButton("countdown",countdown)
window.AddButton("run experiments",runexps)
window.AddButton("auto parachutes",autopara)

addScienceButtons()