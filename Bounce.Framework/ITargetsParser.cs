using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public interface ITargetsParser {
        IDictionary<string, ITask> ParseTargetsFromObject(object targets);
    }
}