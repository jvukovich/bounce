using System.Collections.Generic;

namespace LegacyBounce.Framework {
    public interface ICommandLineParameters : IParameters {
        IEnumerable<IParameter> ParseCommandLineArguments(List<ParsedCommandLineParameter> parameters);
        IEnumerable<IParameter> Parameters { get; }
    }
}