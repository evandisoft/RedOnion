## OperatingSystem.Process

OS Process - collection of threads.


**Constructors:**
- `Process()`: path string
  - Create new process from a script (given path to it).

**Instance Fields:**
- `id`: long - Unique process identifier.

**Static Properties:**
- `current`: OperatingSystem.Process - Current process

**Instance Methods:**
- `terminate()`: void - Terminate the process, but allow cleanup.
- `kill()`: void - Terminate the process now, stopping all threads immediately.
