namespace Bounce.Framework
{
    public class Parameters
    {
        private readonly TaskParameters Params;

        public Parameters(TaskParameters parms)
        {
            Params = parms;
        }

        public static Parameters Main { get; internal set; }

        public T Parameter<T>(string name)
        {
            return (T) Params.Parameter(typeof(T), name);
        }

        public T Parameter<T>(string name, T defaultValue)
        {
            return (T) Params.Parameter(typeof(T), name, defaultValue);
        }
    }
}