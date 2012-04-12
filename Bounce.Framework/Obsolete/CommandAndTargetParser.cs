using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework.Obsolete {
    internal class CommandAndTargetParser {
        private IBounceCommandParser BounceCommandParser;

        public CommandAndTargetParser() : this(new BounceCommandParser()) {
        }

        public CommandAndTargetParser(IBounceCommandParser bounceCommandParser) {
            BounceCommandParser = bounceCommandParser;
        }

        public CommandAndTargets ParseCommandAndTargetNames(string[] buildArguments, IDictionary<string, IObsoleteTask> allTargets) {
            IBounceCommand command = BounceCommandParser.Build;
            int targetNamesIndex;
            if (buildArguments.Length > 0 && TryParseCommand(buildArguments[0], ref command)) {
                targetNamesIndex = 1;
            } else {
                targetNamesIndex = 0;
            }
            var targets = ParseTargets(buildArguments, targetNamesIndex);

            return new CommandAndTargets {
                Command = command,
                Targets = TargetsToBuild(allTargets, targets),
            };
        }

        private bool TryParseCommand(string commandString, ref IBounceCommand bounceCommand) {
            var command = BounceCommandParser.Parse(commandString);

            if (command != null) {
                bounceCommand = command;
                return true;
            } else {
                return false;
            }
        }

        private IEnumerable<string> ParseTargets(string[] args, int targetNamesIndex) {
            string [] targets = new string[args.Length - targetNamesIndex];
            Array.Copy(args, targetNamesIndex, targets, 0, targets.Length);
            return targets;
        }

        private IEnumerable<Target> TargetsToBuild(IDictionary<string, IObsoleteTask> targets, IEnumerable<string> targetNames) {
            return targetNames.Select(name => FindTarget(targets, name)).ToArray();
        }

        private Target FindTarget(IDictionary<string, IObsoleteTask> targets, string targetName) {
            try {
                return new Target {Name = targetName, Task = targets[targetName]};
            } catch (KeyNotFoundException) {
                throw new NoSuchTargetException(targetName, targets.Keys);
            }
        }
    }
}