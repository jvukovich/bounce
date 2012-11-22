# Bounce

A new build framework for C# projects.

(theme track: [Bounce, Rock, Skate, Roll - Vaughan Mason & Crew](http://www.youtube.com/watch?v=dGMD0O7GGP8&feature=related))

## Install

Bounce can be found on [NuGet](http://nuget.org/List/Packages/Bounce), simply:

    PM> Install-Package Bounce

## What is it good for?

Put simply, it's a way to create a project toolbox that can be operated from the command line, not unlike [Rake](http://rake.rubyforge.org/).

Imagine you have this in your project:

    using System;

    namespace MyProject {
        public class Stuff {
            [Task]
            public void HelloWorld() {
                Console.WriteLine("hello, world!");
            }
        }
    }

You could easily call it from the command line like this:

    > bounce HelloWorld
    hello, world!

Of course, you can pass arguments too:

    using System;

    namespace MyProject {
        public class Stuff {
            [Task]
            public void Hello(string name) {
                Console.WriteLine("hello, {0}!", name);
            }
        }
    }

    > bounce Hello /name Bob
    hello, Bob!

And those arguments can even have useful defaults:

    using System;

    namespace MyProject {
        public class Stuff {
            [Task]
            public void Hello(string name = "all") {
                Console.WriteLine("hello, {0}!", name);
            }
        }
    }

    > bounce Hello
    hello, all!

And, if you've forgotten what you can do, just run bounce and it will tell you:

    > bounce
    usage: bounce task [options]

    tasks:

        MyProject.Stuff.HelloWorld

        MyProject.Stuff.Hello
            /name:string = all

Bounce has a bunch of utilities that make it easy to build VisualStudio projects, as well as deploy to IIS sites. More documentation to come.
