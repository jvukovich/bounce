using System;
using System.IO;

namespace Bounce.Framework.Obsolete
{
    public class RenameFile : Task
    {
        [Dependency] public Task<string> From;
        [Dependency] public Task<string> To;

        public override void Build(IBounce bounce)
        {
            if (File.Exists(To.Value))
                throw new ArgumentException("To file '" + To.Value + "' already exists.");
            if (File.Exists(From.Value))
                File.Move(From.Value, To.Value);
            else
                throw new ArgumentException("From file '" + From.Value + "' does not seem to to exist.");
        }
    }
}