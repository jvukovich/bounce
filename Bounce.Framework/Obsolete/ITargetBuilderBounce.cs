using System.IO;

namespace Bounce.Framework.Obsolete {
    public interface ITargetBuilderBounce : IBounce {
        ITaskScope TaskScope(ITask task, IBounceCommand command, string targetName);
        TextWriter DescriptionOutput { get; }
    }
}