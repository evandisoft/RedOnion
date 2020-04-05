## AutoRemoveList.1

Designed for events with auto-remove on process shutdown, but can be used with any type of elements. Removing elements during enumeration is allowed (for current element of the enumerator).


**Types:**
- `Subscription`: [Subscription](AutoRemoveList.1.Subscription.md)
- `AutoSubscription`: [AutoSubscription](AutoRemoveList.1.AutoSubscription.md)

**Instance Properties:**
- `count`: int - Number of subscription.
- `subscriptions`: IEnumerable\[[Subscription](AutoRemoveList.1.Subscription.md)\] - Enumerate all subscriptions.

**Instance Methods:**
- `subscribe()`: [AutoSubscription](AutoRemoveList.1.AutoSubscription.md), value T
  - Subscribe to the list. Similar to `add` but returns auto-remove subscription. This is default member and will be used when you call the object.
- `add()`: [Subscription](AutoRemoveList.1.Subscription.md), value T
  - Add new item. Returns pure subscribtion (or null for duplicit item). Can also be accessed via `+=` operator in ROS.
- `remove()`: [Subscription](AutoRemoveList.1.Subscription.md), value T
  - Remove item. Returns the subscription on success, null if not found. Can also be accessed via `-=` operator in ROS.
- `clear()`: void - Remove all subscriptions.
