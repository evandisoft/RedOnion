-- exploring interaction with editor functionality
editor=Ksp.EditorLogic

ship=editor.ship
parts=ship.parts
part=parts[0]
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