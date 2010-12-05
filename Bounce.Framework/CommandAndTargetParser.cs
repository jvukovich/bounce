using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    internal class CommandAndTargetParser {
        public CommandAndTargets ParseCommandAndTargetNames(string[] buildArguments, IDictionary<string, ITask> allTargets) {
            BounceCommand command = BounceCommand.Build;
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

        private bool TryParseCommand(string command, ref BounceCommand bounceCommand) {
            if (command.ToLower() != command) {
                return false;
            }

            try {
                bounceCommand = (BounceCommand) Enum.Parse(typeof (BounceCommand), command, true);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        private IEnumerable<string> ParseTargets(string[] args, int targetNamesIndex) {
            string [] targets = new string[args.Length - targetNamesIndex];
            Array.Copy(args, targetNamesIndex, targets, 0, targets.Length);
            return targets;
        }

        private IEnumerable<Target> TargetsToBuild(IDictionary<string, ITask> targets, IEnumerable<string> targetNames) {
            return targetNames.Select(name => FindTarget(targets, name)).ToArray();
        }

        private Target FindTarget(IDictionary<string, ITask> targets, string targetName) {
            try {
                return new Target {Name = targetName, Task = targets[targetName]};
            } catch (KeyNotFoundException) {
                throw new NoSuchTargetException(targetName, targets.Keys);
            }
        }
    }
}