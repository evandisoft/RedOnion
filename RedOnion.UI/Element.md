## Element

**Derived:** [Simple](Simple.md), [Label](Label.md), [Button](Button.md), [TextBox](TextBox.md), [Scrollbar](Scrollbar.md)

`UI.Element` is the base class for all UI elements / controls. It manages `UnityEngine.GameObject` and its `RectTransform`, provides layout settings and basic `AddElement` to add child elements. All elements must ultimately be hosted inside [`UI.Window`](Window.md).


**Instance Properties:**
- `GameObject`: GameObject - \[`Unsafe`\] [Unity API](https://docs.unity3d.com/ScriptReference/GameObject.html) Game object of the content.
- `RootObject`: GameObject - \[`Unsafe`\] [Unity API](https://docs.unity3d.com/ScriptReference/GameObject.html) Root game object that will be added as a child when adding this element to another element. Same as `GameObject` for simple elements.
- `RectTransform`: RectTransform - \[`Unsafe`\] [Unity API](https://docs.unity3d.com/ScriptReference/RectTransform.html)RectTransform of `RootObject` (which is the same as `GameObject` for simple elements.
- `Parent`: Element - Parent element (inside which this element is).
- `Tag`: Object - Tag for general usage.
- `Name`: string - Optional name of the element/control. Returns type name if not assigned (null).
- `Active`: bool - Element is set to be visible/active. [`RootObject.activeSelf`](https://docs.unity3d.com/ScriptReference/GameObject-activeSelf.html), [`RootObject.SetActive`](https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html)
- `Visible`: bool - Element is visible (and all parents are). [`RootObject.activeInHierarchy`](https://docs.unity3d.com/ScriptReference/GameObject-activeInHierarchy.html), [`RootObject.SetActive`](https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html)
- `Width`: float - Current width, redirects to PreferWidth when assigning.
- `Height`: float - Current height, redirects to PreferHeight when assigning.
- `MinWidth`: float - Minimal width if set to non-negative number (reads `float.NaN` otherwise, which means that the minimal width is not set - assigning negative number will have same result).
- `MinHeight`: float - Minimal height (same negative/`float.NaN` logic as above and for many below).
- `PreferWidth`: float - Preferred width if set (the layout will use this if possible).
- `PreferHeight`: float - Preferred height if set (the layout will use this if possible).
- `FlexWidth`: float - Flexible width if inside horizontal/vertical layout.
- `FlexHeight`: float - Flexible height if inside horizontal/vertical layout.

**Static Methods:**
- `LoadIcon()`: Texture2D, width int, height int, path string
  - \[`Unsafe`\] Load icon of specified dimensions as `Texture2D` from a file (from `Resources` directory or `Resources.zip` ).
