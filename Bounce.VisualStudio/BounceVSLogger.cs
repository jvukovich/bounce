using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;

namespace Bounce.VisualStudio
{
    public class BounceVSLogger : ILogger
    {
        public void Initialize(IEventSource eventSource) {
            eventSource.AnyEventRaised += eventSource_AnyEventRaised;
        }

        void eventSource_AnyEventRaised(object sender, BuildEventArgs e) {
            TargetFinishedEventArgs targf = e as TargetFinishedEventArgs;

            if (targf != null && targf.TargetName == "CopyFilesToOutputDirectory") {
                Console.WriteLine("bounce project file: " + targf.ProjectFile);
                Console.WriteLine("bounce message: " + targf.Message);
            }

            TargetStartedEventArgs targs = e as TargetStartedEventArgs;

            if (targs != null && targs.TargetName == "CopyFilesToOutputDirectory") {
                Console.WriteLine("bounce started project file: " + targs.ProjectFile);
                Console.WriteLine("bounce started target file: " + targs.TargetFile);
                Console.WriteLine("bounce started message: " + targs.Message);
            }
//            if (e is ProjectFinishedEventArgs) {
            if (e.Message.Contains("->")) {
//                Console.WriteLine("bounce: " + e.GetType().Name + e.Message);
            }
        }

        public void Shutdown() {
        }

        public LoggerVerbosity Verbosity { get; set; }

        public string Parameters { get; set; }
    }
}
