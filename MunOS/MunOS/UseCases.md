Create ui.Buttons. Pussibly, things executed in a button would only run when
parent thread is sleeping. Things executed in the buttons run on a different thread.

Ideas:
    Threading system where different threads can only run if all the other ones are sleeping. And they take turns
    Threading system with atomic(fn). It waits for other threads to sleep, then runs the fn. Inside the fn even
        sleeping does not yield to the other threads