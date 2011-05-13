using System;

namespace Bounce.Framework {
    public interface IBounceCommand {
        void InvokeCommand(Action onBuild, Action onClean, Action onDescribe);
        string InfinitiveTense { get; }
        string PastTense { get; }
        string PresentTense { get; }
        IBounceCommand CleanAfterBuildCommand { get; }
        string CommandLineCommand { get; }
        bool IsLogged { get; }
    }
}