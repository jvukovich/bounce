using System;

namespace Bounce.Framework {
    public static class BounceCommandExtensions
    {
        public static void InvokeCommand(this BounceCommand command, Action build, Action clean)
        {
            switch (command) {
                case BounceCommand.Build:
                case BounceCommand.BuildAndKeep:
                    build();
                    break;
                case BounceCommand.Clean:
                    clean();
                    break;
                default:
                    throw new ConfigurationException(String.Format("no such command {0}, try build or clean", command));
            }
        }

        public static bool CleanAfterBuild(this BounceCommand command) {
            switch (command) {
                case BounceCommand.Build:
                    return true;
                case BounceCommand.BuildAndKeep:
                case BounceCommand.Clean:
                    return false;
                default:
                    throw new ConfigurationException(String.Format("no such command {0}, try build or clean", command));
            }
        }
    }
}