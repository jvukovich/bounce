using System.IO;

namespace LegacyBounce.Framework {
    public interface ITargetBuilderBounce : IBounce {
        ITaskScope TaskScope(IObsoleteTask task, IBounceCommand command, string targetName);
        TextWriter DescriptionOutput { get; }
    }
}