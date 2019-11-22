The **RedOnion** project is divided into multiple projects:
- [Kerbalua](Kerbalua/Development.md) - A MoonSharp-based [Lua scripting engine](Kerbalua/README.md).
- **KerbaluaNUnit** - Tests for the **Kerbalua** project.
- [Kerbalui](Kerbalui/Kerbalui/DevReadme.md) - An [imgui](https://docs.unity3d.com/2019.3/Documentation/Manual/GUIScriptingGuide.html)-based ui lib used to create **LiveRepl**
- [Kerbnlua](Kerbnlua/Kerbnlua/KerbnluaDevNotes.md) - An experimental Lua scripting engine based on [NLua](https://github.com/NLua). Only active in DEBUG builds.
- **LiveRepl** - The main GUI.
- **RedOnion.Build** - The project that builds releases and documentation.
- **RedOnion.KSP** - Contains features used by both **ROS** and **Kerbalua**, including the **CommonAPI**
- **RedOnion.KSP.Tests** - Tests specific to the **RedOnion.KSP** project.
- **RedOnion.OS** - Where we may implement a possible replacement for the current execution system.
- **RedOnion.ROS** - The Red Onion Script custom language.
- **RedOnion.ROS.Tests** - Tests for **RedOnion.ROS**
- **RedOnion.Shell** - A shell where RedOnionScript can be ran independently of KSP.
- **RedOnion.UI** - A ui lib that uses [Unity UI](https://docs.unity3d.com/2019.3/Documentation/Manual/UISystem.html). Intended to be used in scripting.

