using System.Collections.Generic;

namespace Bounce.Framework {
    public interface ICommandLineParameters : IParameters {
        void ParseCommandLineArguments(List<ParsedCommandLineParameter> parameters);
        IEnumerable<IParameter> Parameters { get; }
    }
}