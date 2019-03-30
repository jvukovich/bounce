using System;
using System.Collections;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace Bounce.Framework.VisualStudio {
	public struct MsBuildToolsVersion {
		public const string Version2 = "2.0";
		public const string Version35 = "3.5";
		public const string Version4 = "4.0";
		public const string Version12 = "12.0";
		public const string Version14 = "14.0";
		public const string VersionLatest = "14.0";
	}

	class MsBuild : IMsBuild {
		private const string msbuildExe = "msbuild.exe";
		private readonly IShell Shell;
		public string MsBuildExe { get; set; }

		public MsBuild(IShell shell) {
			Shell = shell;
			MsBuildExe = Path.Combine(GetMsBuildToolsPath(MsBuildToolsVersion.VersionLatest), msbuildExe);
		}

		public void Build(string projSln, string config, string outputDir, string target, string verbosity, bool nologo, bool parallel, string msBuildToolsVersion, string customMsBuildToolsPath = null) {
			var arguments = NormaliseArguments(
				"\"" + projSln + "\"",
				ConfigIfSpecified(config),
				OutputDirIfSpecified(outputDir),
				TargetIfSpecified(target),
				VerbosityIfSpecified(verbosity),
				NoLogoIfSpecified(nologo),
				ParallelIfSpecified(parallel));

			var msBuildToolsPath = GetMsBuildToolsPath(msBuildToolsVersion, customMsBuildToolsPath);

			MsBuildExe = Path.Combine(msBuildToolsPath, msbuildExe);

			Shell.Exec(MsBuildExe, arguments);
		}

		private string NormaliseArguments(params string[] args) {
			return String.Join(" ", args.Where(a => a != null).ToArray());
		}

		protected string ConfigIfSpecified(string config) {
			return config != null ? "/p:Configuration=" + config : null;
		}

		protected string OutputDirIfSpecified(string outputDir) {
			return outputDir != null ? "/p:Outdir=" + EnsureTrailingSlashIsSet(outputDir) : null;
		}

		protected string TargetIfSpecified(string target) {
			return target != null ? "/t:" + target : null;
		}

		protected string VerbosityIfSpecified(string verbosity) {
			return verbosity != null ? "/verbosity:" + verbosity : null;
		}

		protected string NoLogoIfSpecified(bool nologo) {
			return nologo ? "/nologo" : null;
		}

		protected string ParallelIfSpecified(bool parallel) {
			return parallel ? "/m" : null;
		}

		private string EnsureTrailingSlashIsSet(string outputDir) {
			return outputDir.Last() == Path.DirectorySeparatorChar ? outputDir : outputDir + Path.DirectorySeparatorChar;
		}

		private static string GetMsBuildToolsPath(string msBuildToolsVersion, string customMsBuildToolsPath = null) {
			if (!string.IsNullOrWhiteSpace(customMsBuildToolsPath)) {
				var msBuildFile = Path.Combine(customMsBuildToolsPath, msbuildExe);

				if (!File.Exists(msBuildFile))
					throw new FileNotFoundException(string.Format("Custom MsBuild tools path does not exist: '{0}'", msBuildFile));

				return customMsBuildToolsPath.Trim();
			}

			if (msBuildToolsVersion != MsBuildToolsVersion.Version2 &&
			    msBuildToolsVersion != MsBuildToolsVersion.Version35 &&
			    msBuildToolsVersion != MsBuildToolsVersion.Version4 &&
			    msBuildToolsVersion != MsBuildToolsVersion.Version12 &&
			    msBuildToolsVersion != MsBuildToolsVersion.Version14 &&
			    msBuildToolsVersion != MsBuildToolsVersion.VersionLatest)
				throw new IndexOutOfRangeException(string.Format("msBuildToolsVersion '{0}' was not found or is unsupported at this time. The latest version supported by Bounce is '{1}'.", msBuildToolsVersion, MsBuildToolsVersion.VersionLatest));

            // todo: netstandard
			//var keyPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions\" + msBuildToolsVersion, false);

			//if (keyPath == null)
			//	ThrowMsBuildToolsPathNotFoundExeception(msBuildToolsVersion);

			//var msBuildToolsPath = keyPath.GetValue("MSBuildToolsPath");

			//if (msBuildToolsPath == null)
			//	ThrowMsBuildToolsPathNotFoundExeception(msBuildToolsVersion);

			//return msBuildToolsPath.ToString();
		    return string.Empty; // todo: netstandard
		}

		private static void ThrowMsBuildToolsPathNotFoundExeception(string msBuildToolsVersion) {
			throw new Exception(string.Format("Unable to find MsBuild tools path for specified version '{0}'", msBuildToolsVersion));
		}
	}
}