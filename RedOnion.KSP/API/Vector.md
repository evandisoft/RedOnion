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
- `none`: [ConstVector](ConstVector.md) - Vector(nan, nan, nan).
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
- `dot()`: double, a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Dot product.
- `angle()`: double, a [ConstVector](ConstVector.md), b [ConstVector](ConstVector.md)
  - Angle between vectors (0..180).

## Vector (Instance)

3D vector / coordinate. All the usual operators were implemented,
multiplication and division can use both vector (per-axis) and number (all-axes).
Beware that multiplication is scaling, not cross product or dot - use appropriate function for these.
See also [ConstVector.md](ConstVector) which represents read-only base class.
NOTE: Subject to change - may revert to `Vector3d` with custom descriptor.

- `normalized`: Vector - Get normalized vector (size 1).
- `native`: Vector3d - Native Vector3d(`double x, y, z`).
- `x`: double - The X-coordinate
- `y`: double - The Y-coordinate
- `z`: double - The Z-coordinate
- `size`: double - Size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.
- `magnitude`: double - Alias to size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.
- `squareSize`: double - Square size of the vector - `x*x+y*y+z*z`. Scale if setting.
- `Vector3`: Vector3 - Native UnityEngine.Vector3 (`float x,y,z`).
- `Vector2`: Vector2 - Native UnityEngine.Vector2 (`float x,y`).
- `[index]`: double - Index the coordinates as double[3]
- `dot()`: double, rhs [ConstVector](ConstVector.md)
  - Dot product of this vector and another vector.
- `angle()`: double, rhs [ConstVector](ConstVector.md)
  - Angle between this vector and another vector (0..180).
- `angle()`: double, rhs [ConstVector](ConstVector.md), axis [ConstVector](ConstVector.md)
  - Angle between this vector and another vector given point above the plane (-180..180). Note that the vectors are not projected onto the plane, the angle of cross product of the two and the third vector being above 90 makes the result negative.
- `cross()`: Vector, rhs [ConstVector](ConstVector.md)
  - Cross product of this vector with another vector.
- `projectOnVector()`: Vector, normal [ConstVector](ConstVector.md)
  - Project this vector onto another vector.
- `projectOnPlane()`: Vector, normal [ConstVector](ConstVector.md)
  - Project this vector onto plane specified by normal vector.
- `project()`: Vector, normal [ConstVector](ConstVector.md)
  - Project this vector onto another vector (alias to `projectOnVector`).
- `exclude()`: Vector, normal [ConstVector](ConstVector.md)
  - Project this vector onto plane specified by normal vector (alias to `projectOnPlane`).
- `rotate()`: Vector, angle double, axis Vector
  - Rotate vector by an angle around axis.
- `normalize()`: void - Normalize vector (set size to 1).
- `scale()`: void, factor double
  - Scale the vector by a factor (all axes). Multiplication does the same.
- `scale()`: void, v [ConstVector](ConstVector.md)
  - Scale individual axis. Multiplication does the same.
- `shrink()`: void, factor double
  - Shrink the vector by a factor (all axes). Division does the same.
- `shrink()`: void, v [ConstVector](ConstVector.md)
  - Shrink individual axis. Division does the same.
