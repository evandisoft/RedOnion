MoonSharp provides a useful way to iterate over dotnet collections.

```
for item in collection do
    -- something
end
```

Using the native list of ship parts from `ship.native.parts` we can iterate over each part and each module.

`ship` is a global that gies us the current ship if we are in Flight Mode, null otherwise.

So this script iterates and prints each part:

```
for part in ship.native.parts do
    print(part)
end
```

If you want to print all the modules as well you can access the `Modules` member of a part to get a list of the modules as in this script:

```
for part in ship.native.parts do
    for module in part.Modules do
        print(module)
    end
end
```

If you do that, you might see something like the following in the output.
```
o> mk1pod.v2 (ModuleCommand)
o> mk1pod.v2 (ModuleReactionWheel)
o> mk1pod.v2 (ModuleColorChanger)
o> mk1pod.v2 (ModuleScienceExperiment)
o> mk1pod.v2 (ModuleScienceContainer)
o> mk1pod.v2 (ModuleConductionMultiplier)
o> mk1pod.v2 (ModuleDataTransmitter)
o> mk1pod.v2 (ModuleLiftingSurface)
o> mk1pod.v2 (FlagDecal)
o> mk1pod.v2 (ModulePartVariants)
o> mk1pod.v2 (ModuleTripLogger)
o> fuelTank.long (ModulePartVariants)
o> liquidEngine2 (ModuleEngines)
o> liquidEngine2 (ModuleJettison)
o> liquidEngine2 (ModuleGimbal)
o> liquidEngine2 (FXModuleAnimateThrottle)
o> liquidEngine2 (ModuleAlternator)
o> liquidEngine2 (ModuleSurfaceFX)
o> liquidEngine2 (ModuleTestSubject)
o> liquidEngine2 (ModuleTestSubject)
o> Decoupler.1 (ModuleDecouple)
o> Decoupler.1 (ModuleToggleCrossfeed)
o> Decoupler.1 (ModuleTestSubject)
o> fuelTank.long (ModulePartVariants)
o> liquidEngine2 (ModuleEngines)
o> liquidEngine2 (ModuleJettison)
o> liquidEngine2 (ModuleGimbal)
o> liquidEngine2 (FXModuleAnimateThrottle)
o> liquidEngine2 (ModuleAlternator)
o> liquidEngine2 (ModuleSurfaceFX)
o> liquidEngine2 (ModuleTestSubject)
o> liquidEngine2 (ModuleTestSubject)
o> probeCoreHex.v2 (ModuleCommand)
o> probeCoreHex.v2 (ModuleReactionWheel)
o> probeCoreHex.v2 (ModuleSAS)
o> probeCoreHex.v2 (ModuleKerbNetAccess)
o> probeCoreHex.v2 (ModuleDataTransmitter)
o> probeCoreHex.v2 (ModuleTripLogger)
```

When printing a module what gets printed for each module is first the part name, and then the module name.

So when we printed the module `ModuleCommand` of the part `probeCoreHex.v2` in the loop,
```
o> probeCoreHex.v2 (ModuleCommand)
```
resulted.

Parts in KSP have multiple modules. Mods can add modules to existing parts, so if you run the script, it will produce a different output depending on which mods you have installed.