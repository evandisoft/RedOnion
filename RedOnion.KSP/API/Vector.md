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

## Vector

3D vector / coordinate

- `native`: Vector3d - Native Vector3d (convertible to UnityEngine.Vector3).
- `vector3`: Vector3 - Native UnityEngine.Vector3 (float).
- `vector2`: Vector2 - Native UnityEngine.Vector2 (float x,y).
- `scale()`: void - Scale the vector by a factor (all axes at once if number is provided, per-axis if Vector).
