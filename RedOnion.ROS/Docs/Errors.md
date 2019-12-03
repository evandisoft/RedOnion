## Error handling

`throw` and `raise` are for error propagation and can be used with any object/value. `try`, `catch` and `finally` are for handling errors (including exceptions originating from C# code).

`throw` and `raise` work the same, `throw` is from C#/Java, `raise` from Python, both exist in Ruby, but with different meanings (there is no `rescue` in ROS). Python uses `except` instead of `catch` (which can be added in future ROS version - consider it reserved keyword).

## Catch


The following code will return `true`:

```
try                 // start of error-handling block
  throw true        // throw/raise error to be propagated
  return false      // not executed because of the error above
catch var e         // handle error (if there is one)
  return e          // do something with the error-object/value
```


`catch` may be followed by variable declaration (`var e`) so that the following code can use the error-object/value for some logic (e.g. `e.message` for C# exceptions), but that is optional, simple `catch` without `var ...` will also work (if you do not care what the error is).

*Future plan: `catch var e:type` and `catch type`*

## Finally

`finally` is designed for cleanup - to ensure that the code inside is always executed, regardless of any error/exception. It can be combined with `catch` but does not have to:

```
var wnd = new ui.window
var err = null
try
  something
catch var e
  err = e
finally
  wnd.dispose
if err != null
  print err
```

C# does not allow `return` inside `finally`, but Java does, so ROS allows it too:

```
def test
  try
    if true         // raise/throw can be inside any block
      raise "error" // errors propagate until caught
  finally
    return null     // would return null if no exception
    irrelevant      // never gets executed
try
  test              // the raised/thrown "error" is propagated
catch var e         // until caught here
  print e
  throw e           // and can be rethrown / raised again
```