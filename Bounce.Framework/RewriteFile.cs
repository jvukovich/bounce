using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bounce.Framework
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
