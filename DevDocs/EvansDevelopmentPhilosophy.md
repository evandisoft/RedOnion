Here are some notes about how I approach development, in case some of the things I do seem sloppy or messy.

I believe there is sometimes too big of a cost to spending too much time trying to figure out how to do things **The Right Way**.

I used to spend gargantuan amounts of time trying to figure out the perfect way of doing everything. I would read lots of blogs about how to do programming in **The Right Way**. And I would generally get nothing done for fear of it not being done in the **Best Way Possible**.

I also have a cs degree, and in many cases I was told about the **Right Way**, with tales of the early days of programming and how programs, initially thought to be throw-aways, turned into major projects that exist to this day, but were full of issues related to how they were not initialially developed in **The Right Way**.

So I was taught many hoops and hurdles you just need to climb through to ensure your program can be used and updated FOREVER!!!

But in reality, if you have to go through all those hoops, you end up being incentivized to not explore much or produce many things, because the **Right Way** is very expensive.

So instead of having a bunch of useful programs that will exist forever, I wrote practically nothing.

There is a big [opportunity cost](https://en.wikipedia.org/wiki/Opportunity_cost) associated with trying to make everything perfect and maintainable **FOREVER** at the start.

My approach is to build prototypes. Build useful functionality, then change things later when and if I have a good idea on how I should restructure things to be more maintainable, and when I feel the restructuring is appropriate.

Exploration and prototypes help give you real essential information about the actual use-cases of your project, which will dramatically influence any attemps to restructure it into some nice, easy-to-maintain version.

If you try to produce the perfect version without practical experience about use cases, in my experience, you often end up having to do way too much work restructuring **The Perfect Program** into another **The Perfect Program** to provide just one additional type of feature.

So some things I write will be done in ways that were easy to initially write instead of being a nice maintainable structure that is nice and safe and doesn't expose fields as public and can be maintained "FOREVER".

I may, for example, write some small redundant code that could be hypothetically replaced by a loop, but where I consider at the time that writing it in the loop version requires way more effort than it is worth.

To some extent it is like [You aren't going to need it](https://www.martinfowler.com/bliki/Yagni.html).

I think that you should not go to one extreme or another but should try to figure out when it is a good time to restructure things for future maintainability, or when it is a good time to write something quickly.

A lot of stuff in in this project was produced initially as a prototype, then later improved to something more maintainable WHEN these two criteria were met:

1. Adding new features became difficult because the structure of the code was not condusive to easily adding features of the types that were found to be needed to be added.
1. I had enough information to get a feel for what types of features would be added, and so could produce a simplified version of the code that is customized for adding new features of those types.

So generally I start out with some simple code. Later find I need more features or it needs to handle more cases, then I add some if statements to handle some alternative possibilities or features, but then after there are so many exceptions and if-then's that the code becomes confusing, I try to switch to a design that uses object oriented principles and other code reuse strategies to simplify the code.

It took me a while to switch to this type of programming, but it works much better than what I was doing. However, it may bug some people, so this is my defense of some of the code I have written that looks sloppy. This project would not have been feasible for me if was not comfortable with writing some sloppy code.

Now, if your actual purpose is to write a library for use by others, you kinda do have to plan for maintainability at the start. And try to design an interface that allows you flexibility underneath it without changing that interface. But don't fall into the pit of "every library I make for my own use must also be designed for others" because it may be the case that only a small fraction of your own libraries would ever be used as libraries by others, and as such you may be wasting lots of time.

And if you do want to take a library that you designed for yourself and change it to be used by others, you can. And when you do that you will know, if you have used it for a while, what features it needs and you'll be able to provide a nice interface for others to use. And in my opinion it will probably be a better interface than if you had just tried to figure out, at the start, how the interface should be structured with no practical experience guiding you.

An additional problem, and perhaps the biggest problem is that if you spend too much time making some things be implemented perfectly, you may lose inspiration and kill the moment for the thing you were actually producing the library for.