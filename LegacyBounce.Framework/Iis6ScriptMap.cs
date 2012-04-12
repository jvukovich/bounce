namespace LegacyBounce.Framework {
    public class Iis6ScriptMap {
        public string Extension;
        public int Flags {
            get {
                return (ScriptEngine ? 1 : 0) + (VerifyThatFileExists ? 4 : 0);
            }
        }
        public bool ScriptEngine;
        public bool VerifyThatFileExists;
        public bool AllVerbs {
            get {
                return _allVerbs;
            }
            set {
                _allVerbs = value;
                _includedVerbs = "";
            }
        }
        public string IncludedVerbs {
            get {
                return _includedVerbs;
            }
            set {
                _includedVerbs = value;
                _allVerbs = false;
            }
        }
        public string Executable;
        private bool _allVerbs;
        private string _includedVerbs;

        public Iis6ScriptMap() {
            AllVerbs = true;
            ScriptEngine = true;
            VerifyThatFileExists = true;
        }
    }
}