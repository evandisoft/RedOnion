# Note: Most of this doc is obsolete! These classes should be accessed via import in Lua, or reflect in ros, if possible.

# Common Scripting API for Lua and RedOnionScript (ROS)
Here is some information about the API that we provide. In the future a lot of the content in these docs will be automated, but for now we will try to keep documentation manually caught up with at least some of the features we have added. But some features may be out of date or improperly maintained until we have an automatic documentation system.

Note: As we are very early in the process of figuring out all that we can do with this system, this API may change dramatically. We are also going to try to make the API more consistent between the two engines. We are very heavily involved in exploring what we can do with this at the moment and a lot of this can be considered a prototype.

WE ARE PROVIDING ACCESS TO POWERFUL KSP FEATURES:
MANY WILL LEAD TO ERRORS IN ONE OR MORE OF THE SCRIPTING ENGINES.

This is a WIP, and as time goes on we will develop a more robust system.

# KSP
We are exposing various objects/types to our Scripting engines. Some are defined in KSP or UnityEngine, and some are objects/types we created ourselves that are useful for interacting with the game. These objects/types have been added inside an object/namespace that is a global in both scripting languages.

For Lua, this global is spelled "Ksp", and for ROS (though ros is case insensitive) it is spelled "KSP".

For Lua, we must expose instantiated objects, while for ROS we can expose types or instantiated objects.

All of the Following Objects/Types are exposed under the "Ksp" or "KSP" namespace in Lua or ROS or both.

## FlightControl
This an experimental autopilot we have developed. To use with ROS, at the moment, you call GetInstance to get the instance.

ROS: 
```
var ctrl=KSP.FlightControl.GetInstance()
```

Lua: 
```
ctrl=Ksp.FlightControl
```

From there you can call `ctrl.StopSpin()` to put the autopilot in
STOP_SPIN mode.

`ctrl.SetSpin(vec)` Sets the spin to a particular Vector where the xyz components are used as follows: x means pitch, y means roll, and z means yaw.

To set the ship to always point at a particular direction, use
`ctrl.TargetDir=vec`. This is using the raw coordinate system under the hood and a better function will be implemented to replace this in the future.

To stop the autopilot you can click "Kill Ctrl" button in the repl, or call `ctrl.Shutdown()`.

Autopilot currently doesn't handle control surfaces well.
It's a WIP.

## FlightGlobals
Here we are exposing KSP's [FlightGlobals](https://kerbalspaceprogram.com/api/class_flight_globals.html) class. This is just used as a static class, and those are accessed similarly in Lua and ROS.

From flight globals you can get a reference to the `ActiveVessel` which
allows you to interact directly with the very vessel you are flying.

`ActiveVessel` is null when you are not in a flight scene.

You can get access to a lot of different gameplay objects from this class.

## HighLogic
This is KSP's [HighLogic](https://kerbalspaceprogram.com/api/class_high_logic.html) class, which is also used as a static class.

One thing you can do with this, is get access to the [CurrentGame](https://kerbalspaceprogram.com/api/class_game.html), and use that to get a reference to the current [CrewRoster](https://kerbalspaceprogram.com/api/class_kerbal_roster.html).

## EditorLogic
This is KSP's [EditorLogic](https://kerbalspaceprogram.com/api/class_editor_logic.html) class. This is not used as a static class so it's accessed differently in Lua and ROS at the moment.

For Lua, you just access `editor=Ksp.EditorLogic` to get the instance, while in ROS you do `var editor=KSP.EditorLogic.fetch`.

The EditorLogic instance will be null unless you are in the VAB or SPH.

I'm thinking a lot about what I can do with this class. One thing it allows you to do is get a reference to the ship that is being currently built: `editor.ship`.

from this you can iterate over the parts (ROS):
```
var vessel=ksp.EditorLogic.fetch.ship

for var part in vessel.parts
    for var module in part.modules
        print module
```
Lua:
```
--(Lua Moonsharp seems to have an issue with nested generic for-loops so I have to use each or weird things happen.)
editor=Ksp.EditorLogic
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
```

## ShipConstruction
Here we are exposing KSP's [ShipConstruction](https://kerbalspaceprogram.com/api/class_ship_construction.html) class, which is used as a static class.

There are a lot of interesting capabillities that seem to be accessible from this class. I don't know much about it at the moment.

Possibly only useful in the VAB/SPH

## PartLoader
Here we are exposing KSP's [PartLoader](https://kerbalspaceprogram.com/api/class_part_loader.html) class. There are static methods which are available and also there is `KSP.PartLoader.Instance.loadedParts` which lists all the parts that have been loaded by the game (I think).

## FlightDriver
Here we are exposing KSP's [FlightDriver](https://kerbalspaceprogram.com/api/class_flight_driver.html)

One thing I know that this can do is allow you to programmatically switch to a different vessel.

The following code prints out all the vessels and their associated index:

ROS:
```
var vessels=ksp.FlightGlobals.Vessels

for var i=0;i<vessels.count;i++
    print i+":"+vessels[i]
```
Then you can pick an `index` from the printed list and use that `index` to switch to the associated vessel using `FlightDriver` and `HighLogic`.

```
Ksp.FlightDriver.StartAndFocusVessel(Ksp.HighLogic.CurrentGame,index)
```

## StageManager
KSP's [StageManager](https://kerbalspaceprogram.com/api/class_k_s_p_1_1_u_i_1_1_screens_1_1_stage_manager.html) class. You use the static method `ActivateNextStage()` to ... well... activate the next stage.

This is not purely a static class. I believe you get the instance with the `Instance` static method. And though I haven't used the instance, it looks like it allows you to modify the staging order.

## Vec
This is a little helper class with some static methods to help with
vector math.

Partwise vector division:
`Ksp.Vec.Div(vec1,vec2)`

Partwise vector absolute value:
`Ksp.Vec.Abs(vec)`

Instantiate new vector (currently needed for Lua):
`Ksp.Vec.New(x,y,z)` and `Ksp.Vec.New()`

## Mathf
UnityEngine's [Mathf](https://docs.unity3d.com/ScriptReference/Mathf.html)

## Time
UnityEngine's [Time](https://docs.unity3d.com/ScriptReference/Time.html)

Repl/Editor scripts are ran during the FixedUpdate.
You can use Time.deltaTime to ensure accurate physics
calculations.

Example (Lua):
```
vessel=Ksp.FlightGlobals.ActiveVessel
ctrl=Ksp.FlightControl
local previousTime=os.time()
local ctrlTable={roll=0,pitch=0,yaw=0}

while true do
    print(vessel.ctrlState.GetPYR())

    torque=ctrl.GetAvailableTorque(vessel)

    local angularVelocity=Ksp.Vec.Div(vessel.angularMomentum,vessel.MOI)
    local maxAccel=Ksp.Vec.Div(torque,vessel.MOI)
    local input=Ksp.Vec.Div(angularVelocity,maxAccel*Ksp.Time.deltaTime)
    
    ctrlTable.pitch=input.x
    ctrlTable.roll=input.y
    ctrlTable.yaw=input.z
    
    ctrl.SetWithTable(ctrlTable)
    
    if(angularVelocity.magnitude<0.01) then
        ctrl.Shutdown()
        print("finished!")
        break
    end
    
    coroutine.yield()
end
```


## Random
UnityEngine's [Random](https://docs.unity3d.com/ScriptReference/Mathf.html)

