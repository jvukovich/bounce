using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;

namespace Bounce.Framework.VisualStudio {
    public class VisualStudioProjects : IEnumerable<VisualStudioProject> {
        private readonly VisualStudioSolution Solution;
        private readonly IVisualStudioProjectFileLoader ProjectLoader;
        private Dictionary<string, VisualStudioProject> LoadedProjects = new Dictionary<string, VisualStudioProject>();
        private readonly IEnumerable<VisualStudioSolutionProjectReference> ProjectReferences;
        private readonly string Configuration;

        public VisualStudioProjects(VisualStudioSolution solution, IEnumerable<VisualStudioSolutionProjectReference> projectReferences, string configuration) {
            Solution = solution;
            ProjectLoader = new VisualStudioCSharpProjectFileLoader();
            ProjectReferences = projectReferences;
            Configuration = configuration;
        }

        public VisualStudioProject this[string projectName] {
            get {
                var reference = ProjectReferences.SingleOrDefault(p => p.Name == projectName);
                if (reference != null) {
                    return LoadProject(reference);
                } else {
                    throw new ApplicationException("no such project: " + projectName);
                }
            }
        }

        public IEnumerator<VisualStudioProject> GetEnumerator() {
            return ProjectReferences.Select(LoadProject).GetEnumerator();
        }

        private VisualStudioProject LoadProject(VisualStudioSolutionProjectReference reference) {
            VisualStudioProject project;
            if (!LoadedProjects.TryGetValue(reference.Name, out project)) {
                project = ProjectLoader.LoadProject(reference.Path, reference.Name, Configuration);
                project.Solution = Solution;
            }
            return project;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}