# Planned Features
- New user-centric system for managing scripts. Will be a file-selection dialog or the completion area will do this.
- Editor/Repl reimplementation in new library. Hopefully allowing multiple editors/repls open at a time.
- Ability to run multiple scripts at a time and a gui to manage them.
- Lots of new user interfaces for various features built on new gui library.
- Ingame testing framework.
- Ingame debugging.
- Provide editors that can be used for editing files in contexts unrelated to our scripting engines.
- More UI library features.

# Next Release
Just had a release!

# Current
# 0.4
## 0.4.0
### General Changes:
- Updated for KSP 1.8.1
- Automatic documentation for our Common API
- In general much better documentation.
- Better Autopilot.

### LiveRepl Changes:
- Completing the Scriptname Input Area with a file name now loads the related file automatically
- LiveRepl can be dragged while executing a file.
- LiveRepl Overhaul: Much simpler UI code.
- Scriptname Input Area now uses a TextField instead of TextArea so that newlines are automatically disallowed.
- Tabs removed. They were kinda clunky. The new way the Scriptname Input Area works makes it less necessary.
- Clicking Scriptname Input Area now empties it so you can easily click on completion area to select the scriptname you want. And it automatically loads that script into the editor when you select it.
- Loading a file automatically selects the appropriate engine based on the extension
- Saving when  Scriptname Input Area is empty, now creates an "untitled.X" where X is the current engines proper extension.
- You can now set the font that will be used for the Repl, Output Area, and Editor. All other text uses the default font.

### ROS Changes
- ROS redesign. Allowing pause and continue features (something similar to the way the Lua example code uses coroutine.yield())

### Kerbalua Changes
- Removed Kerbalua specific global `import`. It is now available as the global `native` to be consistent with ROS.
- Removed auto conversions from function to CLR Actions and CLR Funcs. Reason is that the execution system doesn't allow script code sent into the CLR to yield in the normal way. This affects attempts to use functions like CLR's `List.Foreach`. Using foreach on a large collection is not interruptible. Any calls to CLR code are not interruptible so they must finish quickly, and in general `List.Foreach` will not.

[Old Versions (0.3.3 and older)](https://evandisoft.github.io/RedOnion/OldChangeLog)