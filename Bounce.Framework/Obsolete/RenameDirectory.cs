using System;
using System.IO;

namespace Bounce.Framework.Obsolete
{
    public class RenameDirectory : Task
    {
        [Dependency] public Task<string> From;
        [Dependency] public Task<string> To;

        public override void Build(IBounce bounce)
        {
            if (Directory.Exists(To.Value))
                throw new ArgumentException("To directory '" + To.Value + "' already exists.");
            if (Directory.Exists(From.Value))
                Directory.Move(From.Value, To.Value);
            else
                throw new ArgumentException("From directory '" + From.Value + "' does not seem to to exist.");
        }
    }
}