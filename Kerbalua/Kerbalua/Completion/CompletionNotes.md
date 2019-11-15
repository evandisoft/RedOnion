Static -> Obj

Completion always starts at globals, a table.
We can figure out the type from there.

Have a class that Contains each type that can complete.

Each such class should handle completion for itself.

Types:
    Only public members are available.
    Static -> This is a class used in a static context. 
        Field access results become an Instance.
        Property access results become InstanceStatic unless
        this type has a `ConstAttribute`
        Can have nested types.
        
    Instance -> This is an actual object instance.
        We complete as if it is an instance, because it is.
        Field access results become an instance.
        Property access results become InstanceStatic unless
        this type has a `ConstAttribute`
        
    InstanceStatic -> This is a class we cannot get the
        instance of, but have to complete as if it were an instance based on types.
        When this is a type that is ICompletable or IHasCompletionProxy we have to stop since
        it is not supposed to complete by any means other than ICompletable
        
    Table -> 
    ICompletable -> Lets the object decide how to complete
    IHasCompletionProxy -> Gets converted to one of these types
    DynValue -> Gets converted to one of these types