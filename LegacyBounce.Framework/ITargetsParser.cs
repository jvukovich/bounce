using System.Collections.Generic;

namespace LegacyBounce.Framework {
    public interface ITargetsParser {
        IDictionary<string, IObsoleteTask> ParseTargetsFromObject(object targets);
    }
}