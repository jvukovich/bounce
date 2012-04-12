namespace LegacyBounce.Framework {
    public abstract class Iis6Task : Task {
        [Dependency]
        public Task<string> Machine;

        public Iis6Task() {
            Machine = "localhost";
        }

        private IisService _iis;

        protected IisService Iis {
            get {
                if (_iis == null) {
                    _iis = new IisService(Machine.Value);
                }
                return _iis;
            }
        }
    }
}