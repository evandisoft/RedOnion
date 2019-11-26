[![buttonscode.jpg](https://i.postimg.cc/HLbbKzWs/buttonscode.jpg)](https://postimg.cc/jWdWnzJG)

This is a simple tutorial with the most basic use of the RedOnion.UI library. Demonstrates creating some buttons and adding them to a window.

`uibasics.lua` has the following code:

```
window=new(ui.Window,"Window",ui.Layout.Vertical)

window.ChildAnchors=ui.Anchors.Fill

function addbutton(text)
    window.Add(new(ui.Button,text,function() print(text.." clicked!") end))
end

addbutton("button1")
addbutton("button2")
addbutton("button3")
addbutton("button4")
addbutton("button5")
addbutton("button6")
addbutton("button7")
addbutton("button8")
```

The UI library is available in the `ui` table/namespace. "ui." is pretty easy to type so it's fairly convenient to use in that respect.

First the window is created, using Kerbalua's `new` function, which calls the constructor of its first argument (the type, `ui.Window`) and passes `ui.Window`'s constructor the arguments, "Window" and `ui.Layout.Vertical`.
```
window=new(ui.Window,"Window",ui.Layout.Vertical)
```
It's the equivalent of doing `new ui.Window("Window",ui.Layout.Vertical)` in C#.

"Window" is the title. You can choose a layout of `ui.Layout.Vertical` or `ui.Layout.Horizontal` to automatically arrange child Elements of the Window Vertically or Horizontally.

Next we set the child Elements to fill up the available space in the window:
```
window.ChildAnchors=ui.Anchors.Fill
```

And we define a function for creating buttons that print out their text when clicked.

```
function addbutton(text)
    window.Add(new(ui.Button,text,function() print(text.." clicked!") end))
end
```

This adds a new button to the window using window.Add.

The new button is created with:
```
new(ui.Button,text,function() print(text.." clicked!") end)
```
Once again this calls Kerbalua's constructor function with the first argument being the type (ui.Button). Second argument is the `text` that will appear in the button, which was passed into `addbutton`.

Third argument is an anonymous function:
```
function() print(text.." clicked!") end
```
This will print out the `text` that was passed to `addbutton` when the button is clicked.
```
print(text.." clicked!")
```
Along with the message " clicked!".

If I run this script, then click the `>>` button to hide the editor, then drag the window over a bit I get the following image and output after clicking all the buttons.

[![buttonsresult.jpg](https://i.postimg.cc/wjCh0ZpG/buttonsresult.jpg)](https://postimg.cc/xXPJdsqv)

The base docs page for the RedOnion.UI library is [here](../RedOnion.UI/README.md)