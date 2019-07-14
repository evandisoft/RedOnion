## Vector

Function to create 3D vector / coordinate, also aliased as simple `V`.
Receives either three arguments (x,y,z), two (x,y - z=0), or one (x=y=z).
Can also convert array / list of numbers (`V([1,2,3])` becomes `V(1,2,3)`).

- `zero`: Vector - Vector(0, 0, 0).
- `one`: Vector - Vector(1, 1, 1).
- `forward`: Vector - Vector(0, 0, 1).
- `fwd`: Vector - Alias to forward - Vector(0, 0, 1).
- `back`: Vector - Vector(0, 0, -1).
- `up`: Vector - Vector(0, 1, 0).
- `down`: Vector - Vector(0, -1, 0).
- `left`: Vector - Vector(-1, 0, 0).
- `right`: Vector - Vector(1, 0, 0).
- `cross()`: Vector - Cross product.
- `crs()`: Vector - Cross product. (Alias to cross.)
- `dot()`: Vector - Dot product.
- `abs()`: Vector - Vector with coordinates changed to non-negative.
- `angle()`: Vector - Angle between vectors (0..180).

## Vector

3D vector / coordinate. All the usual operators were implemented,
multiplication and division can use both vector (per-axis) and number (all-axes).
Beware that multiplication is scaling, not cross product or dot - use appropriate function for these.

- `native`: Vector3d - Native Vector3d (`double x,y,z`).
- `vector3`: Vector3 - Native UnityEngine.Vector3 (`float x,y,z`).
- `vector2`: Vector2 - Native UnityEngine.Vector2 (`float x,y`).
- `x`: double - The X-coordinate
- `y`: double - The Y-coordinate
- `z`: double - The Z-coordinate
- `scale()`: void - Scale the vector by a factor (all axes if number is provided, per-axis if Vector). Multiplication does the same.
- `shrink()`: void - Shrink the vector by a factor (all axes if number is provided, per-axis if Vector). Division does the same.
- `size`: double - Size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.
- `magnitude`: double - Alias to size of the vector - `sqrt(x*x+y*y+z*z)`. Scale if setting.
- `squareSize`: double - Square size of the vector - `x*x+y*y+z*z`. Scale if setting.
- `normalized`: Vector - Get normalized vector (size 1).
