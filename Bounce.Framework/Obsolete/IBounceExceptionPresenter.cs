using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public interface IBounceExceptionPresenter {
        void CommandLineParameters(IEnumerable<ParameterError> errors);
        void CommandError(string output);
        void TaskException(IObsoleteTask task, string message);
        void GeneralException(BounceException bounceException);
    }
}