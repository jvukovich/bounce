# Bounce
An experimental new build framework for C# projects

## Why use Bounce?

So your build configuration can be clean, declarative and easy to maintain.

Sounds nice, but how?

It works slightly differently to NAnt and Rake. It uses tasks that can be composed together to form
complex dependency trees of build operations. Tasks depend on each other, and in doing so they can query
tasks for configuration settings - eliminating the need for global variables.

## Why C#?

No scripting language? How controversial! :)

Everybody's talking about using Ruby and Rake to hack their build scripts? Why use C#?

For us .Net hackers, C# is our lingua franca. And when C# is used as your production language
_and_ you're build language, you can easily share code and configuration between them. No language barriers
- they're just not worth it.

## Show me

Lets say we've got a VisualStudio solution containing a website and you want it installed on IIS 7.0.
We'd write a C# file containing our targets like this:

<pre><code>public class BuildTargets {
    [Targets]
    public static object Targets (IParameters parameters) {
        var solution = new VisualStudioSolution {
			SolutionPath = "WebSolution.sln".V(),
		};
        var webProject = solution.Projects["WebSite".V()];

        return new {
            WebSite = new Iis7WebSite {
				Path = webProject.Directory,
				Name = "My Website".V(),
				Port = 5001.V(),
			},
        };
    }
}</code></pre>

Build the `BuildTargets` class into `MyBuild.dll` and you can build your website like this:

    bounce MyBuild.dll build WebSite

Say you wanted to add a unit test task:

<pre><code>public class BuildTargets {
    [Targets]
    public static object Targets (IParameters parameters) {
		...

        return new {
            WebSite = new Iis7WebSite { ... },
			<b>Tests = new NUnitTests {
                DllPaths = solution.Projects.Select(p => p.OutputFile),
			},</b>
        };
    }
}</code></pre>

And, to run them:

	bounce MyBuild.dll build Tests

And, say you wanted to do a `git` checkout before you built the solution:

<pre><code>public class BuildTargets {
    [Targets]
    public static object Targets (IParameters parameters) {
		<b>var gitrepo = new GitRepo {
			Origin = "git@github.com:refractalize/website.git",
		};</b>
        var solution = new VisualStudioSolution {
			SolutionPath = <b>gitrepo["WebSolution.sln".V()]</b>,
		};
		
		...
	}
}</code></pre>

`GitRepo` clones the github repo and the rest of the build works from the checkout directory.
