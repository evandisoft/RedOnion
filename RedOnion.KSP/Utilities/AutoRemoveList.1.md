## AutoRemoveList.1

Designed for events with auto-remove on process shutdown, but can be used with any type of elements. Removing elements during enumeration is allowed (for current element of the enumerator).


**Types:**
- `Subscription`: [AutoRemoveList.1.Subscription](AutoRemoveList.1.Subscription.md)
- `AutoSubscription`: [AutoRemoveList.1.AutoSubscription](AutoRemoveList.1.AutoSubscription.md)

**Instance Properties:**
- `count`: int - Number of subscription.
- `subscriptions`: IEnumerable\[[AutoRemoveList.1](AutoRemoveList.1.Subscription.md)\[T\]\] - Enumerate all subscriptions.

**Instance Methods:**
- `subscribe()`: [AutoRemoveList.1](AutoRemoveList.1.AutoSubscription.md)\[T\], value T
  - Subscribe to the list. Similar to `add` but returns auto-remove subscription. This is default member and will be used when you call the object.
- `add()`: [AutoRemoveList.1](AutoRemoveList.1.Subscription.md)\[T\], value T
  - Add new item. Returns pure subscribtion (or null for duplicit item). Can also be accessed via `+=` operator in ROS.
- `remove()`: [AutoRemoveList.1](AutoRemoveList.1.Subscription.md)\[T\], value T
  - Remove item. Returns the subscription on success, null if not found. Can also be accessed via `-=` operator in ROS.
- `clear()`: void - Remove all subscriptions.
