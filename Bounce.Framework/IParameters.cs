using System.Collections.Generic;

namespace Bounce.Framework {
    public interface IParameters {
        Parameter<T> Required<T>(string name);
        Parameter<T> Default<T>(string name, T defaultValue);
        Parameter<T> OneOf<T>(string name, IEnumerable<T> availableValues);
        Parameter<T> OneOfWithDefault<T>(string name, T defaultValue, IEnumerable<T> availableValues);
    }
}