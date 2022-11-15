## oxi
.NET implementation of the MOO system.

### tasks
* Every time a player types a command a task is created to execute that command (command tasks)
* Whenever a player connects or disconnects the server starts a task to execute whatever processing is necessary (server tasks)
* The fork statement can be used to create tasks that are delayed for at least some number of seconds (forked tasks)
* The suspend() function can be used to suspend the current task (suspended tasks)
* The read() function can be used to create a task that will be suspended until a line of input is received (reading tasks)

The last three kinds of tasks are known as background or queued tasks since thay may not run immediately.

A task can be seen as an environment and some kind of function F that represents the delayed evaluation and that results in some `IValue` instance.

Tasks are not designed to be persisted.

### fork
```
fork <ident> (expression)
    ...
endfork
```

Expression must evaluate to an integer. This will be the number of seconds that the forked task will be delayed. The identifier is optional. It will have the task id of the task associated with the forked block. This is useful for killing the forked starts before it starts.

# Notes
## Ranges
Range parsing is a bit iffy, something like `[0..maxobject()]` will likely confuse the parser since it will succeed tokenizing `0.` as a floating point token and then it will not have a clue with the rest of the text span. Work arounds are to use either `[(0)..(maxobject())]` or (slightly cleaner) `[0 .. maxobject()]` instead (with spaces around the range (`..`) operator).

## Statements vs. expressions.
By default the interpreter will execute statements. That means it will complain if you forget the semicolon statement terminator `;`. However, it is useful to keep in mind that this is basically just an operator to turn an expression in a statement. If you use an actual statement you don't need the semicolon.

For example, this doesn't work:
```
> 3 + 2
```

The interpreter will respond with somthing similar to:
```
Syntax error: unexpected end of input, expected `;`.
3 + 2
     ^
```

That's because `3 + 2` is an expression. To make it a statement we can just add a semicolon:
```
> 3 + 2;
=> 5
```

And now the interpreter responds with `5` as expected.

This can get a bit confusing however if you're not exactly sure what is a statement and what is an expression. 

> Generally, everything that involves keywords is usually a statement. For example, `if`, `for`, `try`, etc. On the other hand, if it involves symbols like `+`, `-` and `*` then it's usually expressions. 
>
> Similarly, if it changes some kind of common or global state then it should be considered an statement, if something just produces a value based on its inputs then it's best considered an expression. 
>
> In short, statements change state and expressions produce values.

Let's invesitage a bit closer. We use a `for` statement:
```
> for x in [0..10] "frotz"; endfor
=> "frotz"
```

You might be confused **why a statement has a result** and this is the confusing thing: a statement can be an expression as well depending on how you define the rules. In this case, the loop is executed 10 times and each time with a result of "frotz". The result of the `for` statement is defined to be the result of the final expression in the body of the `for` loop.

This should make sense until you realize: "the body of a `for` loop is just a bunch of statements, how can a statement produce an expression?".

Well this is where the `;` (semicolon) operator steps in. If you have an expression, *any* expression, then you can make it into a statement by just applying the `;` operator. Mathematically it would look like:
```
next = ;(expr)
```

But in an imperative language we like to write things as a sequence so we tend to omit the state. The thing that we omit here is commonly known as *global state* but we will come back to that later.

So we just write a bit more conveniently:
```
expr1;
expr2;
```

In this case we *implicitly* use the *global* environment by virtue of the `;` operator which allows us to modify state instead of just producing values.

Conceptually, what this does is create one big function where each *expr* is executed in sequence and the results of one influence the next ones.

However, an *expr* cannot modify the world - it can only produce a value based on known inputs. So in order to have an expression *influence* the world we can turn it into a statement by using the `;` operator.

This works because an expression only produces a value and **should not have any side-effects**. If the expression is to produce any side-effects then by definition it is no longer an expression but a statement instead.

> This also does not work because the tokenizer and parser team are quite stupid and do not have the expertise right now to properly cater for all the use-cases that users come up with.

By definition we can turn any expression into a statement just by applying the `;` operator.

Anyway, you **should not** put semicolons wherever you feel like they make sense since they do impact the parser (in some bad ways).

So back to the correct for loop for now, the following example will show that the result of the `for` body is the result of the last expression in the body of the loop.
```
> for x in [0 .. 10] x; endfor;
=> 10;
```

And we can see that the result of a `for` is the result of the final stament in the loop. 

> Note that even statements produce values, don't worry too much about it. Think of statements as a more powerful version of expressions. Where expressions can only produce a value based on known inputs, a statement can make use of persistence and the world around it to produce a value. It's less mathematical and more mechanical. Expressions are usually purely mathematical. This also means that the singular result of a statement doesn't always make much sense (like in the examples above). However, it should always make the *most* sense given the assumptions.

The semicolon after `"frotz"` is important otherwise the parser will assume `endfor` is part of some expression that is part of the `for` body. If we omit that semicolon we get an error informing us that we missed that semicolon somewhere:
```
Syntax error (line 1, column 28): unexpected endfor `endfor`, expected `;`.
for x in [0 .. 10] "frotz" endfor;
                           ^
```

This will behave as a statement. However, you might be tempted to write the following:
```
for x in [0..10] "frotz"; endfor; 123;
=> "frotz"
```
You might expect the answer to be `123` but the interpreter will answer `"frotz"` instead. This is a bug that causes a `return` value somewhere because of the empty statement caused by the stray semicolon (`;`) after the `endfor` keyword. 

> It looks like the `endfor` already closes the statement so the parser gets in a confused state with the empty statement and somehow decides to return early (needs investigation).

> Although this is a syntax error we shouldn't be too obnoxious about it, I want to provider a better experience for this one but I do not want it to be too magical. It seems like empty expressions should just be ignored but they should definitely not lead to an early return.

For now the workaround is to *not* make this future syntax error that is currently not observed by the parser. Use semicolons **only** when you need to **turn an expression into a statement** and leave them out otherwise. Be careful my friends!
