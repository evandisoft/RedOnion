## ConstVector

Read-only 3D vector / coordinate, base class for `Vector`,
 used for Vector.zero and other constants. Can also be used for properties.
NOTE: Subject to change - may revert to `Vector3d` with custom descriptor.

- `native`: Vector3d - Native Vector3d(`double x, y, z`).
- `x`: Double - The X-coordinate
- `y`: Double - The Y-coordinate
- `z`: Double - The Z-coordinate
- `size`: Double - Size of the vector - `sqrt(x*x+y*y+z*z)`.
- `magnitude`: Double - Alias to size of the vector - `sqrt(x*x+y*y+z*z)`.
- `squareSize`: Double - Square size of the vector - `x*x+y*y+z*z`.
- `normalized`: [Vector](Vector.md) - Get normalized vector (size 1).
- `Vector3`: Vector3 - Native UnityEngine.Vector3 (`float x,y,z`).
- `Vector2`: Vector2 - Native UnityEngine.Vector2 (`float x,y`).
- `[index]`: Double - Index the coordinates as double[3]
- `dot()`: Double, rhs ConstVector
  - Dot product of this vector and another vector.
- `angle()`: Double, rhs ConstVector
  - Angle between this vector and another vector (0..180).
- `angle()`: Double, rhs ConstVector, axis ConstVector
  - Angle between this vector and another vector given point above the plane (-180..180). Note that the vectors are not projected onto the plane, the angle of cross product of the two and the third vector being above 90 makes the result negative.
- `cross()`: [Vector](Vector.md), rhs ConstVector
  - Cross product of this vector with another vector.
- `projectOnVector()`: [Vector](Vector.md), normal ConstVector
  - Project this vector onto another vector.
- `projectOnPlane()`: [Vector](Vector.md), normal ConstVector
  - Project this vector onto plane specified by normal vector.
- `project()`: [Vector](Vector.md), normal ConstVector
  - Project this vector onto another vector (alias to `projectOnVector`).
- `exclude()`: [Vector](Vector.md), normal ConstVector
  - Project this vector onto plane specified by normal vector (alias to `projectOnPlane`).
- `rotate()`: [Vector](Vector.md), angle Double, axis [Vector](Vector.md)
  - Rotate vector by an angle around axis.
