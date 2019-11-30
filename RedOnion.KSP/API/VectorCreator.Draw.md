## VectorCreator.Draw

Vector drawing


**Instance Properties:**
- `reference`: ISpaceObject - Reference for coordinate system (origin at zero if null).
- `origin`: [Vector](Vector.md) - Starting point of the vector (relative to reference).
- `direction`: [Vector](Vector.md) - Direction of the vector (from starting point).
- `from`: [Vector](Vector.md) - Alias to `origin`.
- `to`: [Vector](Vector.md) - End point (relative to reference, not starting point).
- `start`: [Vector](Vector.md) - Alias to `origin`.
- `vector`: [Vector](Vector.md) - Alias to `direction`.
- `color`: Color - Color of the arrow.
- `width`: double - Width / thickness of the arrow.
- `scale`: double - Scale of the vector.

**Instance Methods:**
- `show()`: void - Show the vector. It is created hidden so that you can subscribe to `system.update` first.
- `hide()`: void - Hide the vector.
