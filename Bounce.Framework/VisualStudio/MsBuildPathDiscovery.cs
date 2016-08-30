using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework.VisualStudio {
    class MsBuildPathDiscovery {

        public List<string> MsBuildLocations;
        private readonly IFileSystemWrapper Fs;

        public MsBuildPathDiscovery(IFileSystemWrapper fs = null) {
            Fs = fs ?? new FileSystemWrapper();

            MsBuildLocations = new List<string> {
                Environment.ExpandEnvironmentVariables(@"%MSBuild%"),
                Environment.ExpandEnvironmentVariables(@"%SystemDrive%\Program Files\MSBuild\14.0\Bin\msbuild.exe"),
                Environment.ExpandEnvironmentVariables(@"%SystemDrive%\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"),
                Environment.ExpandEnvironmentVariables(@"%SystemDrive%\Program Files\MSBuild\12.0\Bin\msbuild.exe"),
                Environment.ExpandEnvironmentVariables(@"%SystemDrive%\Program Files (x86)\MSBuild\12.0\Bin\msbuild.exe"),
                Environment.ExpandEnvironmentVariables(@"%SystemDrive%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe")
            };

            if (MsBuildLocations.First() == "%MSBuild%") {
                MsBuildLocations.RemoveAt(0);
            }
        }

        public MsBuildPath LocateMostRecentMsBuildPath() {
            var searchPaths = new List<string>();

            foreach (var location in MsBuildLocations) {
                searchPaths.Add(location);
                if (Fs.FileExists(location)) {
                    return new MsBuildPath(location, new List<string>(searchPaths));
                }
            }

            return new MsBuildPath(MsBuildLocations.Last(), new List<string>(searchPaths));
        }

    }

    class MsBuildPath {
        public string Selected { get; set; }
        public List<string> SearchLocations { get; set; }

        public MsBuildPath(string selected, List<string> searchLocations) {
            Selected = selected;
            SearchLocations = searchLocations;
        }

        public override string ToString() {
            return Selected;
        }

        public static implicit operator string(MsBuildPath p) {
            return p.ToString();
        }
    }

    public class FileSystemWrapper : IFileSystemWrapper {
        public bool FileExists(string path) {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path) {
            return Directory.Exists(path);
        }
    }

    public interface IFileSystemWrapper {
        bool FileExists(string path);
        bool DirectoryExists(string path);
    }
}