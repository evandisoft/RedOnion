## Vector (Function)

Function to create 3D vector / coordinate. Receives either three arguments (x,y,z), two (x,y; z=0), or one (x=y=z). Can also convert array / list of numbers (`V([1,2,3])` becomes `V(1,2,3)`).


**Types:**
- `Draw`: [VectorCreator.Draw](VectorCreator.Draw.md)

**Static Fields:**
- `zero`: [Vector](Vector.md) - Vector(0, 0, 0).
- `one`: [Vector](Vector.md) - Vector(1, 1, 1).
- `forward`: [Vector](Vector.md) - Vector(0, 0, 1).
- `fwd`: [Vector](Vector.md) - Alias to forward - Vector(0, 0, 1).
- `back`: [Vector](Vector.md) - Vector(0, 0, -1).
- `up`: [Vector](Vector.md) - Vector(0, 1, 0).
- `down`: [Vector](Vector.md) - Vector(0, -1, 0).
- `left`: [Vector](Vector.md) - Vector(-1, 0, 0).
- `right`: [Vector](Vector.md) - Vector(1, 0, 0).
- `none`: [Vector](Vector.md) - Vector(nan, nan, nan).

**Static Methods:**
- `cross()`: [Vector](Vector.md), a [Vector](Vector.md), b [Vector](Vector.md)
  - Cross product.
- `crs()`: [Vector](Vector.md), a [Vector](Vector.md), b [Vector](Vector.md)
  - Cross product. (Alias to cross)
- `scale()`: [Vector](Vector.md), a [Vector](Vector.md), b [Vector](Vector.md)
  - Scale vector by vector. Per axis. Multiplication does the same.
- `shrink()`: [Vector](Vector.md), a [Vector](Vector.md), b [Vector](Vector.md)
  - Shrink vector by vector. Per axis. Division does the same.
- `abs()`: [Vector](Vector.md), v [Vector](Vector.md)
  - Vector with coordinates changed to non-negative.
- `dot()`: double, a [Vector](Vector.md), b [Vector](Vector.md)
  - Dot product.
- `angle()`: double, a [Vector](Vector.md), b [Vector](Vector.md)
  - Angle between vectors (0..180).

## Vector (Instance)

3D vector / coordinate. All the usual operators were implemented,
multiplication and division can use both vector (per-axis) and number (all-axes).
Beware that multiplication is scaling, not cross product or dot - use appropriate function for these.


**Instance Properties:**
- `native`: Vector3d - Native KSP `Vector3d` (`double x, y, z`).
- `x`: double - The X-coordinate
- `y`: double - The Y-coordinate
- `z`: double - The Z-coordinate
- `size`: double - Size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.
- `magnitude`: double - Alias to size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.
- `squareSize`: double - Square size of the vector - `x*x+y*y+z*z`. Scale if setting.
- `normalized`: Vector - Get normalized vector (size 1).
- `Vector3`: Vector3 - UnityEngine.Vector3 (`float x,y,z`).
- `Vector2`: Vector2 - UnityEngine.Vector2 (`float x,y`).

**Instance Methods:**
- `dot()`: double, rhs Vector
  - Dot product of this vector and another vector.
- `angle()`: double, rhs Vector
  - Angle between this vector and another vector (0..180).
- `angle()`: double, rhs Vector, axis Vector
  - Angle between this vector and another vector given point above the plane (-180..180). Note that the vectors are not projected onto the plane, the angle of cross product of the two and the third vector being above 90 makes the result negative.
- `cross()`: Vector, rhs Vector
  - Cross product of this vector with another vector. Note that Unity uses left-handed coordinate system, so `ship.away.cross(ship.velocity)` points down (towards south pole) in prograde-orbit (which is the usual).
- `projectOnVector()`: Vector, normal Vector
  - Project this vector onto another vector.
- `projectOnPlane()`: Vector, normal Vector
  - Project this vector onto plane specified by normal vector.
- `project()`: Vector, normal Vector
  - Project this vector onto another vector (alias to `projectOnVector`).
- `exclude()`: Vector, normal Vector
  - Project this vector onto plane specified by normal vector (alias to `projectOnPlane`).
- `rotate()`: Vector, angle double, axis Vector
  - Rotate vector by an angle around axis.
