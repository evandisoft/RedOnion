## Vector Function

Function to create 3D vector / coordinate, also aliased as simple `V`.

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
- `dot()`: Vector - Dot product.
- `abs()`: Vector - Vector with coordinates changed to non-negative.

## Vector

3D vector / coordinate. All the usual operators were implemented,
multiplication and dividion can use both vector (per-axis) and number (all-axes).
Beware that multiplication is scaling, not cross product or dot - use appropriate function for these.

- `native`: Vector3d - Native Vector3d (convertible to UnityEngine.Vector3).
- `vector3`: Vector3 - Native UnityEngine.Vector3 (float).
- `vector2`: Vector2 - Native UnityEngine.Vector2 (float x,y).
- `scale()`: void - Scale the vector by a factor (all axes if number is provided, per-axis if Vector). Multiplication does the same.
- `shrink()`: void - Shrink the vector by a factor (all axes if number is provided, per-axis if Vector). Division does the same.
- `size`: float - Size of the vector - sqrt(x*x+y*y+z*z). Scale if setting.
- `magnitude`: float - Alias to size of the vector - sqrt(x*x+y*y+z*z). Scale if setting.
- `squareSize`: float - Square size of the vector - x*x+y*y+z*z. Scale if setting.
- `normalized`: Vector - Get normalized vector (size 1).
