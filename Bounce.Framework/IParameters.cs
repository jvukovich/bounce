using System.Collections.Generic;

namespace Bounce.Framework {
    public interface IParameters {
        Future<T> Required<T>(string name);
        Future<T> Default<T>(string name, T defaultValue);
        Future<T> OneOf<T>(string name, IEnumerable<T> availableValues);
        Future<T> OneOfWithDefault<T>(string name, T defaultValue, IEnumerable<T> availableValues);
    }
}