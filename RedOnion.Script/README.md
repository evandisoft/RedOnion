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

## Invoke without parentheses

You have probably noticed that there are almost no parentheses
in the example above, except for one pair: `wnd.dispose()`.
This is because ROS took
[inspiration from Ruby](http://ruby-for-beginners.rubymonstas.org/bonus/parentheses.html)
and interprets e.g. `wnd.add new button` as `wnd.add(new button())`.
The reason why `wnd.dispose()` uses the pair is because calling any function
with no arguments automatically could cause problems when trying to pass reference to it
(e.g. in that `btn.click += click` - could be wrongly interpreted as `btn.click += click()`).

There is another potential problem with this approach - **`a +b` does not mean `(a) + (b)`**,
but `a(+b)` like `abs -x` means `abs(-x)`. That can be surprising, but we cannot have both.
`a + b`, `a+b` and `a+ b` all mean `(a)+(b)`, but unary operator not followed by space is unary
and therefore may surprisingly become argument for the previous identifier
(the parser does not know if it is or is not a function, it translates it into function call).