﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using Bounce.Framework;
using System.Text.RegularExpressions;

namespace TestBounceAssembly {
    public class BuildTargets {
        public static object GetTargets (IParameters parameters) {
            var git = new GitCheckout {
                Repository = @"C:\Users\Public\Documents\Development\BigSolution.git",
                Directory = "one"
            };
            var solution = new VisualStudioSolution {
                SolutionPath = git.Files["BigSolution.sln"]
            };
            var webProject = solution.Projects[parameters.Default("proj", "BigSolution")];
            var serviceName = parameters.Default("svc", "BigWindowsService");
            var service = solution.Projects[serviceName];

            return new {
                WebSite = new Iis7WebSite {
                    Directory = webProject.ProjectDirectory,
                    Name = "BigWebSite",
                    Port = 5001
                },
                Tests = new NUnitTests {
                    DllPaths = solution.Projects.Select(p => p.OutputFile)
                },
                Service = new WindowsService {
                    BinaryPath = service.OutputFile,
                    Name = serviceName,
                    DisplayName = "Big Windows Service",
                    Description = "a big windows service demonstrating the bounce build framework"
                },
                Zip = new ZipFile {
                    Directory = webProject.WhenBuilt(() => Path.GetDirectoryName(webProject.OutputFile.Value)),
                    ZipFileName = "web.zip"
                },
            };
        }

        public static object Targets(IParameters parameters) {
            var solution = new VisualStudioSolution {
                SolutionPath = "WebSolution.sln",
            };
            var webProject = solution.Projects["WebSite"];

            return new {
                WebSite = new Iis7WebSite {
                    Directory = webProject.ProjectDirectory,
                    Name = "My Website",
                    Port = parameters.Default("port", 5001),
                },
                Tests = new NUnitTests {
                    DllPaths = solution.Projects.Select(p => p.OutputFile),
                },
            };
        }

        public static object Iis6Targets(IParameters parameters) {
            var appPool = new Iis6AppPool {Name = "MyNewAppPoolForOutputToFile"};

            return new {
                           WebSite = new Iis6WebSite() {
                                                           Directory =
                                                               @"C:\bzr",
                                                           ScriptMapsToAdd = Iis6WebSite.MvcScriptMaps,
                                                           Name = "OutputToFile (bounce)",
                                                           Bindings = new [] {new Iis6WebSiteBinding {Port = 7020}},
                                                           Authentication = new[] {Iis6Authentication.NTLM},
                                                       }
        };
        }

        public static object SomeTargets() {
            return new {
                           Copy = new Copy {FromPath = "TestFrom", ToPath = "TestTo", Excludes = new [] {@"**\_svn\"} },
                       };
        }

        public static object WebSiteDoco() {
            var website = new Iis6WebSite {Name = "MyWebSite", Directory = @"c:\websites\mywebsite"};
    website.Bindings = new[] {
                                 new Iis6WebSiteBinding {Hostname = "mywebsite.com"},
                                 new Iis6WebSiteBinding {IPAddress = IPAddress.Loopback, Port = 7010}
                             };
    website.ScriptMapsToAdd = new[] {new Iis6ScriptMap {
                                                           AllVerbs = false, 
                                                           Executable = @"c:\website\scriptmaps\myscriptmap.dll", 
                                                           Extension = ".dat", 
                                                           IncludedVerbs = "GET,POST,DELETE", 
                                                           ScriptEngine = true, 
                                                           VerifyThatFileExists = false
                                                       }};
    website.ScriptMapsToAdd = Iis6WebSite.MvcScriptMaps;
    website.AppPool = new Iis6AppPool {Name = "MyAppPool"};
    website.Authentication = new[] {Iis6Authentication.NTLM, Iis6Authentication.Anonymous};
    website.Started = false;

            return null;
        }

        private static Func<string, string> RewriteVersion(Task<string> version) {
            Regex asmVersion = new Regex(@"^(\s*\[assembly:\s*Assembly(File)?Version\s*\(\s*"").*?(""\s*\)\s*\])", RegexOptions.Multiline);
            return contents => {
                return asmVersion.Replace(contents, match => {
                    return match.Groups[1].Value + version.Value + match.Groups[3].Value;
                });
            };
        }

        public static object RealTargets(IParameters buildParameters) {
            var version = buildParameters.Required<string>("version");

            var git = new GitCheckout {
                Repository = "git://github.com/refractalize/bounce.git",
                Directory = "tmp2",
            };

            var asmInfoWithVersion = new RewriteFile {
                FilePath = "SolutionAssemblyInfo.cs",
                Rewriter = RewriteVersion(version),
            };

            var solution = new VisualStudioSolution {
                SolutionPath = "Bounce.sln",
                DependsOn = new [] {asmInfoWithVersion}
            };

            var frameworkProject = solution.Projects["Bounce.Framework"];

            var downloadsDir = new CleanDirectory {
                Path = "Downloads",
            };

            var frameworkZip = new ZipFile {
                Directory = frameworkProject.WhenBuilt(() => Path.GetDirectoryName(frameworkProject.OutputFile.Value)),
                ZipFileName = downloadsDir.Files[version.WhenBuilt(() => string.Format("Bounce.Framework.{0}.zip", version.Value))],
            };

            var gitTag = new GitTag {Directory = ".", Tag = version.WhenBuilt(() => "v" + version.Value)};
            var downloads = new All(frameworkZip, gitTag);

            return new {
                Tests = new NUnitTests {
                    DllPaths = solution.Projects.Select(p => p.OutputFile),
                },
                Downloads = downloads,
                RewriteAsmInfo = asmInfoWithVersion,
            };
        }
    }

