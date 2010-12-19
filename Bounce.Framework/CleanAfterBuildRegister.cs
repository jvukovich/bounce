using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    class CleanAfterBuildRegister {
        private HashSet<ITask> NotToBeCleaned;
        private HashSet<ITask> MaybeToBeCleaned;

        public CleanAfterBuildRegister() {
            NotToBeCleaned = new HashSet<ITask>();
            MaybeToBeCleaned = new HashSet<ITask>();
        }

        public void RegisterDependency(TaskDependency dep) {
            if (dep.CleanAfterBuild) {
                MaybeToBeCleaned.Add(dep.Task);
            } else {
                NotToBeCleaned.Add(dep.Task);
            }
        }

        public IEnumerable<ITask> TasksToBeCleaned {
            get { return MaybeToBeCleaned.Except(NotToBeCleaned); }
        }
    }
}