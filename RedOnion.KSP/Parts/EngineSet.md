## EngineSet

Read-only set of engines.

- `AnyOperational`: Boolean - Whether any engine in the set is operational.
- `AllOperational`: Boolean - Whether all the engines in the set are operational.
- `AnyFlameout`: Boolean - Wheter any engine in the set flamed out.
- `AllFlameout`: Boolean - Wheter all engines in the set flamed out.
- `Count`: Int32 - Number of elements in the list (or set).
- `[index]`: [Engine](Engine.md) - Get element by index. Will throw exception if index is out of range.
- `Contains()`: Boolean, item [Engine](Engine.md)
  - Test wether the list (or set) contains specified element.
- `IndexOf()`: Int32, item [Engine](Engine.md)
  - Get index of element. -1 if not found.
