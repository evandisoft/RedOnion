require("basiclib")

window=new(ui.Window,"controls")
window.X=300
window.Y=-200

window.ChildAnchors=ui.Anchors.Fill

local function addbutton(text,fn)
    window.Add(new(ui.Button,text,fn))
end

addbutton("countdown",countdown)
addbutton("run experiments",runexps)
addbutton("auto parachutes",autopara)