using System;

namespace Bounce.Framework.Obsolete {
    class BounceCommandBuild : IBounceCommand {
        public BounceCommandBuild(IBounceCommand cleanAfterBuildCommand, string commandLineCommand) {
            CleanAfterBuildCommand = cleanAfterBuildCommand;
            CommandLineCommand = commandLineCommand;
        }

        public void InvokeCommand(Action onBuild, Action onClean, Action onDescribe) {
            onBuild();
        }

        public string InfinitiveTense {
            get { return "build"; }
        }

        public string PastTense {
            get { return "built"; }
        }

        public string PresentTense {
            get { return "building"; }
        }

        public IBounceCommand CleanAfterBuildCommand { get; private set; }

        public string CommandLineCommand { get; private set; }

        public bool IsLogged {
            get { return true; }
        }
    }

    internal interface IBounceCommandParser {
        IBounceCommand Parse(string command);
        IBounceCommand Build { get; }
        IBounceCommand Clean { get; }
    }

    class BounceCommandParser : IBounceCommandParser {
        private IBounceCommand _build;
        private IBounceCommand _buildAndClean;
        private IBounceCommand _clean;
        private IBounceCommand _describe;

        public IBounceCommand Parse(string command) {
            switch (command) {
                case "build":
                    return Build;
                case "buildandclean":
                    return BuildAndClean;
                case "clean":
                    return Clean;
                case "describe":
                    return Describe;
                default:
                    return null;
            }
        }

        public IBounceCommand Build {
            get {
                if (_build == null) {
                    _build = new BounceCommandBuild(null, "build");
                }
                return _build;
            }
        }

        public IBounceCommand BuildAndClean {
            get {
                if (_buildAndClean == null) {
                    _buildAndClean = new BounceCommandBuild(Clean, "buildandclean");
                }
                return _buildAndClean;
            }
        }

        public IBounceCommand Clean {
            get {
                if (_clean == null) {
                    _clean = new BounceCommandClean();
                }
                return _clean;
            }
        }

        public IBounceCommand Describe {
            get {
                if (_describe == null) {
                    _describe = new BounceCommandDescribe();
                }
                return _describe;
            }
        }
    }

    internal class BounceCommandClean : IBounceCommand {
        public void InvokeCommand(Action onBuild, Action onClean, Action onDescribe) {
            onClean();
        }

        public string InfinitiveTense {
            get { return "clean"; }
        }

        public string PastTense {
            get { return "cleaned"; }
        }

        public string PresentTense {
            get { return "cleaning"; }
        }

        public IBounceCommand CleanAfterBuildCommand {
            get { return null; }
        }

        public string CommandLineCommand {
            get { return "clean"; }
        }

        public bool IsLogged {
            get { return true; }
        }
    }

    internal class BounceCommandDescribe : IBounceCommand {
        public void InvokeCommand(Action onBuild, Action onClean, Action onDescribe) {
            onDescribe();
        }

        public string InfinitiveTense {
            get { return "describe"; }
        }

        public string PastTense {
            get { return "described"; }
        }

        public string PresentTense {
            get { return "describing"; }
        }

        public IBounceCommand CleanAfterBuildCommand {
            get { return null; }
        }

        public string CommandLineCommand {
            get { return "describe"; }
        }

        public bool IsLogged {
            get { return false; }
        }
    }
}