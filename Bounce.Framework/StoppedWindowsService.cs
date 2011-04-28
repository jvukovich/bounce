using System;
using System.Threading;

namespace Bounce.Framework
{
    public class StoppedWindowsService : WindowsServiceBaseTask
    {
        public Task<TimeSpan> Timeout;

        public StoppedWindowsService()
        {
            Timeout = TimeSpan.FromMinutes(5);
        }

        protected override void BuildTask(IBounce bounce)
        {
            if (IsServiceInstalled(bounce))
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
        }

        private bool WaitedLongerThanTimeout(DateTime timeWhenAskedToStop)
        {
            return DateTime.Now.Subtract(timeWhenAskedToStop).CompareTo(Timeout.Value) > 0;
        }
    }
}