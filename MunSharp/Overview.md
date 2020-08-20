Status:
- On local coroutine-experiments branch
- RedOnion successfully compiles and runs using the MunSharp project instead of the package.
- MunSharp.Tests project is integrated
- LICENSES were modified and placed in the MunSharp and MunSharp.Tests project folders.
- Implementations that allow coroutines, and implement sleep, are in effect. Sleep works. Coroutines do not work.
- Out of game tests for the new features of MunSharp were implemented.

Goals:
- Remove redundancy. The Kerbalua layer on top of MunSharp may be mostly redundant.
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
- Make some in-game test scripts.
- Do something about the failing lua os test.
- Ensure that lua error handling is not effected by the changes I make
- Ensure that debugging is not effected by the changes I make