using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework.Obsolete {
    class CleanAfterBuildRegister {
        private HashSet<IObsoleteTask> NotToBeCleaned;
        private HashSet<IObsoleteTask> MaybeToBeCleaned;

        public CleanAfterBuildRegister() {
            NotToBeCleaned = new HashSet<IObsoleteTask>();
            MaybeToBeCleaned = new HashSet<IObsoleteTask>();
        }

        public void RegisterDependency(TaskDependency dep) {
            if (dep.CleanAfterBuild) {
                MaybeToBeCleaned.Add(dep.Task);
            } else {
                NotToBeCleaned.Add(dep.Task);
            }
        }

        public IEnumerable<IObsoleteTask> TasksToBeCleaned {
            get { return MaybeToBeCleaned.Except(NotToBeCleaned); }
        }
    }
}