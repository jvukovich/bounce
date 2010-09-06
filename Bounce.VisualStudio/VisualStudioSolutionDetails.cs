using System.Collections.Generic;

namespace Bounce.VisualStudio {
    public class VisualStudioSolutionDetails {
        public IEnumerable<VisualStudioProjectDetails> Projects { get; set; }
    }
}