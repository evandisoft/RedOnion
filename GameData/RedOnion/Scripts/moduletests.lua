function countdown()
    for i=10,1,-1 do
        print(i)
        sleep(1)
    end
    print("Lift off!")
end

function findmodules(modulename)
    local modules={}
    for part in ship.native.parts do
        for module in part.Modules do
            if module.ClassName==modulename then
                table.insert(modules,module)
            end
        end
    end
    return modules
end

function waitforfalling()
    local alt1=altitude
    local alt2=altitude+100000
    while true do
        alt1=altitude
        sleep(1)
        alt2=altitude
        if alt2<alt1 then
            break
        end
    end
end

function runExperiments()
    local experiments=findmodules("ModuleScienceExperiment")
    for exper in experiments do
        exper.DeployAction()
    end
end

function waitforanddeployparachutes()
    local parachutes=findmodules("ModuleParachute")
    local parachute=parachutes[1]
    while altitude>8000 do
        sleep(1)
    end
    while tostring(parachute.deploymentSafeState)~="SAFE" do
        sleep(1)
    end
    for parachute in parachutes do
        parachute.DeployAction()
    end
end

experiments=findmodules("ModuleScienceExperiment")

experiment=experiments[1]
container=findmodules("ModuleScienceContainer")[1]

data=new(native.ScienceData,1,1,1,"crewReport@KerbinSrfLandedLaunchPad","blah")

container.AddData(data)