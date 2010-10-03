using System;

namespace Bounce.Console {
    public class TargetsAssemblyArgumentsParser {
        private readonly ITargetsAssemblyFinder TargetsAssemblyFinder;

        public TargetsAssemblyArgumentsParser() : this(new TargetsAssemblyFinder()) {}

        public TargetsAssemblyArgumentsParser(ITargetsAssemblyFinder targetsAssemblyFinder) {
            TargetsAssemblyFinder = targetsAssemblyFinder;
        }

        public TargetsAssemblyAndArguments GetTargetsAssembly(string[] args) {
            if (args.Length > 0) {
                var firstArg = args[0];
                if (firstArg.StartsWith("/targets:")) {
                    var remainingArgs = new string[args.Length - 1];
                    Array.Copy(args, 1, remainingArgs, 0, remainingArgs.Length);
                    return new TargetsAssemblyAndArguments {
                        TargetsAssembly = firstArg.Substring("/targets:".Length),
                        RemainingArguments = remainingArgs,
                    };
                } else if (firstArg == "/targets") {
                    var remainingArgs = new string[args.Length - 2];
                    Array.Copy(args, 2, remainingArgs, 0, remainingArgs.Length);
                    return new TargetsAssemblyAndArguments {
                        TargetsAssembly = args[1],
                        RemainingArguments = remainingArgs,
                    };
                }
            }

            var targets = TargetsAssemblyFinder.FindTargetsAssembly();
            if (targets != null) {
                return new TargetsAssemblyAndArguments {
                    TargetsAssembly = targets,
                    RemainingArguments = args,
                };
            } else {
                throw new TargetsAssemblyNotFoundException();
            }
        }
    }
}