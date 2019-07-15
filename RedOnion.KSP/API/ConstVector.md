## ConstVector

Read-only 3D vector / coordinate, base class for `Vector`, used for Vector.zero and other constants. Can also be used for properties.

- `Native`: Vector3d - Native Vector3d(`double x, y, z`).
- `X`: Double - The X-coordinate
- `Y`: Double - The Y-coordinate
- `Z`: Double - The Z-coordinate
- `Size`: Double - Size of the vector - `sqrt(x*x+y*y+z*z)`.
- `Magnitude`: Double - Alias to size of the vector - `sqrt(x*x+y*y+z*z)`.
- `SquareSize`: Double - Square size of the vector - `x*x+y*y+z*z`.
- `Normalized`: [Vector](Vector.md) - Get normalized vector (size 1).
- `Vector3`: Vector3 - Native UnityEngine.Vector3 (`float x,y,z`).
- `Vector2`: Vector2 - Native UnityEngine.Vector2 (`float x,y`).
- `[index]`: Double - Index the coordinates as double[3]
