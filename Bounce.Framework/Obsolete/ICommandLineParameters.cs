using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public interface ICommandLineParameters : IParameters {
        IEnumerable<IParameter> ParseCommandLineArguments(List<ParsedCommandLineParameter> parameters);
        IEnumerable<IParameter> Parameters { get; }
    }
}