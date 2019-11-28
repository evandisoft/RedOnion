## Element

**Derived:** [Simple](Simple.md)

`UI.Element` is the base class for all UI elements / controls. It manages `UnityEngine.GameObject`and its `RectTransform`, provides layout settings and basic `AddElement` to add child elements.


**Instance Properties:**
- `Parent`: Element - Parent element (inside which this element is).
- `Tag`: Object - Tag for general usage.
- `Name`: string - Optional name of the element/control. Returns type name if not assigned (null).
- `Active`: bool - Element is set to be visible/active. [`GameObject.activeSelf`](https://docs.unity3d.com/ScriptReference/GameObject-activeSelf.html), [`GameObject.SetActive`](https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html)
- `Visible`: bool - Element is visible (and all parents are). [`GameObject.activeInHierarchy`](https://docs.unity3d.com/ScriptReference/GameObject-activeInHierarchy.html), [`GameObject.SetActive`](https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html)
- `Width`: float - Current width, redirects to PreferWidth when assigning.
- `Height`: float - Current height, redirects to PreferHeight when assigning.
- `MinWidth`: float - Minimal width if set to non-negative number (reads `float.NaN` otherwise, which means that the minimal width is not set - assigning negative number will have same result).
- `MinHeight`: float - Minimal height (same negative/`float.NaN` logic as above and for many below).
- `PreferWidth`: float - Preferred width if set (the layout will use this if possible).
- `PreferHeight`: float - Preferred height if set (the layout will use this if possible).
- `FlexWidth`: float - Flexible width if inside horizontal/vertical layout.
- `FlexHeight`: float - Flexible height if inside horizontal/vertical layout.
