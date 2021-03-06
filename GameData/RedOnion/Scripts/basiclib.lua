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

function runexps()
    print("Running experiments")
    local experiments=findmodules("ModuleScienceExperiment")
    for exper in experiments do
        exper.DeployAction()
    end
end

function runexp(title)
    local experiments=findmodules("ModuleScienceExperiment")
    for experiment in experiments do
        if experiment.experiment.experimentTitle==title then
            print("running experiment "..experiment.experiment.experimentTitle)
            experiment.ResetAction()
            experiment.DeployAction()
        end
    end
end

function printexperiments()
    local experiments=findmodules("ModuleScienceExperiment")
    for experiment in experiments do
        print(experiment.experiment.experimentTitle)
    end
end

function autopara()
    local parachutes=findmodules("ModuleParachute")
    local parachute=nil
    print("waiting for altitude < 8000")
    while altitude>8000 do
        sleep(1)
    end
    print("waiting for parachute safety")
    while #parachutes>0 do
        for i=#parachutes,1,-1 do
            parachute=parachutes[i]
            if tostring(parachute.deploymentSafeState)=="SAFE" then
                print("Deploying "..tostring(parachute))
                parachute.DeployAction()
                table.remove(parachutes,i)
            end
        end
        sleep(1)
    end
end

function basiclaunch()
    countdown()

    autopilot.pitch=90
    autopilot.heading=0
    ship.throttle=1
    
    stage()
    
    print('staged')
    waitforfalling()
    print('falling')
    
    
    --print('running experiments')
    --runexps()
    
    stage()
    
    print('waiting for parachutes')
    autopara()
    print('parachutes deployed')
    
    autopilot.reset()
end