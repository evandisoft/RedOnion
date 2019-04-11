### Script Won't work:
Make sure that if you are trying to execute Lua code, the Lua engine is selected. Click the "Lua" button to select that engine.
If you are trying to execute ROS code, ensure RedOnionScript engine is selected. Click the "RedOnion" button to select that engine.

The Repl/Editor both use whatever engine is currently selected. The currently selected engine will be shown in the textbox below the RedOnion and Lua buttons. The Repl also remembers which engine you used last.

### Repl won't respond to input:
Repl/Editor is disabled when a program is running.
Try terminating the current program with ctrl+c

### How do I run a script?
Click the evaluate button when a script is in the editor. Ensure you have the [right engine selected](https://github.com/evandisoft/RedOnion/blob/master/TroubleShooting.md#script-wont-work).

### This mod might have bad interactions with other mods that use MoonSharp.
The Lua engine needs to modify many global MoonSharp settings to work properly, and that may modify MoonSharp settings used by other mods. Or other mods that use MoonSharp might modify some settings used by this mod.

**This will be fixed in the next release by using a custom compiled version of MoonSharp in a different assembly.**
