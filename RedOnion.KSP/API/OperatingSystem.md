## OperatingSystem

Operating System - interface to MunOS.


**Types:**
- `Process`: [Process](OperatingSystem.Process.md)

**Static Properties:**
- `core`: MunCore - \[`Unsafe`\] Default instance of MunOS.
- `processCount`: int - Number of processes.
- `threadCount`: int - Number of threads.
- `processes`: Process[] - List of all processes.

**Static Methods:**
- `terminate()`: void - Terminate current process, but allow cleanup.
- `kill()`: void - Terminate current process now, stopping all threads immediately.
