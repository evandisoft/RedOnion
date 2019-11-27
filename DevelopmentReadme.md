## Development Philosophy:
Some of my (Evan) code may appear a bit sloppy at times. Short reason is [opportunity cost](https://en.wikipedia.org/wiki/Opportunity_cost). Long rambling explanation is [here](DevDocs/EvansDevelopmentPhilosophy.md). If there is some desire to work with some of my code that is a bit ugly, let me know, and I will set aside some time to make it nicer.

Once we have more people working on the project, I will put more focus on making sure that all of my code is more maintainable, but right now it is a waste of time.

## Sub-Projects
The **RedOnion** project is divided into multiple sub-projects:
- [Kerbalua](Kerbalua/Development.md) - A MoonSharp-based [Lua scripting engine](Kerbalua/README.md).
- **KerbaluaNUnit** - Tests for the **Kerbalua** project.
- [Kerbalui](Kerbalui/DevReadme.md) - An [imgui](https://docs.unity3d.com/2019.3/Documentation/Manual/GUIScriptingGuide.html)-based ui lib used to create **LiveRepl**. Not designed to be used by as a general ui library. Only implemented what I needed for **LiveRepl** and left it to be refactored later if it is ever intended to be used as a general ui lib (no plans for this).
- [Kerbnlua](Kerbnlua/Kerbnlua/KerbnluaDevNotes.md) - An experimental Lua scripting engine based on [NLua](https://github.com/NLua). I'm developing this in a different branch, `nlua-experiments`. It is disabled in the main development branches.
- [LiveRepl](LiveRepl/DevReadme.md) - The main GUI. It's `LiveReplMain` class extends MonoBehaviour, manages this project's toolbar button, and handles UnityEngine's Update, FixedUpdate, and OnGUI calls.
- **RedOnion.Build** - The project that builds releases and documentation.
- **RedOnion.KSP** - Contains features used by both **ROS** and **Kerbalua**, including the [Common API](RedOnion.KSP/API/Globals.md)
- **RedOnion.KSP.Tests** - Tests specific to the **RedOnion.KSP** project.
- **RedOnion.ROS** - The Red Onion Script custom language.
- **RedOnion.ROS.Tests** - Tests for **RedOnion.ROS**
- **RedOnion.Shell** - A shell where RedOnionScript can be ran independently of KSP.
- **RedOnion.UI** - A ui lib that uses [Unity UI](https://docs.unity3d.com/2019.3/Documentation/Manual/UISystem.html). Can be used in scripting.

The two orignal developers on this project, Evan and Firda (Lukáš Fireš) are generally responsible for different sub-projects. For example, Firda is responsible for most of the "RedOnion.X" projects. The exception is "RedOnion.KSP", which is our sub-project for things that bring our scripting language sub-projects together.


## Sub-Project Reference Structure
Since C# doesn't allow cyclic dependencies, this structure is important for figuring out where to implement what.

```
LiveRepl
    RedOnion.KSP
        RedOnion.ROS
        RedOnion.UI
    RedOnion.ROS
    RedOnion.UI
    Kerbalua
        RedOnion.KSP
        RedOnion.ROS
        RedOnion.UI
    Kerbalui
        RedOnion.KSP
        RedOnion.ROS
```

In other words, LiveRepl references RedOnion.KSP, RedOnion.ROS, etc.

Kerbalua references RedOnion.KSP, RedOnion.ROS, etc.