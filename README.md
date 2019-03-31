# What is Bounce?

Bounce is a task runner for .NET. Our goal is to get developers to stop using PowerShell, MSBuild, NAnt and other scripting frameworks. Bounce allows you to more easily utilize the .NET stack in a consistent way, share code, leverage third party libraries, and build tests for your tasks.

## Coming Soon

**Full library Deprecation**

The entire framework will be moving to .NET Core 2.x when version 1.0.0 is launched.

**Why .NET Core and not .NET Standard?**

Because **AppDomain.CreateDomain** throws a **PlatformNotSupportedException** in .NET Standard.

Additionally, **AssemblyLoadContext** is only available in .NET Core.

One of these is necessary to load an assembly at runtime.

**Breaking Changes**

Support for Visual Studio tools and IIS management will be dropped (for better cross-platform support).

In the future, common scripting APIs may be launched as separate NuGet packages.

Developers are encouraged to migrate their Bounce projects to .NET Core.

## Project Setup

Bounce can be found on [NuGet](http://nuget.org/List/Packages/Bounce).

To create a Bounce script project:

    PM> Install-Package Bounce

**Required**: your assembly file name must end with **.Bounce.dll**, or an executable ending with **.Bounce.exe** (both case-insensitive).

Example: **MyProject.Bounce.dll** or **MyProject.Bounce.exe**

For optimal performance, Bounce only looks for tasks in these assemblies.

Finally, your project must be in .NET Core (and the same version Bounce is built on).

## Running

To execute a Bounce project, first install the .NET Core tool:

    dotnet tool install --global Bounce.Console [--version x.x.x]

You may need to restart your console to refresh your environment variables.

See [.NET Core Global Tools](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) for more information and options.

Then, you can execute Bounce from anywhere on your system:

    > bounce [args]

## Examples

The idea is to create a project toolbox that can be operated from the command line: similar to [Rake](https://github.com/ruby/rake).

Imagine you have this in your project:

    using System;
    using Bounce.Framework;

    namespace MyProject.Bounce
    {
        public class Stuff
        {
            [Task]
            public void HelloWorld()
            {
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

    namespace MyProject.Bounce
    {
        public class Stuff
        {
            [Task]
            public void Hello(string name)
            {
                Console.WriteLine("hello, {0}!", name);
            }
        }
    }

Then:

    > bounce Hello /name:Bob
    hello, Bob!

And those arguments can even have useful defaults:

    using System;
    using Bounce.Framework;

    namespace MyProject.Bounce
    {
        public class Stuff
        {
            [Task]
            public void Hello(string name = "all")
            {
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

Finally, if there are any colliding task names in your project, you can invoke them by namespace.

MyProject.Bounce.Namespace1:

    using System;
    using Bounce.Framework;

    namespace MyProject.Bounce.Namespace1
    {
        public class Stuff1
        {
            [Task]
            public void HelloWorld()
            {
                Console.WriteLine("hello, world! (Namespace1)");
            }
        }
    }

MyProject.Bounce.Namespace2:

    using System;
    using Bounce.Framework;
    
    namespace MyProject.Bounce.Namespace2
    {
        public class Stuff2
        {
            [Task]
            public void HelloWorld()
            {
                Console.WriteLine("hello, world! (Namespace2)");
            }
        }
    }

Then:

    > bounce MyProject.Bounce.Namespace1.HelloWorld
    hello, all! (Namespace1)
    
    > bounce MyProject.Bounce.Namespace2.HelloWorld
    hello, all! (Namespace2)

If is possible to execute multiple tasks at once. It is also possible to share arguments passed to tasks, without the methods requiring those arguments as parameters. To do this, utilize the static **Props** class:

    using System;
    using Bounce.Framework;
    
    namespace MyProject.Bounce
    {
        public class Stuff
        {
            [Task]
            public void Task1()
            {
                var env = Props.Get("env");
                Console.WriteLine($"Task1, env = {env}");
            }

            [Task]
            public void Task2(string a)
            {
                var env = Props.Get("env");
                Console.WriteLine($"Task2, env = {env}, a = {a}");
            }
        }
    }

Then:

    > bounce Task1 Task2 /env:myenv /a:a
    Task1, env = myenv
    Task2, env = myenv, a = a

Note: **Props** only supports strings. Any necessary type conversion is your responsibility.

## License

Bounce uses the [MIT License](https://github.com/jvukovich/bounce/blob/master/LICENSE).