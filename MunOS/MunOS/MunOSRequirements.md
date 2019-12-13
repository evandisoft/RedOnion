(some notes I made beforehand)

Interface for an engine.

should be able to run a script without connecting it to a ui 
    facilitate people using our scripting languages as a normal part of their mod.
    people should be able to just use munos and one or the other of our scripting languages, without using LiveRepl
    munos would manage execution for any use of our languages among potentially multiple mods.
    would bring us close to the goal of other people being able to write plugin functionality in our scripting languages.
    
    Mods could spawn a different execution manager (which they give a name) for their mod (but have one static os really managing things)
    so that that particular mod has access to a list of all their own processes without having access to other mods processes
    
    Have processes which do not persist across scene changes, as well as ones that do.
    
    Give munos a simple UI that shows the percentage of execution averaged over last 10 updates for the different
    execution managers.
    
    Tell users to use one execution manager per mod and name it for their mod.
    
    Allow a mod to remove their execution manager from executing, which removes all its processes.
    
    Require execution manager names to be unique

    Give the user, rather than the modmakers, the ability to kill any ExecutionManager and set the max execution time.
    
    Execution could be paused for a given execution manager, allowing someone to debug all execution for a particular mod.
    
    Associate stats with the processes so modders can display them how they like.