using System.IO;

namespace Bounce.Framework.Obsolete {
    public interface ITargetBuilderBounce : IBounce {
        ITaskScope TaskScope(IObsoleteTask task, IBounceCommand command, string targetName);
        TextWriter DescriptionOutput { get; }
    }
}