**LiveRepl** is the main gui at the moment for the RedOnion Project. It is made using the [Kerbalui](../Kerbalui/DevReadme.md) library.

`LiveRepl/LiveReplMain.cs` contains the code that hides/shows the gui when the user clicks the **LiveRepl** button on the toolbar.

`LiveRepl/ScriptWindow` is the main window that the gui consists of. It is a partial class to divide some functionality up.
- ScriptWindow.Actions mostly contains functions that are ran by hotkeys or by button presses.
- ScriptWindow.Evaluation contains code that deals with the code evaluation system.
- ScriptWindow.KeyBindings contains global keybindings.
- ScriptWindow.Layout contains the code related to layout. But `LiveRepl/Parts` handles the code for the layout of the individual parts.

`LiveRepl/ScriptWindowParts` is a class that is used to store all the references to the parts of the UI. It is passed to each part that needs references to other parts for their functionality.

`LiveRepl/Parts` contains all the parts of the GUI. Each part is a **Kerbalui** object.

`LiveRepl/Interfaces` contains some interfaces used for the completion system.

`LiveRepl/Execution` contains some classes the ScriptWindow uses for evaluating code in script engines. The `LiveRepl/Execution/ReplEvaluator` represents a subclass for an adapter for a script engine in order to present the ScriptWindow with a simple interface for evaluating source code.

`LiveRepl/Decorators` contains `ScriptDisabledElement` which can wrap an Element up and hide it whenever a script is running. (which is what makes parts of the gui be greyed out when a script is running)

`LiveRepl/Completion` contains code for managing completion between the parts that use it and the CompletionArea, which shows the completion results.