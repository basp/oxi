# oxi
.NET implementation of the MOO system.

# tasks
* Every time a player types a command a task is created to execute that command (command tasks)
* Whenever a player connects or disconnects the server starts a task to execute whatever processing is necessary (server tasks)
* The fork statement can be used to create tasks that are delayed for at least some number of seconds (forked tasks)
* The suspend() function can be used to suspend the current task (suspended tasks)
* The read() function can be used to create a task that will be suspended until a line of input is received (reading tasks)

The last three kinds of tasks are known as background or queued tasks since thay may not run immediately.

A task can be seen as an environment and some kind of function F that represents the delayed evaluation and that results in some `IValue` instance.

Tasks are not designed to be persisted.

# fork
```
fork <ident> (expression)
    ...
endfork
```

Expression must evaluate to an integer. This will be the number of seconds that the forked task will be delayed. The identifier is optional. It will have the task id of the task associated with the forked block. This is useful for killing the forked starts before it starts.

