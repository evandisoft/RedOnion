window=new(ui.Window,"Buttons")

window.ChildAnchors=ui.Anchors.Fill

window.AddButton("button1",function() print("button1") end)

window.AddButton("1-10000",function() for i=1,10000 do print(i) end end)