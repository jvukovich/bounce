using System.Collections.Generic;

namespace Bounce.Framework {
    public interface IParameters {
        Val<T> Required<T>(string name);
        Val<T> Default<T>(string name, T defaultValue);
        Val<T> OneOf<T>(string name, IEnumerable<T> availableValues);
        Val<T> OneOfWithDefault<T>(string name, T defaultValue, IEnumerable<T> availableValues);
    }
}