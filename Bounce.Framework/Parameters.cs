namespace Bounce.Framework
{
    public class Parameters
    {
        private readonly TaskParameters _params;

        public Parameters(TaskParameters parms)
        {
            _params = parms;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public static Parameters Main { get; internal set; }

        // ReSharper disable once UnusedMember.Global
        public T Parameter<T>(string name)
        {
            return (T) _params.Parameter(typeof(T), name);
        }

        // ReSharper disable once UnusedMember.Global
        public T Parameter<T>(string name, T defaultValue)
        {
            return (T) _params.Parameter(typeof(T), name, defaultValue);
        }
    }
}