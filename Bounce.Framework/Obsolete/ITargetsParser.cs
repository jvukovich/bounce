using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public interface ITargetsParser {
        IDictionary<string, IObsoleteTask> ParseTargetsFromObject(object targets);
    }
}