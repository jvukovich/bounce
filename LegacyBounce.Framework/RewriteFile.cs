using System;
using System.IO;

namespace LegacyBounce.Framework
{
    public class RewriteFile : Task
    {
        [Dependency]
        public Task<string> FilePath;
        public Func<string, string> Rewriter;

        public override void Build()
        {
            string from = File.ReadAllText(FilePath.Value);
            string to = Rewriter(from);
            File.WriteAllText(FilePath.Value, to);
        }
    }
}
