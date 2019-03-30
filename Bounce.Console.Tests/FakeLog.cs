using System;
using Bounce.Framework;

namespace Bounce.Console.Tests
{
    public sealed class FakeLog : ILog
    {
        public void Debug(string format, params object[] args)
        {
        }

        public void Debug(object message)
        {
        }

        public void Info(string format, params object[] args)
        {
        }

        public void Info(object message)
        {
        }

        public void Warning(string format, params object[] args)
        {
        }

        public void Warning(Exception exception, string format, params object[] args)
        {
        }

        public void Warning(object message)
        {
        }

        public void Warning(Exception exception, object message)
        {
        }

        public void Error(string format, params object[] args)
        {
        }

        public void Error(Exception exception, string format, params object[] args)
        {
        }

        public void Error(object message)
        {
        }

        public void Error(Exception exception, object message)
        {
        }

        public ICommandLog BeginExecutingCommand(string command, string args)
        {
            return new FakeCommandLog(command, args);
        }
    }

    public class FakeCommandLog : ICommandLog
    {
        private readonly string Args;

        public FakeCommandLog(string command, string args)
        {
            Args = args;
        }

        public void CommandOutput(string output)
        {
        }

        public void CommandError(string error)
        {
        }

        public void CommandComplete(int exitCode)
        {
        }

        public string CommandArgumentsForLogging
        {
            get { return Args; }
        }
    }
}