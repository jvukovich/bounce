using System.Collections.Generic;

namespace Bounce {
    public interface IParameters {
        IValue<T> Required<T>(string name);
        IValue<T> Default<T>(string name, T defaultValue);
        IValue<T> OneOf<T>(string name, IEnumerable<T> availableValues);
        IValue<T> OneOfWithDefault<T>(string name, T defaultValue, IEnumerable<T> availableValues);
    }
}