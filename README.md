# Bounce

A task runner for .Net.

Coming soon: full library depreciation. Entire framework will be moving to .NET Core 2.x when version 1.0.0 is launched.

## Install

Bounce can be found on [NuGet](http://nuget.org/List/Packages/Bounce), simply:

    PM> Install-Package Bounce

Then, make sure that your bounce project's output directory is the `Bounce` folder, in the root of your solution. `bounce.exe` looks for this folder in the current and all parent directories. For example, go to your project settings, go to the **Build** tab, then enter `..\Bounce` into the **Output path** box.

## What is it good for?

Put simply, it's a way to create a project toolbox that can be operated from the command line, not unlike [Rake](http://rake.rubyforge.org/).

Imagine you have this in your project:

    using System;
    using Bounce.Framework;

    namespace MyProject.Bounce {
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
    using Bounce.Framework;

    namespace MyProject.Bounce {
        public class Stuff {
            [Task]
            public void Hello(string name) {
                Console.WriteLine("hello, {0}!", name);
            }
        }
    }

Then:

    > bounce Hello /name Bob
    hello, Bob!

And those arguments can even have useful defaults:

    using System;
    using Bounce.Framework;

    namespace MyProject.Bounce {
        public class Stuff {
            [Task]
            public void Hello(string name = "all") {
                Console.WriteLine("hello, {0}!", name);
            }
        }
    }

Then:

    > bounce Hello
    hello, all!

And, if you've forgotten what you can do, just run bounce and it will tell you:

    > bounce
    usage: bounce task [options]

    tasks:

        MyProject.Bounce.Stuff.HelloWorld

        MyProject.Bounce.Stuff.Hello
            /name:string = all


## Naming your tasks project

Ensure your project produces an assembly ending with ".Bounce.dll" or an executable ending with ".Bounce.exe" (both case-insensitive). Bounce only looks for tasks in these assemblies, so that it can start up really quickly.

Bounce has a bunch of utilities that make it easy to build VisualStudio projects, as well as deploy to IIS sites. More documentation to come.
