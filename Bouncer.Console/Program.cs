using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bounce;

namespace Bouncer.Console
{
    class Program
    {
        static void Main(string[] args) {
            CommandLineParameters parameters = CommandLineParameters.ParametersWithUsualTypeParsers();

            var solution = new VisualStudioSolution() { SolutionPath = parameters.Default("sln", @"C:\Users\Public\Documents\Development\Bounce\Bounce.sln"), PrimaryProjectName = parameters.Default("proj", "Bouncer.Console") };
            var targets = new
            {
                Default = new IisWebSite() { WebSiteDirectory = solution },
                Tests = new NUnitTestResults() { DllPaths = solution.Property(s => s.Projects.Select(p => p.OutputFile)) },
            };

            var builder = new TargetBuilder ();

            try {
                if (args.Length >= 2) {
                    string[] buildArguments = parameters.ParseCommandLineArguments(args);

                    string command = buildArguments[0];
                    Action<TargetBuilder, ITarget> commandAction = GetCommand(command);

                    for (int i = 1; i < buildArguments.Length; i++) {
                        string targetName = buildArguments[i];
                        ITarget target = FindTarget(targets, targetName);

                        if (target != null) {
                            commandAction(builder, target);
                        } else {
                            System.Console.WriteLine("no target named {0}", targetName);
                            System.Console.WriteLine("try one of the following:");
                            foreach (var name in GetTargetNames(targets)) {
                                System.Console.WriteLine("  " + name);
                            }
                        }
                    }
                } else {
                    System.Console.WriteLine("usage: bounce build|clean target-name");
                    System.Console.WriteLine();
                    System.Console.WriteLine("targets:");
                    foreach (var name in GetTargetNames(targets)) {
                        System.Console.WriteLine("  " + name);
                    }
                    System.Console.WriteLine();
                    System.Console.WriteLine("arguments:");
                    foreach (var param in parameters.Parameters) {
                        System.Console.Write("  /" + param.Name);
                        if (param.Required) {
                            System.Console.Write(" required");
                        }
                        if (param.HasValue) {
                            System.Console.Write(" default: " + param.DefaultValue);
                        }
                        System.Console.WriteLine();
                    }
                }
            } catch (BuildException ce) {
                System.Console.WriteLine(ce.Message);
                System.Console.Write(ce.Output);
            } catch (ConfigurationException ce) {
                System.Console.WriteLine(ce.Message);
            }
        }

        private static Action<TargetBuilder, ITarget> GetCommand(string command) {
            switch (command.ToLower()) {
                case "build": {
                    return (builder, target) => builder.Build(target);
                }
                case "clean": {
                    return (builder, target) => builder.Clean(target);
                }
                default: {
                    throw new ConfigurationException(String.Format("command {0} not recognised", command));
                }
            }
        }

        private static ITarget FindTarget(object targets, string targetName) {
            PropertyInfo propertyInfo = targets.GetType().GetProperty(targetName);

            if (propertyInfo != null) {
                return (ITarget)propertyInfo.GetValue(targets, new object[0]);
            } else {
                return null;
            }
        }

        private static IEnumerable<string> GetTargetNames(object targets) {
            return targets.GetType().GetProperties().Select(p => p.Name);
        }
    }
}
