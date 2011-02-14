using System.Collections.Generic;

namespace Bounce.Framework {
    public interface IParameters {
        Task<T> Required<T>(string name);
        Task<T> Default<T>(string name, T defaultValue);
        Task<T> OneOf<T>(string name, IEnumerable<T> availableValues);
        Task<T> OneOfWithDefault<T>(string name, T defaultValue, IEnumerable<T> availableValues);
    }
}