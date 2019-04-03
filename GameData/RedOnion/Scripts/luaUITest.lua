w=reflect.new(UI.Window,"Window",UI.Layout.None)

p=reflect.new(UI.Panel)
w.Add(p)
p.Layout=UI.Layout.Vertical
p.ChildAnchors=UI.Anchors.Fill
b=reflect.new(UI.Button)
b.text="buttontext"
p.Add(b)

b.click.Add(function() local i=0 while true do print(i) i=i+1 end end)
--[[
var wnd = new window
function shutdown
    wnd.dispose
  
var ctrl = ksp.flightcontrol.GetInstance()
var vessel = ksp.flightglobals.ActiveVessel

var panel=new panel

wnd.add panel
panel.layout = layout.vertical
panel.ChildAnchors = Anchors.Fill

def mkbutton text,clickFn
    var btn = new button
    btn.text = text
    btn.click += clickFn
    return btn

panel.add mkbutton "StopSpin",def
    ctrl.StopSpin()
    
panel.add mkbutton "HoldOrientation",def
    ctrl.TargetDir = vessel.transform.up
    
panel.add mkbutton "Disable",def
    ctrl.shutdown
--]]