w=new(ui.Window,"Window",ui.Layout.None)

p=new(ui.Panel)
w.Add(p)
p.Layout=ui.Layout.FlowVertical
p.ChildAnchors=ui.Anchors.Fill
b=new(ui.Button)
b.text="buttontext"
p.Add(b)

b.click.add(function() print("button pressed!") end)