**Kerbalui** is a ui library designed for use with **LiveRepl**.

IT WAS NOT DESIGNED FOR OTHER PROJECTS. Many decisions were made for easy implementation in the moment. 

Part of the reason LiveRepl and Kerbalui are in different projects is because LiveRepl will probably someday be switched to use a different UI lib. 

Kerbalui was designed specifically to provide functionality for use with LiveRepl and I have not spent time to make it a general UI Lib. This philosophy applies somewhat: http://c2.com/xp/YouArentGonnaNeedIt.html

Part of the reason I have not tried to make it perfect is that [imgui](https://docs.unity3d.com/Manual/GUIScriptingGuide.html) is hard for me to work with.

## Types
All the following GUI classes are Elements.

- Controls represent a particular imgui control.

- Decorators represent an object which contains a control and acts as a proxy for it.

- Groups contain multiple controls and execute all of their Update's in an imgui `GUI.BeginGroup`

- Windows represent a top-level window in the ui system. They contain a title and a content area, which can be a Control, Decorator, or Group.

## Spacers
Spacers are defined in Kerbalui/Layout, and they are Groups that perform automatic layout for their child controls.
`VerticalSpacers` layout objects vertically and `HorizontalSpacers` lay them out horizontally.

With a spacer you add child elements with the following functions:
- AddFixed - This adds a child Control and reserves a fixed space for it
- AddMinSized - This adds a child Control and reserves a minimum space for it based on its content size.
- AddWeighted - This adds a child control and sets a weight for it.

Layout works by first reserving all the space needed for the fixed and minsized elements, and then dividing up the remaining space allocated to the spacer between the weighted elements based on their weights.

It is pretty easy to make a GUI out of nothing but vertical and horizontal spacers.

To fill a spot in a spacer with empty space, the Filler class can be used.

## Pitfalls
There are some pitfalls of imgui. Some have been fixed with the new version of Unity that KSP is now using.

One remaining one is that different sets of events are sent in Windows, as opposed to Linux (haven't tested on OSX).

So I have a special system for dealing with these events which I won't describe, but is the purpose of the `ConsumeAndMarkNextCharEvent` and `ConsumeMarkedCharEvent` calls in `Kerbalui.Util.GUILibUtil.cs`