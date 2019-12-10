window=new(ui.Window,"Buttons")

window.ChildAnchors=ui.Anchors.Fill

--[[
function addbutton(text)
    window.Add(new(ui.Button,text,function() print(text) end))
end

addbutton("button1")
addbutton("button2")
addbutton("button3")
addbutton("button4")
addbutton("button5")
addbutton("button6")
addbutton("button7")
addbutton("button8")
--]]