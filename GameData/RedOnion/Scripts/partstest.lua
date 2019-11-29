parts=ship.native.parts
modules=parts[1].Modules
module=modules[0]

for part in parts do
    for module in part.Modules do
        print(module)
    end
end