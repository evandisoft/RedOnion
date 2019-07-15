## Vector (Function)

Function to create 3D vector / coordinate, also aliased as simple `V`. Receives either three arguments (x,y,z), two (x,y - z=0), or one (x=y=z). Can also convert array / list of numbers (`V([1,2,3])` becomes `V(1,2,3)`).

- `Zero`: [ConstVector](ConstVector.md) - Vector(0, 0, 0).
- `One`: [ConstVector](ConstVector.md) - Vector(1, 1, 1).
- `Forward`: [ConstVector](ConstVector.md) - Vector(0, 0, 1).
- `Fwd`: [ConstVector](ConstVector.md) - Alias to forward - Vector(0, 0, 1).
- `Back`: [ConstVector](ConstVector.md) - Vector(0, 0, -1).
- `Up`: [ConstVector](ConstVector.md) - Vector(0, 1, 0).
- `Down`: [ConstVector](ConstVector.md) - Vector(0, -1, 0).
- `Left`: [ConstVector](ConstVector.md) - Vector(-1, 0, 0).
- `Right`: [ConstVector](ConstVector.md) - Vector(1, 0, 0).
- `Cross()`: [Vector](Vector.md), a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Cross product.
- `Crs()`: [Vector](Vector.md), a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Cross product. (Alias to cross)
- `Dot()`: [Vector](Vector.md), a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Dot product.
- `Abs()`: [Vector](Vector.md), v [ConstVector](ConstVector.md)
  - Vector with coordinates changed to non-negative.
- `Angle()`: Double, a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Angle between vectors (0..180).

## Vector (Instance)

3D vector / coordinate. All the usual operators were implemented, multiplication and division can use both vector (per-axis) and number (all-axes). Beware that multiplication is scaling, not cross product or dot - use appropriate function for these. See also [ConstVector.md](ConstVector) which represents read-only base class.

- `Normalized`: Vector - Get normalized vector (size 1).
- `Native`: Vector3d - Native Vector3d(`double x, y, z`).
- `X`: Double - The X-coordinate
- `Y`: Double - The Y-coordinate
- `Z`: Double - The Z-coordinate
- `Size`: Double - Size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.
- `Magnitude`: Double - Alias to size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.
- `SquareSize`: Double - Square size of the vector - `x*x+y*y+z*z`. Scale if setting.
- `Vector3`: Vector3 - Native UnityEngine.Vector3 (`float x,y,z`).
- `Vector2`: Vector2 - Native UnityEngine.Vector2 (`float x,y`).
- `[index]`: Double - Index the coordinates as double[3]
- `Normalize()`: Void - Normalize vector (set size to 1).
- `Scale()`: Void, factor Double
  - Scale the vector by a factor (all axes). Multiplication does the same.
- `Scale()`: Void, v [ConstVector](ConstVector.md)
  - Scale individual axis. Multiplication does the same.
- `Shrink()`: Void, factor Double
  - Shrink the vector by a factor (all axes). Division does the same.
- `Shrink()`: Void, v [ConstVector](ConstVector.md)
  - Shrink individual axis. Division does the same.