    public class SimpleTask : Task {
        [Dependency]
        public Task<string> Description;

        public override void Build(IBounce bounce) {
            bounce.ShellCommand.ExecuteAndExpectSuccess("cmd", "/c dir");
        }

        public override void Clean(IBounce bounce) {
            Console.WriteLine("Cleaning {0}", Description.Value);
        }
    }

    public class TargetBuilder {
        public static object GetTargets() {
            return new {
                           Simple = new SimpleTask {Description = "our simple task"},
                       };
        }
    }

    class PrintTask : Task {
        [Dependency]
        public Task<string> Description;

        protected readonly TextWriter Output;

        public PrintTask(TextWriter output) {
            Output = output;
        }

        public PrintTask() {
            Output = Console.Out;
        }

        public override void Build() {
            Output.WriteLine(Description.Value);
        }
    }

    class PrintTaskWithPath : PrintTask {
        [Dependency] private Task<string> _path;

        public Task<string> Path {
            get { return this.WhenBuilt(() => _path.Value); }
            set { _path = value; }
        }

        public override void Build() {
            Output.WriteLine(Description.Value);
            Output.WriteLine(_path.Value);
        }
    }

    class SubBounceFactory : IRemoteBounceFactory {
        public ITask CreateRemoteBounce(Task<string> bounceArguments, Task<string> workingDirectory, Task<string> machine) {
            return new SubBounce {
                BounceArguments = bounceArguments,
                WorkingDirectory = workingDirectory,
            };
        }
    }

    public class Stuff
    {
    public static object GetTargets(IParameters parameters)
    {
        var stage = parameters.Default("stage", "packageDeploy");

        var solution = new VisualStudioSolution {SolutionPath = "Solution.sln"};

        var machines = new[] {
            new DeployMachine {
                LocalPath = @"c:\Deployments",
                Machine = "liveserver1",
                RemotePath = @"\\liveserver1\Deployments"
            },
            new DeployMachine {
                LocalPath = @"c:\Deployments",
                Machine = "liveserver2",
                RemotePath = @"\\liveserver2\Deployments"
            }
        };

        var targets = new StagedDeployTargetBuilder(stage);
        var website = targets.CreateTarget("WebSite");

        website.Package = new Copy {
            FromPath = solution.Projects["WebSite"].ProjectDirectory,
            ToPath = new CleanDirectory {Path = "package"}.Path.SubPath("WebSite")
        }.ToPath;

        website.InvokeRemoteDeploy = website.CopyToAndInvokeOnMachines(machines, new SubBounceFactory());

        website.Deploy = package => new Iis7WebSite {
            Directory = new Copy {
                FromPath = package.SubPath("WebSite"),
                ToPath = @"C:\Sites\WebSite"
            }.ToPath,
            Name = "WebSite"
        };

        return targets.Targets;
    }
    }

    public class LongRunningTask : Task
    {
        [Targets]
        public static object GetTargets()
        {
            return new {LongRunning = new LongRunningTask()};
        }

        public override void Build(IBounce bounce)
        {
            bounce.ShellCommand.ExecuteAndExpectSuccess(@"VeryLongRunningConsole\bin\Debug\VeryLongRunningConsole", "");
        }
    }

    public class SubBounce : Task {
        [Dependency] public Task<string> BounceArguments;
        [Dependency] public Task<string> WorkingDirectory;

        public override void Build(IBounce bounce) {
            var cwd = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(WorkingDirectory.Value);
            try {
                bounce.ShellCommand.ExecuteAndExpectSuccess(@"bounce.exe", BounceArguments.Value);
            } finally {
                Directory.SetCurrentDirectory(cwd);
            }
        }
    }
}
