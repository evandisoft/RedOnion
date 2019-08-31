## Bodies

String-keyed read-only dictionary that exposes its values as properties.
The dictionary is filled by the scripting engine.
Both the properties and indexed values will first try exact case match (even in ROS),
then (if exact match not found) try insensitive match where UPPER is preferred
(`SomeThing` will match `Something` if there is `Something` and `something`).
Can be used as base class for list of celestial bodies,
discovered assemblies, namespaces, classes etc.

