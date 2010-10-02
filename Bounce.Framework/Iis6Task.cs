using System;

namespace Bounce.Framework {
    public abstract class Iis6Task : Task {
        public Val<string> Machine;

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

        protected static void WithOptionalProperty<T>(Val<T> property, Action<T> action) where T : class {
            if (property != null) {
                var prop = property.Value;
                if (prop != null) {
                    action(prop);
                }
            }
        }
    }
}