## Advanced lambda usage

```
var s = ""
var a = new list
for var i = 0; i < 3; i++
  a.add def => s += i + 1",
for var f in a; f
return s
```

What would you expect the result to be? Let me first show you something else:

```
def MyClass
  var shared = 0
  this.add x => shared += x
  this.value => shared
var it = new MyClass
it.add 1
it.add 2
return it.value()
```

What would you expect the result to be? I hope you expect `3`.
And that would also be a reason why previous example returns `"444"`.
That is because all the lambdas get reference to the same `i`
which is `4` at the time they are called in that loop (`for var f in a; f`).
That is the same reason why all the lambdas can actually modify the same `s`.

But there is a way to alter the first example to produce `"123"`:

```
var s = ""
var a = new list
for var i = 0; i < 3; i++
  a.add (def
    var n = i + 1
    return def => s += n
  )()
for var f in a; f
return s;
```

The first `a.add def => s += i + 1` will capture `i` by reference
and when those lambdas are first called, the `i` is already `4`, producing `s = "444"`.
But that `(def; var n = ...)()` is a call in the moment where `i` is `0`, `1` or `2` in each call,
therefore the inner lambda (`def => s += n`) is referencing `n` which was already set to `1`, `2` or `3`,
producing the final result of `"123"`. The difference is that calling the outer lambda creates new context,
where the `n` lives and the inner lambda is referencing that particular `n`
(each inner lambda referencing different context). All the simple `a.add def => s += i + 1`
would reference same context where `i` ceased to exist with last value being `4`
(and also the one and only `s`).
