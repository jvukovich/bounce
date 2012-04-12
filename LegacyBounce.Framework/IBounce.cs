using System.Collections.Generic;

namespace LegacyBounce.Framework {
    public interface IBounce {
        ILog Log { get; }
        IShellCommandExecutor ShellCommand { get; }
        LogOptions LogOptions { get; }
        ITaskLogFactory LogFactory { get; set; }
        IEnumerable<IParameter> ParametersGiven { get; }
        void Invoke(IBounceCommand command, IObsoleteTask task);
    }
}