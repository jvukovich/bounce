using System;

namespace Bounce.Framework
{
    public interface ITaskParameter
    {
        string Name { get; }
        string TypeDescription { get; }
        bool IsRequired { get; }
        object DefaultValue { get; }
        Type Type { get; }
    }
}