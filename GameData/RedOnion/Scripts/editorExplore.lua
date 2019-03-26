-- exploring interaction with editor functionality
editor=Ksp.EditorLogic
panels=Ksp.EditorPanels
ship=editor.ship
parts=ship.parts

function each(lst,action)
    for i=0,lst.count-1 do
        action(lst[i])
    end
end

for part in parts do
    each(part.Modules,function(m)
        print(m)
    end)
end