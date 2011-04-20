using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Bounce.Framework;

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

        public static object RealTargets(IParameters buildParameters) {
            var version = buildParameters.Required<string>("version");

            var git = new GitCheckout {
                Repository = "git://github.com/refractalize/bounce.git",
                Directory = "tmp2",
            };
            var solution = new VisualStudioSolution {
                SolutionPath = "Bounce.sln",
            };
            var frameworkProject = solution.Projects["Bounce.Framework"];

            var downloadsDir = new CleanDirectory {
                Path = "Downloads",
            };

            var frameworkZip = new ZipFile {
                Directory = frameworkProject.WhenBuilt(() => Path.GetDirectoryName(frameworkProject.OutputFile.Value)),
                ZipFileName = downloadsDir.Files[version.WhenBuilt(() => string.Format("Bounce.Framework.{0}.zip", version.Value))],
            };

            var downloads = new All(frameworkZip, new GitTag {Directory = ".", Tag = version.WhenBuilt(() => "v" + version.Value)});

            return new {
                Tests = new NUnitTests {
                    DllPaths = solution.Projects.Select(p => p.OutputFile),
                },
                Downloads = downloads,
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

    public class MultiStage2 {
        public static object GetTargets (IParameters parameters) {
            var archive = new Archive(parameters.Default("archive", false), "archive");
            var solution = new VisualStudioSolution {
                SolutionPath = @"C:\Users\Public\Documents\Development\BigSolution\BigSolution.sln",
            };
            var service = archive.Begin("service", solution.Projects["BigWindowsService"].ProjectDirectory);
            var tests = archive.Begin("tests", solution.Projects["BigSolution.Tests"].ProjectDirectory);

            return new {
                Service = new PrintTask(Console.Out) { Description = service },
                Tests = new PrintTask(Console.Out) { Description = tests },
            };
        }
    }

    public class MultiStage {
        public static object GetTargets (IParameters parameters) {
            var archive = new Archive(
                parameters.Default("archive", false),
                "archive"
            );

            var beforeArchive1 = new PrintTaskWithPath() {
                Description = "before archive 1",
                Path = @"fromdir1",
            };
            var beforeArchive2 = new PrintTaskWithPath() {
                Description = "before archive 2",
                Path = @"fromdir2",
            };

            var webServiceDirectory1 = archive.Begin(
                "WebSite",
                beforeArchive1.Path
            );
            var webServiceDirectory2 = archive.Begin(
                "WebSite",
                beforeArchive2.Path
            );

            var webSite = new PrintTaskWithPath {
                Description = "after archive",
                Path = webServiceDirectory1,
            };

            return new {
                Service = archive.End(webSite),
            };
        }
    }

    public class Archive : Task {
        public Task<bool> IsArchive;
        private Task<string> ArchiveDirectory;

        private List<Task<string>> ProjectDirectories;

        public Archive(Task<bool> isArchive, Task<string> archiveDirectory) {
            IsArchive = isArchive;
            ArchiveDirectory = archiveDirectory;
            ProjectDirectories = new List<Task<string>>();
        }

        public Task<string> Begin(string service, Task<string> projectDirectory) {
            ArchiveDirectory = "archive";
            ProjectDirectories.Add(projectDirectory);
            return new ArchivePath(projectDirectory, IsArchive, ArchiveDirectory);
        }

        class ArchivePath : TaskWithValue<string> {
            [Dependency] private readonly Task<string> Directory;
            [Dependency] private readonly Task<bool> IsArchive;
            [Dependency] private readonly Task<string> ArchiveDirectory;
            [Dependency] private readonly ITask ArchivedDirectory;

            public ArchivePath(Task<string> directory, Task<bool> isArchive, Task<string> archiveDirectory) {
                Directory = directory;
                IsArchive = isArchive;
                ArchivedDirectory = IsArchive.IfTrue(new Copy {FromPath = Directory, ToPath = new CleanDirectory {Path = archiveDirectory}.Path});
                ArchiveDirectory = archiveDirectory;
            }

            protected override string GetValue() {
                return Path.Combine(ArchiveDirectory.Value, Path.GetFileName(Directory.Value));
            }
        }

        public ITask End(ITask task) {
            return IsArchive.SelectTask(isArchive => {
                if (isArchive) {
                    return new MultiTask<Task<string>>(ProjectDirectories);
                } else {
                    return task;
                }
            });
        }

        public class MultiTask<TOutput> : EnumerableFuture<TOutput> where TOutput : ITask {
            private readonly IEnumerable<TOutput> Tasks;

            public MultiTask(IEnumerable<TOutput> tasks) {
                Tasks = tasks;
            }

            public override IEnumerable<TOutput> GetTasks(IBounce bounce) {
                return Tasks;
            }
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
        public ITask CreateRemoteBounce(Task<string> bounceArguments, Task<string> workingDirectory, Task<string> machine)
        {
            return new SubBounce {
                BounceArguments = bounceArguments,
                WorkingDirectory = workingDirectory,
            };
        }
    }

    public class Stuff
    {
        [Targets]
        public static object GetTargets(IParameters parameters)
        {
            var stage = parameters.Default("stage", "buildDeploy");
            var webName = parameters.Required<string>("webName");

            var machines = new[] {
                new DeployMachine {
                    RemotePath = @"\\localhost\deploy",
                    LocalPath = @"c:\Deploy",
                    BounceParameters = new [] {webName.WithValue("heart")},
                },
            };

            var targets = new StagedDeployTargets(stage, machines, new SubBounceFactory());

            var website = targets.CreateTarget("Website");
            website.Build = new Copy {FromPath = "built", ToPath = new CleanDirectory {Path = "tmp"}.Path}.ToPath;
            website.Deploy = archive => new Copy {
                FromPath = archive.SubPath("built"),
                ToPath = archive.SubPath(webName)
            };

            return targets.Targets;
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
