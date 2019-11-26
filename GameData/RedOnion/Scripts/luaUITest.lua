window=new(ui.Window,"Window",ui.Layout.Horizontal)

panel=new(ui.Panel,ui.Layout.Vertical)
window.Add(panel)
panel.ChildAnchors=ui.Anchors.Fill


function addbutton(text)
    local button=new(ui.Button)
    button.Text=text
    local f=function() print(text.." pressed!") end
    button.click.Add(f)
    panel.Add(button)
end

f=function(b) end
panel.Add(new(ui.Button,"test",f))

---[[
addbutton("blah")
addbutton("blah2")
addbutton("blah3")
--]]


    