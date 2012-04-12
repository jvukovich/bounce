using System.Collections.Generic;
using Bounce.Framework.Obsolete;

namespace Bounce.Framework.Tests.Obsolete {
    public class FakeArtefactTask : Task {
        private HashSet<string> BuiltArtefacts;
        private readonly string ArtefactName;

        public FakeArtefactTask(HashSet<string> builtArtefacts, string artefactName) {
            BuiltArtefacts = builtArtefacts;
            ArtefactName = artefactName;
        }

        public override void Build() {
            BuiltArtefacts.Add(ArtefactName);
        }

        public override void Clean() {
            BuiltArtefacts.Remove(ArtefactName);
        }
    }
}