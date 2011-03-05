using System;
using System.Collections.Generic;

namespace Bounce.Console {
    public class TargetsAssemblyArgumentsParser {
        private readonly ITargetsAssemblyFinder TargetsAssemblyFinder;

        public TargetsAssemblyArgumentsParser() : this(new TargetsAssemblyFinder()) {}

        public TargetsAssemblyArgumentsParser(ITargetsAssemblyFinder targetsAssemblyFinder) {
            TargetsAssemblyFinder = targetsAssemblyFinder;
        }

        private void TryGetTargetsFromArguments(OptionsAndArguments optionsAndArguments)
        {
            var args = optionsAndArguments.RemainingArguments;
            if (args.Length > 0)
            {
                var firstArg = args[0];
                if (firstArg.StartsWith("/targets:"))
                {
                    var remainingArgs = new string[args.Length - 1];
                    Array.Copy(args, 1, remainingArgs, 0, remainingArgs.Length);
                    optionsAndArguments.TargetsAssembly = new BounceDirectoryExecutable {
                        Executable = firstArg.Substring("/targets:".Length),
                        ExecutableType = BounceDirectoryExecutableType.Targets
                    };
                    optionsAndArguments.RemainingArguments = remainingArgs;
                }
                else if (firstArg == "/targets")
                {
                    var remainingArgs = new string[args.Length - 2];
                    Array.Copy(args, 2, remainingArgs, 0, remainingArgs.Length);
                    optionsAndArguments.TargetsAssembly = new BounceDirectoryExecutable {
                        Executable = args[1],
                        ExecutableType = BounceDirectoryExecutableType.Targets
                    };
                    optionsAndArguments.RemainingArguments = remainingArgs;
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

            if (optionsAndArguments.TargetsAssembly != null)
            {
                return optionsAndArguments;
            }

            var targets = TargetsAssemblyFinder.FindTargetsAssembly();
            if (targets != null) {
                optionsAndArguments.TargetsAssembly = targets;
                return optionsAndArguments;
            } else {
                throw new TargetsAssemblyNotFoundException();
            }
        }
    }
}