using System.IO;

namespace Bounce.Framework {
    public interface ICommandLogFactory {
        ICommandLog Create(string command, string args, TextWriter stdout, TextWriter stderr, LogOptions logOptions);
    }

    class CommandLogFactory : ICommandLogFactory {
        public ICommandLog Create(string command, string args, TextWriter stdout, TextWriter stderr, LogOptions logOptions) {
            if (logOptions.CommandOutput) {
                return new CommandLog(command, args, stdout, stderr);
            } else {
                return new NullCommandLog(args);
            }
        }
    }
}