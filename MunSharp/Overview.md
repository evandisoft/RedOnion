Status:
- On local munsharpintegration branch
- RedOnion successfully compiles and runs using the MunSharp project instead of the package.

Goals:
- Remove redundancy. The Kerbalua layer on top of MunSharp is probably mostly redundant.
- Provide coroutines to the user. Instead of using autoyield for regularly interrupting execution, implement a way of doing that that is compatible with giving the user access to coroutines.
- Customize MunSharp to work as smoothly as possible with Munos.
- Figure out debugging possibilities.
- Become very familiar with the MunSharp code.

Optional Goals:
- Replace MunSharp compiler with an Antlr based one.
- Retroactive debugging info using opcodes and queuelogger?
- Consolidate intellisense code.
- Give the user something close to the normal lua debugging experience.

Todo:
- Integrate munsharp tests
- Modify license