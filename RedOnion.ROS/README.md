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

There also has to be a rule which comma belongs to which function call,
and that would be to the nearest:

```
print new point x, y
print "{0} and {1} {2}", abs(start-now), sum(x, y), "seconds"
print "{0} and {1} {2}", (abs start-now), (sum x, y), "seconds"
```

## Multi-line statements

Parentheses can also be useful when you want to write an expression
spanning multiple lines. The parser will continue parsing
until all parentheses are closed, binary operators have their arguments,
or if line ends with `\` or comma (when in function call).

```
def each list, actionOrCondition, action
  if arguments.length == 2
    for var e in list
      actionOrCondition e
    return
  for var e in list
    if actionOrCondition()
      action e
each [1,2,3],
  (def e => e % 2 == 1), // better always use parens here
  (def e => print e)     // parens here are optional (last arg)
var z = x +
  y
z = x \
  + y
```

## Number types

Beware that ROS distinguishes between various types of numbers,
especially integers (`int`, `uint`, `byte`, ...) vs. floating-point number (`double` or `float`).
This could sometimes surprise you: `1/3` is `0` but `1.0/3` is `0.333...`
because `1/3` uses integer arithmetic, but `1.0/3` uses floating-point arithmetic.
Integer types get promoted to floating-point (`double`)
if used in any operation involving floating-point number (the `3` gets converted to `3.0`
making the expression `1.0/3.0`). Smaller integer types always get promoted to `int` or `double`.

Operator `^` could also surprise you, because it means bitwise-xor in C#/C++.
ROS translates the operator to `math.pow` if used with floating-point number,
but again, remember that you first have to enforce floating-point arithmetic,
or just use `**` operator (which works like `math.pow` even for integers).
Also note that bitwise operators (`&`, `|`, `^`, `<<` and `>>`)
have much higher priority/precedence in ROS than in C#/C++.

## Functions and lambdas

Functions are created by `function` or `def` keyword
followed by name of the function and list of arguments.
The body has to be indented at least one space more
than the `function`/`def` keyword (or rather the line with that keyword).

Lambdas have similar syntax but are anonymous,
therefore there is no name after the keyword.
`=>` can be used to inline the body.

Functions (and lambdas) can be called with any number of arguments,
regardless of how many are specified in the definition.
Array of all the arguments actually passed can be accessed
via `arguments` variable, as seen in previous example
(`if arguments.length == 2`). It can also be indexed
(`arguments[0]` and `for var a in arguments`).

Every named argument actually passed is also accessible
as a variable under the name specified in the definition.
Arguments not provided will have the value `null`.

```
def sum
  var it = 0
  for var e in arguments
    it += e
  return it

function fill list, element
  for var i = 0; i < list.length; i++
    list[i] = element

var a = [1,2,3]
fill a    // a = [null, null, null]
fill a, 1 // a = [1,1,1]
```

*Future plan: default arguments.*

## Statements

All the usual statements can be found in ROS.
Most of them come from C#, some from other languages.

`if/unless` is followed by a condition,
optional `then`, colon (`:`) or semicolon (`;`).
Statement may follow, more statements must be indented.
`else` with another block of statements may follow last.

```
if condition
  doSomething()
if condition then doSomething() else somethingElse()
unless condition; doIfFalse()
unless condition: doIfFalse(); else: somethingElse()
```

`while/until` works in similar way but loops `while` (as long as)
a condition is true or `until` it becomes false.
The test can be moved at the end, by starting the loop with `do`.
Optional `do` after condition can also be used.

```
while condition do something()
while condition do:
  something()
until apoapsis > desired
  ship.throttle = 1

do something()
until done()
```

`for` and `foreach` are the advanced loops.
`for` can either be used in the form of
`for init; test; final; block`
or like `foreach var e in list` with just `for var e in list`.

```
for var int i = 0; i < arguments.length; i++
  print arguments[i]
for var e in list
  print e
foreach var e in list
  print e

var sum = 0
for var e in arguments: sum += e
foreach var e in arguments do sum += e
for var i = 0; i < arguments.length; i++; print arguments[i]
```

`break` can break the execution of any loop (jumping after the loop),
`continue` can continue at the start of the loop (skipping the rest of the block).
