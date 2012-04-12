using System.Collections.Generic;

namespace LegacyBounce.Framework {
    public interface IGitCommand {
        void Pull(string workingDirectory, ILog log, IBounce bounce);
        void Clone(string repo, string directory, IDictionary<string, string> options, ILog log, IBounce bounce);
        void Tag(string tag, bool force, IBounce bounce);
    }
}