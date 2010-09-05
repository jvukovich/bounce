using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.BuildEngine;
using System.IO;

namespace Bounce {
    public class VisualStudioSolution : IIisWebSiteDirectory {
        public IValue<string> SolutionPath;
        public IValue<string> OutputProjectName;
        public IEnumerable<string> DllPaths { get; private set; }

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {SolutionPath}; }
        }

        public void Build() {
            Console.WriteLine("building solution at: " + SolutionPath.Value);

            new ShellCommandExecutor().ExternalBuild("msbuild.exe", SolutionPath.Value, "msbiuld exited funny");

            LastBuilt = DateTime.UtcNow;

            var binPath = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SolutionPath.Value), OutputProjectName.Value), @"bin\Debug");
            DllPaths = Directory.GetFiles(binPath);
        }

        public void Clean() {
            Console.WriteLine("cleaning solution at: " + SolutionPath.Value);
        }

        public DateTime? LastBuilt { get; set; }

        public string Path {
            get { return SolutionPath.Value; }
        }
    }
}