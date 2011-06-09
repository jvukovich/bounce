using System;
using System.Threading;

namespace Bounce.Framework
{
    public class StoppedWindowsService : WindowsServiceBaseTask
    {
        [Dependency]
        public Task<TimeSpan> Timeout;
        [Dependency]
        public Task<TimeSpan> Wait;

        public StoppedWindowsService()
        {
            Timeout = TimeSpan.FromMinutes(5);
            Wait = TimeSpan.FromMinutes(0);
        }

        protected override void BuildTask(IBounce bounce)
        {
            if (IsServiceInstalled(bounce) && !IsServiceStopped(bounce))
            {
                StopService(bounce);

                DateTime timeWhenAskedToStop = DateTime.Now;

                do
                {
                    Thread.Sleep(1000);

                    if (WaitedLongerThanTimeout(timeWhenAskedToStop))
                    {
                        throw new BounceException(String.Format("service {0} could not be stopped", Name));
                    }
                } while (!IsServiceStopped(bounce));

                Thread.Sleep(Wait.Value);
            }
        }

        private bool WaitedLongerThanTimeout(DateTime timeWhenAskedToStop)
        {
            return DateTime.Now.Subtract(timeWhenAskedToStop).CompareTo(Timeout.Value) > 0;
        }
    }
}