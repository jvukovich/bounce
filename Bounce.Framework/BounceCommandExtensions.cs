using System;

namespace Bounce.Framework {
    public static class BounceCommandExtensions
    {
        public static void InvokeCommand(this BounceCommand command, Action build, Action clean)
        {
            switch (command) {
                case BounceCommand.Build:
                    build();
                    break;
                case BounceCommand.Clean:
                    clean();
                    break;
                default:
                    throw new ConfigurationException(String.Format("no such command {0}, try build or clean", command));
            }
        }
    }
}