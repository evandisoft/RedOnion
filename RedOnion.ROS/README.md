## Basics

ROS (Red Onion Script) is inspired by C#,
[Ruby](https://www.ruby-lang.org/),
[Python](https://www.python.org/) and
[ActionScript](https://en.wikipedia.org/wiki/ActionScript).
The syntax is mostly Ruby/Python-like,
most features and operators come from C#,
some ideas were taken from ActionScript (and Pascal) and first design was based on JavaScript,
but ROS is on its own path now.

For other documens see [Links](Docs/Links.md),
[Vim syntax file here](redonion.vim).

```
var wnd = new window "Hello"    // variable declaration
wnd.close += wnd.dispose        // events

var btn = wnd.add new button    // parentheses are optional
btn.text = "Click Me!"
var lbl = wnd.add new label
lbl.text = "Clicked 0x"

var counter = 0
btn.click += def                // lambda / inline function
  counter++                     // shared outer scope
  lbl.text = "Clicked " + counter + "x"

function abs x                  // custom functions
  return x < 0 ? -x : x         // ternary operator
def sum x,y                     // def and function work the same
  return x+y
var sum3 = def a,b,c => a+b+c   // lambda shortcut (inline body)
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


## Statements

All the usual statements can be found in ROS.
Most of them come from C#, some from other languages.

`var` is used to declare new (local) variable (`global` or `globals`
can be used for global variables - like `global.x = 1` or `system.globals.x`).
Variable names are searched from the point of reference, up through all
(non-function) blocks, then in `this` if function was executed as method,
then in outer function or script and lastly in globals
(notice that function `click` in example above modified script-local `x`
when called as function, but `obj.x` when called as method).

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
a condition is true or `until` it becomes true (loops while it is false).
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

For `try..catch..finally` and `throw`/`raise` see [Error handling](Docs/Errors.md).


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

For more info see [Advanced lambda usage](Docs/LambdaAdv.md).

## Events and parallelism

ROS offers parallelism (coroutines / fibers / threads)
through `system.update`, `system.idle` and `system.once` events.
These events cannot be used with `+=` and `-=` operators,
but are functions, that accept delegate (reference to function)
and return a subscription object (which has `.remove` method).
You have to keep a reference to that object in order to
keep the subscription alive and executed.
(There also are `.add` and `.remove` methods, which return different
subscription object without the auto-remove functionality,
but it is adviced to use the call instead.)

* `update` is designed for things that need to run every physics tick
  (like steering and throttle control). At least 100 instructions
  of subscribers will be executed in each tick (upto 500 if possible).
* `idle` is for less important things (like staging) and subscribed functions
  may get executed less often (depends on load), but always at least
  every 10th physics tick (at least 100 instructions).
* `once` is designed for things that need to run only once
  and is mostly used for UI events like `Button.Click`
  (and generally for any native/reflected event from .NET - functions
  are converted to delegates by using lambda that subscribes it to `once`).

The script, currently being executed, gets a share every tick as well,
depending on the load, but always at least 100 instructions
or until `yield` or `wait` statement is reached - this serves as
voluntary release of the processor/core and can also be used
in the functions subscribed to `update`/`idle`/`once`.

Take extra caution when script and functions subsribed to those events
are accessing same variables! Reading (get) and writing (set/assign)
is atomic (cannot be interrupted), but `.` (dot) operator is instruction
on its own, which means that `object.name` is not atomic operation
(`if obj != null then doSomethingWith obj.name` is not safe
if some other parallel function can do `obj = null`).

*Future plan: `using` statement - subscription objects are disposable.*


## Running other scripts

Function `run` (`system.run` - but namespace `system`
is automatically included in the scope) can execute other scripts.

* `run path` - calls a script like a function
  (suspends execution of current script until the one called exits).
* `run.library path` - calls a script in current context, sharing script-local scope.
  This is useful for libraries if you want to share script-local scope
  (you would otherwise have to use `system.globals`).
* `run.replace path` - terminates current script and replaces it with the one referenced.
  This is good for big switch in logic (e.g. launch - circularize - gen. control).

* `run.source string` - like `run path`, but parses the input string directly.
  This can be used to evaluate expressions or whole scripts created as text.
* `run.library.source` and `run.replace.source` - like the respective *`run.xxx path`*
  but accepting text.

*Future plan: creating new processors/processes and load monitoring.*

