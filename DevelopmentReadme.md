## Development Philosophy:
The two orignal developers on this project, Evan and Firda (Lukáš Fireš), have different approaches to development.

Generally we are responsible for different sub-projects. For example, Firda is responsible for most of the "RedOnion.X" projects. The exception is "RedOnion.KSP", which is our sub-project for things that bring our scripting language sub-projects together.

Some of my (Evan) code may appear a bit sloppy at times. Here is why I do [this](DevDocs/EvansDevelopmentPhilosophy.md).

We are not extremely picky about multiple approaches coexisting in the same overall project.

## Project Structure

The **RedOnion** project is divided into multiple sub-projects:
- [Kerbalua](Kerbalua/Development.md) - A MoonSharp-based [Lua scripting engine](Kerbalua/README.md).
- **KerbaluaNUnit** - Tests for the **Kerbalua** project.
- [Kerbalui](Kerbalui/DevReadme.md) - An [imgui](https://docs.unity3d.com/2019.3/Documentation/Manual/GUIScriptingGuide.html)-based ui lib used to create **LiveRepl**
- [Kerbnlua](Kerbnlua/Kerbnlua/KerbnluaDevNotes.md) - An experimental Lua scripting engine based on [NLua](https://github.com/NLua). I'm developing this in a different branch, `nlua-experiments`. It is disabled in the main development branches.
- [LiveRepl](LiveRepl/DevReadme.md) - The main GUI.
- **RedOnion.Build** - The project that builds releases and documentation.
- **RedOnion.KSP** - Contains features used by both **ROS** and **Kerbalua**, including the [Common API](RedOnion.KSP/API/Globals.md)
- **RedOnion.KSP.Tests** - Tests specific to the **RedOnion.KSP** project.
- **RedOnion.ROS** - The Red Onion Script custom language.
- **RedOnion.ROS.Tests** - Tests for **RedOnion.ROS**
- **RedOnion.Shell** - A shell where RedOnionScript can be ran independently of KSP.
- **RedOnion.UI** - A ui lib that uses [Unity UI](https://docs.unity3d.com/2019.3/Documentation/Manual/UISystem.html). Can be used in scripting.

## Project Reference Structure
Since C# doesn't allow cyclic dependencies, this structure is important.

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