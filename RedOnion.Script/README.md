## Basics

ROS (Red Onion Script) is based on
[ECMAScript/JavaScript](https://www.ecma-international.org/publications/standards/Ecma-262.htm)
/ [ActionScript](https://en.wikipedia.org/wiki/ActionScript)
and inspired by
[Ruby](https://www.ruby-lang.org/)
/ [Python](https://www.python.org/)
/ [Boo](http://boo-lang.org/).

```
var wnd = new window
function shutdown
  wnd.dispose()

var btn = wnd.add new button
btn.text = "Click Me!"
var lbl = wnd.add new label
lbl.text = "Clicked 0x"

var counter = 0
function click
  counter++
  lbl.text = "Clicked " + counter + "x"
btn.click += click
```