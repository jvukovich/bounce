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

    class Switch : Task
    {
        [Dependency]
        public Task<string> Condition { get; set; }

        private IDictionary<string, ITask> Cases;

        public Switch(Task<string> condition)
        {
            Condition = condition;
            Cases = new Dictionary<string, ITask>();
        }

        public ITask this [string _case]
        {
            get { return Cases[_case]; }
            set { Cases[_case] = value; }
        }

        public override void Invoke(IBounceCommand command, IBounce bounce)
        {
            ITask action;
            if (Cases.TryGetValue(Condition.Value, out action)) {
                bounce.Invoke(command, action);
            } else {
                throw new ConfigurationException(String.Format("no such case for `{0}'", Condition.Value));
            }
        }
    }

    public class StagedDeployer
    {
        public IDictionary<string, ITask> Targets { get; private set; }
        private readonly Task<IEnumerable<DeployMachine>> MachineConfigurations;
        private readonly IRemoteBounceFactory RemoteBounceFactory;
        private readonly Parameter<string> Stage;

        public StagedDeployer(Parameter<string> stage, Task<IEnumerable<DeployMachine>> machineConfigurations, IRemoteBounceFactory remoteBounceFactory)
        {
            Targets = new Dictionary<string, ITask>();
            MachineConfigurations = machineConfigurations;
            RemoteBounceFactory = remoteBounceFactory;
            Stage = stage;
        }

        public StagedDeploy CreateDeployment(string name)
        {
            return new StagedDeploy(Targets, name, Stage, MachineConfigurations, RemoteBounceFactory);
        }
    }

    public class StagedDeploy : Task
    {
        private readonly string TargetName;

        private readonly Parameter<string> Stage;
        [Dependency]
        private readonly Task<IEnumerable<DeployMachine>> MachineConfigurations;

        private readonly IRemoteBounceFactory RemoteBounceFactory;

        [Dependency]
        private readonly Switch Switch;

        public StagedDeploy(IDictionary<string, ITask> targets, string targetName, Parameter<string> stage, Task<IEnumerable<DeployMachine>> machineConfigurations, IRemoteBounceFactory remoteBounceFactory)
        {
            TargetName = targetName;
            Stage = stage;
            MachineConfigurations = machineConfigurations;
            RemoteBounceFactory = remoteBounceFactory;
            Switch = new Switch(stage);
            targets[targetName] = this;
        }

        private void SetupSwitch() {
            if (Build != null && Deploy != null) {
                var buildWithBounce = CopyBounceDirectoryIntoArchive(Build);

                Switch["build"] = buildWithBounce;
                Switch["remoteDeploy"] = GetRemoteDeploy(".");
                Switch["deploy"] = Deploy(".");
                Switch["buildRemoteDeploy"] = GetRemoteDeploy(buildWithBounce);
                Switch["buildDeploy"] = Deploy(Build);
            }
        }

        private ITask GetRemoteDeploy(Task<string> archive) {
            return MachineConfigurations.SelectTasks(machConf => {
                var archiveOnRemote = new Copy {
                    FromPath = archive,
                    ToPath = machConf.RemotePath,
                };

                var parameters = new List<IParameter>();
                parameters.Add(Stage.WithValue("deploy"));
                parameters.AddRange(machConf.BounceParameters);

                var localPath = new All(archiveOnRemote, machConf.LocalPath).WhenBuilt(() => machConf.LocalPath.Value);
                return RemoteBounceFactory.CreateRemoteBounce(BounceArguments.ForTarget(TargetName, parameters), localPath);
            });
        }

        private Func<Task<string>, ITask> _deploy;
        public Func<Task<string>, ITask> Deploy {
            get { return _deploy; }
            set {
               _deploy = value;
                SetupSwitch();
            }
        }

        private Task<string> _build;
        public new Task<string> Build {
            get { return _build; }
            set {
                _build = value;
                SetupSwitch();
            }
        }

        private Task<string> CopyBounceDirectoryIntoArchive(Task<string> archive) {
            return new Copy {
                FromPath = Path.GetDirectoryName(BounceRunner.TargetsPath),
                ToPath = archive.SubPath("Bounce"),
            }.ToPath.SubPath("..");
        }
    }

    public static class BounceArguments {
        public static RemoteBounceArguments ForTarget(string target, params IParameter [] parameters) {
            return ForTarget(target, (IEnumerable<IParameter>) parameters);
        }

        public static RemoteBounceArguments ForTarget(string target, IEnumerable<IParameter> parameters) {
            return new RemoteBounceArguments {Targets = new [] {target}, Parameters = parameters};
        }
    }

    public interface IRemoteBounceFactory {
        ITask CreateRemoteBounce(Task<string> bounceArguments, Task<string> workingDirectory);
    }

    class SubBounceFactory : IRemoteBounceFactory {
        public ITask CreateRemoteBounce(Task<string> bounceArguments, Task<string> workingDirectory) {
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

            var deployer = new StagedDeployer(stage, machines, new SubBounceFactory());

            var website = deployer.CreateDeployment("Website");
            website.Build = new Copy {FromPath = "built", ToPath = new CleanDirectory {Path = "tmp"}.Path}.ToPath;
            website.Deploy = archive => new Copy {
                FromPath = archive.SubPath("built"),
                ToPath = archive.SubPath(webName)
            };

            return deployer.Targets;
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

    public class DeployMachine
    {
        public Task<string> RemotePath;
        public Task<string> LocalPath;
        public IEnumerable<IParameter> BounceParameters = new IParameter[0];
    }
}
