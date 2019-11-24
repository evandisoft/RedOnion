## Script Won't work:
Make sure that if you are trying to execute Lua code, the Lua engine is selected. Click the "Lua" button to select that engine.
If you are trying to execute ROS code, ensure RedOnionScript engine is selected. Click the "ROS" button to select that engine.

The Repl will use whatever engine is currently selected. The currently selected engine is shown by a label `Engine: EngineName` above the `ROS` and `Lua` buttons.

The editor will run the engine associated with a particular script name extension. If your script ends with ".lua" it will run the Lua engine, and if your script ends with ".ros" it will run the ros engine.

## How do I run a script?
Click the `Run` button when a script is in the editor, or enter some code into the repl input area.

## Example programs don't run
We bundle our example programs in Scripts.zip and they are only used if you do not also have a program of the same name in the Scripts folder. If you modify any of the example programs and save them, you will not overwrite a bundled file in Scripts.zip, but will create a new file of the same name in the Scripts folder.

So, if you modified any of the exaple files, you won't see the newer versions when you update to a new version of RedOnion. Just delete any scripts in the Scripts folder that are also found in Scripts.zip.

## For anything else
Make an issue [here on github](https://github.com/evandisoft/RedOnion/issues), or ask in [this thread](https://forum.kerbalspaceprogram.com/index.php?/topic/183050-redonion-033-unrestricted-in-game-scripting-has-repl-editor-and-intellisense-lua-and-a-custom-jsruby-like-language-implemented-tested-on-ksp-17/&tab=comments#comment-3566618)
