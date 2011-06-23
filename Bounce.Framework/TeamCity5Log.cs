using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bounce.Framework
{
    class TeamCity5Log : TeamCityLog
    {
        public TeamCity5Log(TextWriter output, LogOptions logOptions, ILogMessageFormatter logMessageFormatter)
            : base(output, logOptions, logMessageFormatter)
        {
        }

        public override ICommandLog NUnitLogger(string args, TextWriter output, ICommandLog log)
        {
            return new TeamCity5NUnitLogger(args, output, log);
        }
    }
}
