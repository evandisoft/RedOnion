## ConstVector

Read-only 3D vector / coordinate, base class for `Vector`, used for Vector.zero and other constants. Can also be used for properties.

- `native`: Vector3d - Native Vector3d(`double x, y, z`).
- `x`: Double - The X-coordinate
- `y`: Double - The Y-coordinate
- `z`: Double - The Z-coordinate
- `size`: Double - Size of the vector - `sqrt(x*x+y*y+z*z)`.
- `magnitude`: Double - Alias to size of the vector - `sqrt(x*x+y*y+z*z)`.
- `squareSize`: Double - Square size of the vector - `x*x+y*y+z*z`.
- `normalized`: [Vector](Vector.md) - Get normalized vector (size 1).
- `vector3`: Vector3 - Native UnityEngine.Vector3 (`float x,y,z`).
- `vector2`: Vector2 - Native UnityEngine.Vector2 (`float x,y`).
- `[index]`: Double - Index the coordinates as double[3]
