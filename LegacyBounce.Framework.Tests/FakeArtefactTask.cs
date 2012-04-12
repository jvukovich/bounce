using System.Collections.Generic;

namespace LegacyBounce.Framework.Tests {
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