using System;
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

    public class MultiStage {
        [Targets]
        public static object GetTargets (IParameters parameters) {
            var archive = new Archive(parameters);
            var solution = new VisualStudioSolution {
                SolutionPath = @"C:\Users\Public\Documents\Development\BigSolution\BigSolution.sln",
            };
            var service = archive.Add("service", solution.Projects["BigWindowsService"].ProjectDirectory);
            var tests = archive.Add("tests", solution.Projects["BigSolution.Tests"].ProjectDirectory);

            return new {
                Service = new PrintTask(Console.Out) { Description = service },
                Tests = new PrintTask(Console.Out) { Description = tests },
            };
        }
    }

    public class Archive : Task {
        public Task<bool> IsArchive;
        public CleanDirectory ArchiveDirectory;

        public Archive(IParameters parameters) {
            IsArchive = parameters.Default("archive", false);
            ArchiveDirectory = new CleanDirectory() {Path = "archive"};
        }

        public Task<string> Add(string service, Task<string> projectDirectory) {
//            return IsArchive.IfTrue(new Copy {FromPath = projectDirectory, ToPath = ArchiveDirectory.Path}.ToPath, projectDirectory);
            return new ArchivePath(projectDirectory, IsArchive, ArchiveDirectory);
        }

        class ArchivePath : TaskWithValue<string> {
            [Dependency] private readonly Task<string> Directory;
            [Dependency] private readonly Task<bool> IsArchive;
            [Dependency] private readonly CleanDirectory ArchiveDirectory;
            [Dependency] private readonly ITask ArchivedDirectory;

            public ArchivePath(Task<string> directory, Task<bool> isArchive, CleanDirectory archiveDirectory) {
                Directory = directory;
                IsArchive = isArchive;
                ArchivedDirectory = IsArchive.IfTrue(new Copy {FromPath = Directory, ToPath = archiveDirectory.Path});
                ArchiveDirectory = archiveDirectory;
            }

            protected override string GetValue() {
                if (IsArchive.Value) {
                    return Path.Combine(ArchiveDirectory.Path.Value, Path.GetFileName(Directory.Value));
                } else {
                    return Directory.Value;
                }
            }
        }
    }

    class PrintTask : Task {
        [Dependency]
        public Task<string> Description;

        private readonly TextWriter Output;

        public PrintTask(TextWriter output) {
            Output = output;
        }

        public override void Build() {
            Output.WriteLine(Description.Value);
        }
    }
}
