using System;
using System.Text.RegularExpressions;

namespace Bounce.Framework
{
    public abstract class WindowsServiceBaseTask : Task
    {
        [Dependency]
        public Task<string> Name;

        [Dependency]
        public Task<string> Machine;

        protected WindowsServiceBaseTask()
        {
            Machine = "localhost";
        }

        protected abstract void BuildTask(IBounce bounce);
        
        public override void Build(IBounce bounce)
        {
            BuildTask(bounce);
        }

        protected void ExecuteScAndExpectSuccess(IBounce bounce, string commandArgs, params object[] args)
        {
            bounce.ShellCommand.ExecuteAndExpectSuccess("sc", OnMachine(commandArgs, args));
        }

        protected string OnMachine(string commandArgs, params object [] args) {
            return String.Format(@"\\{0} {1}", Machine.Value, String.Format(commandArgs, args));
        }

        protected void StopService(IBounce bounce) {
            ExecuteScAndExpectSuccess(bounce, @"stop ""{0}""", Name.Value);
        }

        protected ProcessOutput ExecuteSc(IBounce bounce, string commandArgs, params object[] args) {
            return bounce.ShellCommand.Execute("sc", OnMachine(commandArgs, args));
        }

        protected bool IsServiceStarted(IBounce bounce)
        {
            return IsServiceStatus(bounce, "started");
        }

        protected bool IsServiceStopped(IBounce bounce)
        {
            return IsServiceStatus(bounce, "stopped");
        }

        private bool IsServiceStatus(IBounce bounce, string status)
        {
            Regex statePattern = new Regex(@"STATE\s+:\s+\d\s+" + status.ToUpper(), RegexOptions.IgnoreCase);
            var queryOutput = ExecuteSc(bounce, @"query ""{0}""", Name.Value).Output;
            return statePattern.Match(queryOutput).Success;
        }

        protected bool IsServiceInstalled(IBounce bounce)
        {
            return !ExecuteSc(bounce, @"query ""{0}""", Name.Value)
                        .Output
                        .Contains("service does not exist");
        }
    }
}