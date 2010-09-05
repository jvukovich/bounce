using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.BuildEngine;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Bounce.VisualStudio.Tests
{
    [TestFixture]
    public class Scratch
    {
        [Test, RequiresSTA]
        public void ShouldBuildSolution() {
            var engine = new Engine();
//            engine.RegisterLogger(new BounceVSLogger());
            var logger = new FileLogger();

            // Set the logfile parameter to indicate the log destination
            logger.Parameters = @"logfile=C:\Users\Public\Documents\Development\Bounce\build.log";

            // Register the logger with the engine
            engine.RegisterLogger(logger);
            Assert.That(engine.BuildProjectFile(@"C:\Users\Public\Documents\Development\Bounce\Bounce.sln"));
        }
    }
}
