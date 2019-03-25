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
    wnd.dispose

var btn = wnd.add new button
btn.text = "Click Me!"
var lbl = wnd.add new label
lbl.text = "Clicked 0x"

var counter = 0
btn.click += def
    counter++
    lbl.text = "Clicked " + counter + "x"

// def and function are the same
function abs x
    return x < 0 ? -x : x
def sum x,y
    return x+y
// lambda
var sum3 = def a,b,c => a+b+c
```

## Invoke without parentheses

You have probably noticed that there are no parentheses in the example above.
This is because ROS took
[inspiration from Ruby](http://ruby-for-beginners.rubymonstas.org/bonus/parentheses.html)
and interprets `wnd.add new button` as `wnd.add(new button())`
and `wnd.dispose` as `wnd.dispose()`. You can still use parentheses, but do not have to.

There is one potential problem with this approach -
**`a`&#160;`+b` does not mean `(a)`&#160;`+`&#160;`(b)`**,
but `a(+b)` like `abs`&#160;`-x` means `abs(-x)`.
That can be surprising, but we cannot have both.
`a`&#160;`+`&#160;`b`, `a+b` and `a+`&#160;`b` all mean `(a)+(b)`.
Unary operator preceded but not followed by space
is unary and therefore may surprisingly become argument for the previous identifier
(the parser does not know if it is or is not a function, it translates it into function call).

Handling of potential functions without arguments is very restrictive
to avoid calling functions where you don't want to (e.g. `btn.click += click`
does not translate to `btn.click += click()`). It must be simple statement
to auto-invoke the function, like in `wnd.dispose` or e.g. `stage`
(which translates to `stage()`). It has to be pure identifier (`stage`)
or must end with dot + identifier (`wnd.dispose` or `get().action`).
This won't happen inside expressions (`btn.click += click` -
the `click` is syntactically inside `+=` operator).

```
var x = -1
var y = abs x   // abs(x)
var z = abs -y  // abs(-y)

var a = x+y     // ok, x + y
var b = x + y   // ok, x + y
var c = x+ y    // allowed, x + y
var d = x +y    // warning! x(+y)

var fn = abs    // no call to abs, fn is now alias to abs
z = fn (x)      // warning! it works but means fn((x))
z = sum (x,y)   // problem! (x,y) as one argument does not have meaning (yet)
z = sum(x, y)   // ok, this works as expected

def click => x++
button.click += click // no call to click
click           // yes, this calls click()

var obj = new object
obj.x = 0
obj.action = click
def get => obj
get().action    // calls obj.action() and increments obj.x
```

## Scopes and Classes

Although syntax for classes is still TODO, there is a way
and you could already see it in previous example.
You might have been surprised that `x` inside `click`
was automatically redirected to `this.x` when `click`
was used as a method, because that won't happen in JavaScript.
We decided to make it more C#-like in that regard
(including block scopes and no hoisting).
Beware that outer scope except global (function inside function)
has precedence over self-scope (this-object properties).
That means that local variables are accessed first,
top-level local variables of outer function next,
properties of the object after that (if method)
and global variables last.

```
def MyClass
    // to allow MyClass() be used like new MyClass
    if this == null // or this === undefined
        return new MyClass

    // create some properties
    this.name = "My Class"
    this.counter = 0

    // private properties
    var _total = 0

    // some method
    this.action = def
        counter++   // this.counter++
        _total++    // that private _total

    // read-only access to total
    this.getTotal = def => _total

var obj = new MyClass
obj.counter = 10
obj.action      // now obj.counter is 11
obj.getTotal    // returns 1
```

## Inheritance by `prototype`

Inheritance can be achieved by pointing `MyClass.prototype` to base class,
just like in JavaScript:

```
def BaseClass
    this.name = "Base Class"
    this.setCounter = def value => this.counter = value
MyClass.prototype = new BaseClass
var obj = new MyClass
obj.setCounter 123
```