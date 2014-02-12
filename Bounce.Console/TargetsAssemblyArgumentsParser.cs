using System;
using System.Collections.Generic;
using System.IO;

namespace Bounce.Console {
    public class TargetsAssemblyArgumentsParser {
        private readonly IBounceDirectoryFinder _bounceDirectoryFinder;
        private string _bounceDir;

        public TargetsAssemblyArgumentsParser() : this(new BounceDirectoryFinder()) {}

        public TargetsAssemblyArgumentsParser(IBounceDirectoryFinder bounceDirectoryFinder) {
            _bounceDirectoryFinder = bounceDirectoryFinder;
        }

        private void TryGetTargetsFromArguments(OptionsAndArguments optionsAndArguments)
        {
            var args = optionsAndArguments.RemainingArguments;
            if (args.Length > 0)
            {
                var firstArg = args[0];
                _bounceDir = "bounceDir";
                if (firstArg.StartsWith("/" + _bounceDir + ":"))
                {
                    var remainingArgs = new string[args.Length - 1];
                    Array.Copy(args, 1, remainingArgs, 0, remainingArgs.Length);
                    optionsAndArguments.BounceDirectory = firstArg.Substring(("/" + _bounceDir + ":").Length);
                    optionsAndArguments.RemainingArguments = remainingArgs;
                    optionsAndArguments.WorkingDirectory = Directory.GetCurrentDirectory();
                }
                else if (firstArg == "/" + _bounceDir)
                {
                    var remainingArgs = new string[args.Length - 2];
                    Array.Copy(args, 2, remainingArgs, 0, remainingArgs.Length);
                    optionsAndArguments.BounceDirectory = args[1];
                    optionsAndArguments.RemainingArguments = remainingArgs;
                    optionsAndArguments.WorkingDirectory = Directory.GetCurrentDirectory();
                }
            }
        }

        private void TryGetRecurseFromArguments(OptionsAndArguments optionsAndArguments)
        {
            var args = optionsAndArguments.RemainingArguments;
            if (args.Length > 0)
            {
                var firstArg = args[0];
                if (firstArg == "/recurse")
                {
                    var remainingArgs = new string[args.Length - 1];
                    Array.Copy(args, 1, remainingArgs, 0, remainingArgs.Length);
                    optionsAndArguments.Recurse = true;
                    optionsAndArguments.RemainingArguments = remainingArgs;
                }
            }
        }

        public OptionsAndArguments GetTargetsAssembly(string[] args) {
            var optionsAndArguments = new OptionsAndArguments {RemainingArguments = args};

            TryGetTargetsFromArguments(optionsAndArguments);

            TryGetRecurseFromArguments(optionsAndArguments);

            if (optionsAndArguments.BounceDirectory != null)
            {
                return optionsAndArguments;
            }

            var targets = _bounceDirectoryFinder.FindBounceDirectory();
            if (targets != null) {
                optionsAndArguments.BounceDirectory = targets;
                optionsAndArguments.WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(targets));
                return optionsAndArguments;
            } else {
                throw new TargetsAssemblyNotFoundException();
            }
        }
    }
}