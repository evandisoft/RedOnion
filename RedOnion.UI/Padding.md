## Padding

Element's inner padding (4 values).


**Constructors:**
- `Padding()`: all float
  - Set all values to the one specified.
- `Padding()`: horizontal float, vertical float
  - Set `left = right = horizontal` and `top = bottom = vertical`.
- `Padding()`: left float, right float, top float, bottom float
  - Specify all the values.

**Instance Fields:**
- `left`: float - Padding on the left side.
- `right`: float - Padding on the right side.
- `top`: float - Padding above the content.
- `bottom`: float - Padding below the content.

**Instance Properties:**
- `All`: float - One value for all (if same or setting), `NaN` if not.
- `Horizontal`: float - Value of `left` and `right` if same, `NaN` if not.
- `Vertical`: float - value of `top` and `bottom` if same, `NaN` if not.
