## Vector (Function)

Function to create 3D vector / coordinate, also aliased as simple `V`. Receives either three arguments (x,y,z), two (x,y - z=0), or one (x=y=z). Can also convert array / list of numbers (`V([1,2,3])` becomes `V(1,2,3)`).

- `zero`: [ConstVector](ConstVector.md) - Vector(0, 0, 0).
- `one`: [ConstVector](ConstVector.md) - Vector(1, 1, 1).
- `forward`: [ConstVector](ConstVector.md) - Vector(0, 0, 1).
- `fwd`: [ConstVector](ConstVector.md) - Alias to forward - Vector(0, 0, 1).
- `back`: [ConstVector](ConstVector.md) - Vector(0, 0, -1).
- `up`: [ConstVector](ConstVector.md) - Vector(0, 1, 0).
- `down`: [ConstVector](ConstVector.md) - Vector(0, -1, 0).
- `left`: [ConstVector](ConstVector.md) - Vector(-1, 0, 0).
- `right`: [ConstVector](ConstVector.md) - Vector(1, 0, 0).
- `cross()`: [Vector](Vector.md), a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Cross product.
- `crs()`: [Vector](Vector.md), a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Cross product. (Alias to cross)
- `scale()`: [Vector](Vector.md), a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Scale vector by vector. Per axis. Multiplication does the same.
- `shrink()`: [Vector](Vector.md), a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Shrink vector by vector. Per axis. Division does the same.
- `abs()`: [Vector](Vector.md), v [ConstVector](ConstVector.md)
  - Vector with coordinates changed to non-negative.
- `dot()`: Double, a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Dot product.
- `angle()`: Double, a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Angle between vectors (0..180).

## Vector (Instance)

3D vector / coordinate. All the usual operators were implemented, multiplication and division can use both vector (per-axis) and number (all-axes). Beware that multiplication is scaling, not cross product or dot - use appropriate function for these. See also [ConstVector.md](ConstVector) which represents read-only base class.

- `normalized`: Vector - Get normalized vector (size 1).
- `native`: Vector3d - Native Vector3d(`double x, y, z`).
- `x`: Double - The X-coordinate
- `y`: Double - The Y-coordinate
- `z`: Double - The Z-coordinate
- `size`: Double - Size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.
- `magnitude`: Double - Alias to size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.
- `squareSize`: Double - Square size of the vector - `x*x+y*y+z*z`. Scale if setting.
- `Vector3`: Vector3 - Native UnityEngine.Vector3 (`float x,y,z`).
- `Vector2`: Vector2 - Native UnityEngine.Vector2 (`float x,y`).
- `[index]`: Double - Index the coordinates as double[3]
- `normalize()`: Void - Normalize vector (set size to 1).
- `scale()`: Void, factor Double
  - Scale the vector by a factor (all axes). Multiplication does the same.
- `scale()`: Void, v [ConstVector](ConstVector.md)
  - Scale individual axis. Multiplication does the same.
- `shrink()`: Void, factor Double
  - Shrink the vector by a factor (all axes). Division does the same.
- `shrink()`: Void, v [ConstVector](ConstVector.md)
  - Shrink individual axis. Division does the same.
