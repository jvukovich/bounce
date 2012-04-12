using System;

namespace Bounce.Framework.Obsolete {
    public interface ILog {
        void Debug(string format, params object[] args);
        void Debug(object message);

        void Info(string format, params object[] args);
        void Info(object message);

        void Warning(string format, params object[] args);
        void Warning(Exception exception, string format, params object[] args);
        void Warning(object message);
        void Warning(Exception exception, object message);

        void Error(string format, params object[] args);
        void Error(Exception exception, string format, params object[] args);
        void Error(object message);
        void Error(Exception exception, object message);

        ICommandLog BeginExecutingCommand(string command, string args);
        ITaskLog TaskLog { get; }
    }
}