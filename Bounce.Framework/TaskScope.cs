using System;

namespace Bounce.Framework {
    class TaskScope : ITaskScope {
        private readonly Action<ILog> onSuccess;
        private readonly Action<ILog> onFailure;
        private readonly ILog outerLogger;
        private bool succeeded;

        public TaskScope(Action<ILog> onSuccess, Action<ILog> onFailure, ILog outerLogger) {
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;
            this.outerLogger = outerLogger;
            succeeded = false;
        }

        public void Dispose() {
            if (!succeeded) {
                onFailure(outerLogger);
            }
        }

        public void TaskSucceeded() {
            onSuccess(outerLogger);
            succeeded = true;
        }
    }
}